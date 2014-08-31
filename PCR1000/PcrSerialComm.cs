﻿/*
 * PcrComm
 * Serial communication component of the PCR1000 Library
 * 
 * Copyright Matthew Knox © 2013-Present.
 * This program is distributed with no warentee or garentee
 * what so ever. Do what you want with it as long as attribution
 * to the origional authour and this comment is provided at the
 * top of this source file and any derivative works. Also any
 * modifications must be in real Australian, New Zealand or
 * British English where the language allows.
 */

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace PCR1000
{
    /// <summary>
    /// PCR1000 COM Port Communications Class
    /// </summary>
    public class PcrSerialComm : IComm
    {
        /// <summary>
        /// Data received in autoupdate mode.
        /// </summary>
        public event AutoUpdateDataRecv DataReceived;

        /// <summary>
        /// The serial port of the PCR1000.
        /// </summary>
        private readonly SerialPort _serialPort;

        /// <summary>
        /// Gets and sets autoupdate mode.
        /// </summary>
        public bool AutoUpdate { get; set; }

        /// <summary>
        /// Number of 50ms timeouts to wait before aborting in SendWait.
        /// </summary>
        private const int RecvTimeout = 20;

        /// <summary>
        /// Unnessercery characters potentially returned in each message.
        /// </summary>
        private static readonly char[] TrimChars = { '\n', '\r', ' ', '\t', '\0' };

        /// <summary>
        /// Last two received messages.
        /// </summary>
        private RecvMsg _msgSlot1, _msgSlot2;

        /// <summary>
        /// Received message structure.
        /// </summary>
        private struct RecvMsg
        {
            /// <summary>
            /// The message received.
            /// </summary>
            public string Message;

            /// <summary>
            /// The time the message was received.
            /// </summary>
            public DateTime Time;
        }

        /// <summary>
        /// Instantiate a new PcrComm object to communicate with the PCR1000.
        /// </summary>
        /// <param name="port">COM Port to communicate on. Defaults to COM1</param>
        /// <param name="baud">Baud rate to use. Defaults to 9600.</param>
        public PcrSerialComm(string port = "COM1", int baud = 9600)
        {
            Debug.WriteLine("PcrComm Being Created");
            _serialPort = new SerialPort(port, baud, Parity.None, 8, StopBits.One);
            _serialPort.DataReceived += SerialPortDataReceived;
            _serialPort.DtrEnable = true;
            _serialPort.Handshake = Handshake.RequestToSend;
            Debug.WriteLine("PcrComm Created");
        }

        /// <summary>
        /// Gets the object refering to the raw communications port.
        /// </summary>
        /// <returns>The communications port object.</returns>
        public object GetRawPort()
        {
            Debug.WriteLine("PcrComm Returning Raw Port");
            return _serialPort;
        }

#if DEBUG
        /// <summary>
        /// Keeps track of wheather debug logging is enabled.
        /// </summary>
        private bool _debugLogger;

        /// <summary>
        /// Enables or disables debug logging in the comminication library.
        /// </summary>
        /// <param name="debug">Enable or disable.</param>
        public void SetDebugLogger(bool debug)
        {
            Debug.WriteLine("PcrComm Debug Logging: " + debug);
            _debugLogger = debug;
        }
#endif
        /// <summary>
        /// Method called when data is received from the serial port.
        /// </summary>
        /// <param name="sender">The serial port that called the method.</param>
        /// <param name="e">Event arguments.</param>
        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
#if DEBUG
            Debug.WriteLineIf(!_debugLogger, "PcrComm Data Recv");
#endif
            try
            {
                var recvBuff = new byte[_serialPort.ReadBufferSize];
                _serialPort.Read(recvBuff, 0, _serialPort.ReadBufferSize);
                var str = _serialPort.Encoding.GetString(recvBuff);
                _serialPort.DiscardInBuffer();
                str = str.Trim(TrimChars);

                if (AutoUpdate && DataReceived != null)
                {
                    DataReceived(this, DateTime.Now, str);
                }

                _msgSlot2 = _msgSlot1;
                _msgSlot1 = new RecvMsg { Message = str, Time = DateTime.Now };

#if DEBUG
                Debug.WriteLineIf(_debugLogger, _serialPort.PortName + ": RECV -> " + str);
#endif
            }
            catch (Exception)
            {
                Debug.WriteLine("RECV:ERR");
            }
        }

        /// <summary>
        /// Sends a messsage to the PCR1000.
        /// </summary>
        /// <param name="cmd">The command to send.</param>
        /// <returns>If sending succeeded.</returns>
        public bool Send(string cmd)
        {
#if DEBUG
            Debug.WriteLineIf(!_debugLogger, "PcrComm Data Send");
            Debug.WriteLineIf(_debugLogger, _serialPort.PortName + ": SEND -> " + cmd);
#endif
            try
            {
                if (!_serialPort.IsOpen)
                {
                    if (!PcrOpen())
                    {
                        return false;
                    }
                }
                if (!AutoUpdate)
                {
                    cmd += "\r\n";
                }
                _serialPort.Write(cmd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends a message to the PCR1000 and waits for a reply.
        /// </summary>
        /// <param name="cmd">The command to send.</param>
        /// <param name="overrideAutoupdate">When in autoupdate mode behaves like Send()
        /// this overrides that behaviour.</param>
        /// <returns>The reply or "" if nothing is received.</returns>
        public string SendWait(string cmd, bool overrideAutoupdate = false)
        {
            Debug.WriteLine("PcrComm SendWait");
            Send(cmd);
            if (AutoUpdate && !overrideAutoupdate) return "";
            var dt = DateTime.Now;
            for (var i = 0; i < RecvTimeout; i++)
            {
                if (dt < _msgSlot1.Time)
                {
                    return dt < _msgSlot2.Time ? _msgSlot2.Message : _msgSlot1.Message;
                }
                Thread.Sleep(50);
            }
            return "";
        }

        /// <summary>
        /// Gets the latest message from the PCR1000.
        /// </summary>
        /// <returns>The latest message.</returns>
        public string GetLastReceived()
        {
            Debug.WriteLine("PcrComm Last Recv");
            try
            {
                return _msgSlot1.Message;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the previously received message.
        /// </summary>
        /// <returns>The previous message.</returns>
        public string GetPrevReceived()
        {
            Debug.WriteLine("PcrComm PrevRecv");
            try
            {
                return _msgSlot2.Message;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Opens the PCR1000 serial port.
        /// </summary>
        /// <returns>Success</returns>
        public bool PcrOpen()
        {
            Debug.WriteLine("PcrComm Open");
            try
            {
                if (_serialPort.IsOpen) return true;
                _serialPort.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Closes the PCR1000 serial port.
        /// </summary>
        /// <returns>Success</returns>
        public bool PcrClose()
        {
            Debug.WriteLine("PcrComm Close");
            try
            {
                if (!_serialPort.IsOpen) return true;
                _serialPort.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            Debug.WriteLine("PcrComm Dispose");
            PcrClose();
            _serialPort.Dispose();
        }
    }
}
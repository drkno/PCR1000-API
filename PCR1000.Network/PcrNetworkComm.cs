/*
 * PcrNetwork
 * Network communication component of the PCR1000 Library
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
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCR1000.Network
{
    /// <summary>
    /// Client to connect to a remote radio.
    /// </summary>
    public class PcrNetworkClient : IComm
    {
        private Thread _tcpListen;
        private Stream _tcpStream;
        private TcpClient _tcpClient;
        private byte[] _tcpBuffer;
        // ReSharper disable ConvertToConstant.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private bool _listenActive;
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore ConvertToConstant.Local
        private readonly string _server;
        private readonly int _port;
        private readonly bool _ssl;

        /// <summary>
        /// Unnessercery characters potentially returned in each message.
        /// </summary>
        private static readonly char[] TrimChars = { '\n', '\r', ' ', '\t', '\0' };

        /// <summary>
        /// Number of 50ms timeouts to wait before aborting in SendWait.
        /// </summary>
        private const int RecvTimeout = 20;

        /// <summary>
        /// Last two received messages.
        /// </summary>
        private RecvMsg _msgSlot1, _msgSlot2;

        /// <summary>
        /// Password to transmit to server (if requested)
        /// </summary>
        private readonly string _password;

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

        private void ListenThread()
        {
            try
            {
                while (_listenActive)
                {
                    var lData = _tcpStream.Read(_tcpBuffer, 0, _tcpClient.ReceiveBufferSize);
                    var datarecv = Encoding.ASCII.GetString(_tcpBuffer);
                    datarecv = datarecv.Substring(0, lData);
                    datarecv = datarecv.Trim(TrimChars);
                    if (datarecv.Length <= 0) continue;
#if DEBUG
                    Debug.WriteLineIf(!_debugLogger, "PcrNetwork Data Recv");
#endif
                    if (AutoUpdate)
                    {
                        DataReceived?.Invoke(this, DateTime.Now, datarecv);
                    }

                    _msgSlot2 = _msgSlot1;
                    _msgSlot1 = new RecvMsg {Message = datarecv, Time = DateTime.Now};

#if DEBUG
                    Debug.WriteLineIf(_debugLogger, _server + ":" + _port + " : RECV -> " + datarecv);
#endif
                }
            }
            catch (ThreadAbortException)
            {
                Debug.WriteLine("Listen thread aborting...");
            }
            catch (IOException e)
            {
                Debug.WriteLine("A socket read failure occurred:\n" + e.Message + "\r\n" + e.StackTrace);
            }
        }

        /// <summary>
        /// Instantiates a new PCR1000 network client.
        /// </summary>
        /// <param name="server">The server to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="ssl">Use SSL to secure connections. This MUST be symmetric.</param>
        /// <param name="password">The password to connect with.</param>
        /// <exception cref="ArgumentException">If invalid arguments are provided.</exception>
        public PcrNetworkClient(string server, int port = 4456, bool ssl = false, string password = "")
        {
            if (string.IsNullOrWhiteSpace(server) || port <= 0)
            {
                throw new ArgumentException("Invalid Instantiation Arguments");
            }
            _password = password;
            _server = server;
            _port = port;
            _ssl = ssl;
        }

        /// <summary>
        /// Disposes of the PcrNetworkClient
        /// </summary>
        public void Dispose()
        {
            if (!_tcpClient.Connected) return;
            _tcpListen.Abort();
            _tcpListen.Join();
            _tcpClient.Close();
        }

        /// <summary>
        /// Data was received from the remote radio.
        /// </summary>
        public event AutoUpdateDataRecv DataReceived;
        /// <summary>
        /// The remote radio should auto-update.
        /// </summary>
        public bool AutoUpdate { get; set; }
        /// <summary>
        /// Gets the underlying system port of the radio.
        /// </summary>
        /// <returns>The network port connected to the remote radio.</returns>
        public object GetRawPort()
        {
            return _tcpClient;
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
            _debugLogger = debug;
        }
#endif

        /// <summary>
        /// Sends a command to the radio.
        /// </summary>
        /// <param name="cmd">Command to send.</param>
        /// <returns>Success.</returns>
        public bool Send(string cmd)
        {
            try
            {
                if (!AutoUpdate)
                {
                    cmd += "\r\n";
                }
                _tcpStream.Write(Encoding.ASCII.GetBytes(cmd), 0, cmd.Length);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
            Debug.WriteLine("PcrNetwork SendWait");
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
            Debug.WriteLine("PcrNetwork Last Recv");
            try
            {
                return _msgSlot1.Message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Gets the previously received message.
        /// </summary>
        /// <returns>The previous message.</returns>
        public string GetPrevReceived()
        {
            Debug.WriteLine("PcrNetwork PrevRecv");
            try
            {
                return _msgSlot2.Message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Opens the connection to the remote radio.
        /// </summary>
        /// <returns>Success.</returns>
        public bool PcrOpen()
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    return true;
                }
                _tcpClient = new TcpClient(_server, _port);
                _tcpStream = _ssl ? (Stream) new SslStream(_tcpClient.GetStream()) : _tcpClient.GetStream();
                _tcpBuffer = new byte[_tcpClient.ReceiveBufferSize];
                _listenActive = true;
                _tcpListen = new Thread(ListenThread);
                _tcpListen.Start();
                if (!string.IsNullOrWhiteSpace(_password))
                {
                    Send("<pwd>" + _password + "</pwd>");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Closes the connection to the remote radio.
        /// </summary>
        /// <returns>Success.</returns>
        public bool PcrClose()
        {
            try
            {
                if (_tcpClient == null)
                {
                    return true;
                }
                Send("<disconnect>");
                _listenActive = false;
                _tcpListen.Abort();
                _tcpBuffer = null;
                _tcpStream.Close();
                _tcpClient.Close();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}

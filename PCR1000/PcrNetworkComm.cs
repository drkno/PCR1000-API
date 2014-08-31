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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCR1000
{
    /// <summary>
    /// 
    /// </summary>
    public class PcrNetworkClient : IComm
    {
        private Thread _tcpListen;
        private NetworkStream _tcpStream;
        private TcpClient _tcpClient;
        private byte[] _tcpBuffer;
        // ReSharper disable ConvertToConstant.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private bool _listenActive;
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore ConvertToConstant.Local
        private readonly string _server;
        private readonly int _port;

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

                    if (datarecv.StartsWith("<") && datarecv.EndsWith(">")) // server command
                    {
                        switch (datarecv.Substring(1,datarecv.Length-2))
                        {
                            case "authnow":
                            {
                                Send("<pwd=\"" + _password + "\">");
#if DEBUG
                                Debug.WriteLineIf(_debugLogger, _server + ":" + _port + " : Request for authentication -> " + datarecv);
#endif
                                continue;
                            }
                        }
                    }

                    if (AutoUpdate && DataReceived != null)
                    {
                        DataReceived(this, DateTime.Now, datarecv);
                    }

                    _msgSlot2 = _msgSlot1;
                    _msgSlot1 = new RecvMsg { Message = datarecv, Time = DateTime.Now };

#if DEBUG
                    Debug.WriteLineIf(_debugLogger, _server + ":" + _port + " : RECV -> " + datarecv);
#endif
                }
            }
            catch (ThreadAbortException)
            {
                Debug.WriteLine("Listen thread aborting...");
            }
        }

        /// <summary>
        /// Instantiates a new PCR1000 network client.
        /// </summary>
        /// <param name="server">The server to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="password">The password to connect with.</param>
        /// <exception cref="ArgumentException">If invalid arguments are provided.</exception>
        public PcrNetworkClient(string server, int port = 4456, string password = "")
        {
            if (string.IsNullOrWhiteSpace(server) || port <= 0)
            {
                throw new ArgumentException("Invalid Instantiation Arguments");
            }
            _password = password;
            _server = server;
            _port = port;
            PcrOpen();
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public event AutoUpdateDataRecv DataReceived;
        public bool AutoUpdate { get; set; }
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

        public bool PcrOpen()
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    return true;
                }
                _tcpClient = new TcpClient(_server, _port);
                _tcpStream = _tcpClient.GetStream();
                _tcpBuffer = new byte[_tcpClient.ReceiveBufferSize];
                _listenActive = true;
                _tcpListen = new Thread(ListenThread);
                _tcpListen.Start();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

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

    /// <summary>
    /// 
    /// </summary>
    public class PcrNetworkServer
    {
        public int Port { get; protected set; }

        /// <summary>
        /// The serial port of the PCR1000.
        /// </summary>
        public IComm PortComm { get; protected set; }

        private TcpListener _tcpListener;

        private TcpClient tcpClient;

        private string _password;
        private bool _isAuthenticated;

        /// <summary>
        /// Instantiate a new PcrNetwork object to communicate with the PCR1000.
        /// </summary>
        /// <param name="pcrComm">Method of communication to use to connect to the radio.</param>
        /// <param name="netport">Network port to communucate on. Defaults to 4456.</param>
        /// <param name="password">Password to use. Defaults to none.</param>
        public PcrNetworkServer(IComm pcrComm, int netport = 4456, string password = "")
        {
            Debug.WriteLine("PcrNetwork Being Created");
            _password = password;
            if (_password == "") _isAuthenticated = true;
            Port = netport;
            pcrComm.DataReceived += PcrCommOnDataReceived;
            Debug.WriteLine("PcrNetwork Created");
        }

        private bool listenContinue = true;

        private void ListenForClients()
        {
            _tcpListener.Start();

            while (listenContinue)
            {
                var client = _tcpListener.AcceptTcpClient();
                var clientThread = new Thread(ListenForCommands);
                clientThread.Start(client);
            }
        }

        private void ListenForCommands(object obj)
        {
            tcpClient = (TcpClient)obj;
            var clientStream = tcpClient.GetStream();

            while (true)
            {
                try
                {
                    var message = new byte[tcpClient.ReceiveBufferSize];
                    var bytesRead = clientStream.Read(message, 0, tcpClient.ReceiveBufferSize);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    var cmd = Encoding.ASCII.GetString(message, 0, bytesRead);

                    if (cmd.StartsWith("<") && cmd.EndsWith(">"))
                    {
                        
                    }

                    if (!_isAuthenticated)
                    {
                        var stream = tcpClient.GetStream();
                        stream.Write(Encoding.ASCII.GetBytes("<authnow>"), 0, 9);
                    }
                    else
                    {
                        //TODO:_serialPort.Write(cmd);
                    }

#if DEBUG
                    Debug.WriteLineIf(_debugLogger, "Network : RECV -> " + cmd);
#endif
                }
                catch
                {
                    break;
                }
            }

            tcpClient.Close();
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
            Debug.WriteLine("PcrNetwork Debug Logging: " + debug);
            _debugLogger = debug;
        }
#endif
        /// <summary>
        /// Method called when data is received from the serial port.
        /// </summary>
        /// <param name="sender">The serial port that called the method.</param>
        /// <param name="e">Event arguments.</param>

        private void PcrCommOnDataReceived(IComm sender, DateTime recvTime, string data)
        {
            /*throw new NotImplementedException();
        }
        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {*/
#if DEBUG
            Debug.WriteLineIf(!_debugLogger, "PcrNetwork Data Recv");
#endif
            try
            {
                /*var recvBuff = new byte[_serialPort.ReadBufferSize];
                _serialPort.Read(recvBuff, 0, _serialPort.ReadBufferSize);
                var str = _serialPort.Encoding.GetString(recvBuff);
                _serialPort.DiscardInBuffer();*/

                if (tcpClient != null)
                {
                    var stream = tcpClient.GetStream();
                    //TODO:stream.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
                }

#if DEBUG
                Debug.WriteLineIf(_debugLogger, _serialPort.PortName + ": RECV -> " + str);
#endif
            }
            catch (Exception)
            {
                Debug.WriteLine("RECV:ERR");
            }
        }

        public bool Start()
        {
            Debug.WriteLine("PCR::NETS->Start");
            try
            {
                if (listenContinue || !PortComm.PcrOpen())
                {
                    return false;
                }

                listenContinue = true;
                _tcpListener = new TcpListener(IPAddress.Any, Port);
                var listenThread = new Thread(ListenForClients);
                listenThread.Start();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool Stop()
        {
            Debug.WriteLine("PCR::NETS->Stop");
            try
            {
                if (!listenContinue || !PortComm.PcrClose())
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}

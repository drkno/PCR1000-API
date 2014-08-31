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
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCR1000
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

        public void Dispose()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public event AutoUpdateDataRecv DataReceived;
        /// <summary>
        /// 
        /// </summary>
        public bool AutoUpdate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// Server class to manage remote connections.
    /// </summary>
    public class PcrNetworkServer
    {
        /// <summary>
        /// Network port to use. By default this is 4456.
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// The serial port of the PCR1000.
        /// </summary>
        private readonly IComm _portComm;

        private TcpListener _tcpListener;

        private TcpClient _tcpClient;

        private readonly bool _ssl;
        private readonly string _password;
        private bool _isAuthenticated;

        /// <summary>
        /// Instantiate a new PcrNetwork object to communicate with the PCR1000.
        /// </summary>
        /// <param name="pcrComm">Method of communication to use to connect to the radio.</param>
        /// <param name="netport">Network port to communucate on. Defaults to 4456.</param>
        /// <param name="ssl">Use SSL to secure connections. This MUST be symmetric.</param>
        /// <param name="password">Password to use. Defaults to none.</param>
        public PcrNetworkServer(IComm pcrComm, int netport = 4456, bool ssl = false, string password = "")
        {
            Debug.WriteLine("PcrNetwork Being Created");
            _ssl = ssl;
            _password = password;
            if (string.IsNullOrWhiteSpace(_password)) _isAuthenticated = true;
            _port = netport;
            _portComm = pcrComm;
            pcrComm.DataReceived += PcrCommOnDataReceived;
            Debug.WriteLine("PcrNetwork Created");
        }

        private bool _listenContinue;

        /// <summary>
        /// Listen for a new incoming connection.
        /// </summary>
        private void ListenForClients()
        {
            _tcpListener.Start();

            while (_listenContinue)
            {
                var client = _tcpListener.AcceptTcpClient();
                var clientThread = new Thread(ListenForCommands);
                clientThread.Start(client);
            }
        }

        private void Send(string cmd)
        {
            var stream = _tcpClient.GetStream();
            stream.Write(Encoding.ASCII.GetBytes(cmd), 0, cmd.Length);
        }

        /// <summary>
        /// Listens for commands from a TcpClient.
        /// </summary>
        /// <param name="obj">The TcpClient</param>
        private void ListenForCommands(object obj)
        {
            _tcpClient = (TcpClient)obj;
            var clientStream = _ssl ? (Stream) new SslStream(_tcpClient.GetStream()) : _tcpClient.GetStream();

            while (true)
            {
                try
                {
                    var message = new byte[_tcpClient.ReceiveBufferSize];
                    var bytesRead = clientStream.Read(message, 0, _tcpClient.ReceiveBufferSize);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    var cmd = Encoding.ASCII.GetString(message, 0, bytesRead);

                    if (!_isAuthenticated)
                    {
                        if (cmd.StartsWith("<pwd>") && cmd.EndsWith("</pwd>"))
                        {
                            cmd = cmd.Substring(5, cmd.Length - 11);
                            if (cmd == _password)
                            {
                                _isAuthenticated = true;
                            }
                            else
                            {
                                Send("<auth>fail</auth>");
                                Stop();
                                Debug.WriteLine("Network: RECV -> [INVALID AUTH]");
                            }
                        }
                        else
                        {
                            Send("<auth>required</auth>");
                        }
                    }
                    else
                    {
                        _portComm.Send(cmd);
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

            _tcpClient.Close();
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
        /// Method called when data is received from the communication port.
        /// </summary>
        /// <param name="sender">The IComm that called the method.</param>
        /// <param name="recvTime">The time that the message was received.</param>
        /// <param name="data">Data received.</param>
        private void PcrCommOnDataReceived(IComm sender, DateTime recvTime, string data)
        {
#if DEBUG
            Debug.WriteLineIf(!_debugLogger, "PCR::NETS->OnDataReceived");
#endif
            try
            {
                if (_tcpClient != null)
                {
                    var stream = _tcpClient.GetStream();
                    stream.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
                }
#if DEBUG
                Debug.WriteLineIf(_debugLogger, "RECV -> " + data);
#endif
            }
            catch (Exception)
            {
                Debug.WriteLine("RECV:ERR");
            }
        }

        private Thread _listenThread;

        /// <summary>
        /// Starts the network server.
        /// </summary>
        /// <returns>Success.</returns>
        public bool Start()
        {
            Debug.WriteLine("PCR::NETS->Start");
            try
            {
                if (_listenContinue || !_portComm.PcrOpen())
                {
                    return false;
                }

                _listenContinue = true;
                _tcpListener = new TcpListener(IPAddress.Any, _port);
                _listenThread = new Thread(ListenForClients);
                _listenThread.Start();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Stops the network server.
        /// </summary>
        /// <returns>Success.</returns>
        public bool Stop()
        {
            Debug.WriteLine("PCR::NETS->Stop");
            try
            {
                if (!_listenContinue || !_portComm.PcrClose())
                {
                    return false;
                }
                _listenContinue = false;
                _tcpListener.Stop();
                _listenThread.Abort();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}

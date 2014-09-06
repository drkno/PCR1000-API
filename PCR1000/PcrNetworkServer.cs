﻿using System;
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
            try
            {
                _tcpListener.Start();

                while (_listenContinue)
                {
                    var client = _tcpListener.AcceptTcpClient();
                    var clientThread = new Thread(ListenForCommands);
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Listen encountered exception: " + ex.Message);
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
            var clientStream = _ssl ? (Stream)new SslStream(_tcpClient.GetStream()) : _tcpClient.GetStream();

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
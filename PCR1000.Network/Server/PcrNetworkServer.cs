using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCR1000.Network.Server
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
        private readonly List<ConnectedClient> _clients;
        private long _clientCounter, _activeClient;
        private readonly bool _tls;
        private readonly string _password;

        /// <summary>
        /// Instantiate a new PcrNetwork object to communicate with the PCR1000.
        /// </summary>
        /// <param name="pcrComm">Method of communication to use to connect to the radio.</param>
        /// <param name="netport">Network port to communucate on. Defaults to 4456.</param>
        /// <param name="tls">Use TLS to secure connections. This MUST be symmetric.</param>
        /// <param name="password">Password to use. Defaults to none.</param>
        public PcrNetworkServer(IComm pcrComm, int netport = 4456, bool tls = false, string password = "")
        {
            Debug.WriteLine($"PcrNetwork Being Created: port={netport} tls={tls} password=\"{password}\"");
            _tls = tls;
            _password = password;
            _clients = new List<ConnectedClient>();
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
                    var tcpClient = _tcpListener.AcceptTcpClient();
                    var cc = _clientCounter++;
                    Func<string, bool> send = cmd => Send(cc, cmd);
                    Func<bool, bool> takeControl = b =>
                    {
                        if (b) _activeClient = cc;
                        return _activeClient == cc;
                    };
                    var client = new ConnectedClient(send, takeControl, tcpClient, _tls, _password);
                    _clients.Add(client);
                    client.OnDisconnect += s => _clients.Remove(client);
                    _activeClient = cc;
                    var clientThread = new Thread(client.Listen) {IsBackground = true};
                    clientThread.Start(clientThread);
                }
            }
            catch (Exception ex)
            {
                if (_listenContinue)
                {
                    Debug.WriteLine("Listen encountered exception: " + ex.Message);
                }
                else
                {
                    Debug.WriteLine("Listen thread shutting down.");
                }
            }
        }

        private bool Send(long client, string cmd)
        {
            return _activeClient == client && _portComm.Send(cmd);
        }

        /// <summary>
        /// Method called when data is received from the communication port.
        /// </summary>
        /// <param name="sender">The IComm that called the method.</param>
        /// <param name="recvTime">The time that the message was received.</param>
        /// <param name="data">Data received.</param>
        private void PcrCommOnDataReceived(IComm sender, DateTime recvTime, string data)
        {
            ConnectedClient last = null;
            try
            {
                var bytes = Encoding.ASCII.GetBytes(data);
                foreach (var client in _clients)
                {
                    last = client;
                    client.Send(bytes);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("RECV:ERR The client probably disconnected prematurely.\n" + e.Message + "\n" + e.StackTrace);
                last?.ForceDisconnect();
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
            catch (Exception e)
            {
                Debug.WriteLine("PCR::NETS->Start Failed to start server with error:\n" + e.Message + "\n" + e.StackTrace);
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
                _listenThread.Join();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PCR1000.Network.Server
{
    internal sealed class ConnectedClient
    {
        private const string ServerPrefix = "$";
        private const float ProtocolVersion = 2.0f;
        
        public event Action<object> OnDisconnect;

        private readonly Func<string, bool> _sendFunc;
        private readonly Func<bool, bool> _hasControl; 
        private readonly TcpClient _tcpClient;
        private readonly Stream _networkStream;
        private bool _isAuthenticated, _hasHelloed, _shouldListen;
        private readonly string _password;
        private Thread _listenThread;

        internal ConnectedClient(Func<string, bool> sendFunc, Func<bool, bool> hasControl, TcpClient tcpClient, bool tls, string password)
        {
            _sendFunc = sendFunc;
            _hasControl = hasControl;
            _tcpClient = tcpClient;
            _networkStream = tls ? (Stream)new SslStream(tcpClient.GetStream()) : tcpClient.GetStream();
            _isAuthenticated = string.IsNullOrEmpty(password);
            _password = password;
            _shouldListen = true;
#if DEBUG
            var endPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
            Debug.WriteLine($"Network: Client Connected ip={endPoint?.Address} port={endPoint?.Port}");
#endif
        }

        internal void Send(byte[] bytes)
        {
            if (!_hasHelloed || !_isAuthenticated)
            {
                return;
            }
            InternalSend(bytes);
        }

        private void Send(ClientErrorCode code, string message)
        {
            Debug.WriteLine($"Network Send: code={code} message=\"{message}\"");
            var cmd = ServerPrefix + code + " \"" + message + "\"";
            InternalSend(Encoding.UTF8.GetBytes(cmd));
        }

        private void InternalSend(byte[] bytes)
        {
            if (!_shouldListen) return;
            _networkStream.Write(bytes, 0, bytes.Length);
        }

        private void Disconnect(ClientErrorCode errorCode, string message)
        {
            Send(errorCode, message);
            _shouldListen = false;
            _tcpClient.Close();
            OnDisconnect?.Invoke(this);
        }

        #region Server Commands
        private void HandleClientHello(string cmd)
        {
            const int helloFields = 2;

            if (!cmd.StartsWith("$HELLO "))
            {
                Disconnect(ClientErrorCode.ERR_HELLO_NOTFOUND, "Polite Clients Say Hello");
                return;
            }

            var split = cmd.Split(' ');
            if (split.Length < helloFields)
            {
                Disconnect(ClientErrorCode.ERR_HELLO_INVALID, "Unknown Client Hello");
                return;
            }

            if (split.Length > helloFields)
            {
                Send(ClientErrorCode.WAR_HELLO_UNKNOWN, "Client Hello Contains Unknown Values");
            }

            float protoVersion;
            if (!float.TryParse(split[1], out protoVersion))
            {
                Disconnect(ClientErrorCode.ERR_HELLO_INVALID, "Unknown Client Hello");
                return;
            }

            if (protoVersion < ProtocolVersion)
            {
                Disconnect(ClientErrorCode.ERR_PROTOVER_TOOOLD, "Protocol Version of Client is Too Old");
                return;
            }
            Send(ClientErrorCode.SUC_HELLO_PASSED, "Client has connected to server.");
            _hasHelloed = true;
        }

        private void HandleClientAuthentication(string cmd)
        {
            if (!cmd.StartsWith("$AUTH "))
            {
                Send(ClientErrorCode.ERR_AUTH_NOTFOUND, "Authentication is required.");
                return;
            }

            var regex = Regex.Match(cmd, "(?<=(^\\$AUTH \"))[^\"]+(?=\")", RegexOptions.Compiled);

            if (!regex.Success)
            {
                Disconnect(ClientErrorCode.ERR_AUTH_INVALID, "Unknown Client Authentication");
                return;
            }

            if (regex.Value != _password)
            {
                Disconnect(ClientErrorCode.ERR_AUTH_INCORRECT, "The provided authorisation was incorrect.");
                return;
            }

            Send(ClientErrorCode.SUC_AUTH_PASSED, "Client has authenticated with server.");
            _isAuthenticated = true;
        }

        private void HandleServerCommand(string cmd)
        {
            if (!_hasHelloed)
            {
                HandleClientHello(cmd);
                return;
            }

            if (!_isAuthenticated)
            {
                HandleClientAuthentication(cmd);
                return;
            }

            switch (cmd?.Trim())
            {
                case "$ECHO":
                    Send(ClientErrorCode.SUC_ECHO_RESPONSE, "Reply name=\"" + Assembly.GetEntryAssembly().FullName + "\" proto=\"" + ProtocolVersion + "\"");
                    break;

                case "$HASCONTROL":
                    Send(ClientErrorCode.SUC_HASCONTROL_RESPONSE, _hasControl(false) ? "Yes" : "No");
                    break;

                case "$TAKECONTROL":
                    Send(ClientErrorCode.SUC_TAKECONTROL_RESPONSE, _hasControl(true) ? "Yes" : "No");
                    break;

                case "$DISCONNECT":
                   // Disconnect(ClientErrorCode.INF_CLIENT_DISCONNECT, "Client initiated disconnect.");
                    break;

                default:
                    Send(ClientErrorCode.WAR_COMMAND_UNKNOWN, "The command \"" + cmd + "\" is unknown.");
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Listens for commands from a TcpClient.
        /// </summary>
        /// <param name="listenThread">The thread that this method is being executed on.</param>
        internal void Listen(object listenThread)
        {
            _listenThread = (Thread) listenThread;
            while (_shouldListen)
            {
                try
                {
                    var message = new byte[_tcpClient.ReceiveBufferSize];
                    var bytesRead = _networkStream.Read(message, 0, _tcpClient.ReceiveBufferSize);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    var cmd = Encoding.ASCII.GetString(message, 0, bytesRead);
                    if (cmd.StartsWith(ServerPrefix) || !_isAuthenticated || !_hasHelloed)
                    {
                        HandleServerCommand(cmd);
                        continue;
                    }

                    Debug.WriteLine($"{_tcpClient.Client.RemoteEndPoint as IPEndPoint}: RECV -> " + cmd);
                    if (_sendFunc(cmd)) continue;
                    if (_hasControl(false))
                    {
                        Debug.WriteLine("Network: Query Failed: " + cmd);
                        Send(ClientErrorCode.ERR_QUERY_FAILED, "The command provided failed.");
                    }
                    else
                    {
                        Debug.WriteLine("Network: Another client has control.");
                        Send(ClientErrorCode.ERR_HASCONTROL_RESPONSE, "No");
                    }
                }
                catch (Exception e)
                {
                    // client connected to a tls server without a tls stream
                    if (e is InvalidOperationException && e.Message.Contains("authenticated") && _networkStream is SslStream)
                    {
                        Debug.WriteLine("Client connected to TLS server without TLS stream. Disconnecting...");
                    }
                    else
                    {
                        Debug.WriteLine("Client disconnect with exception: " + e.Message + "\n" + e.StackTrace);
                    }
                    return;
                }
            }
            
            Disconnect(ClientErrorCode.INF_CLIENT_DISCONNECT, "Client has initiated disconnect.");
        }

        /// <summary>
        /// Forces the disconnect of a client after they do not follow protocol.
        /// </summary>
        internal void ForceDisconnect()
        {
            _shouldListen = false;
            _tcpClient.Close();
            OnDisconnect?.Invoke(this);
            _listenThread.Abort();
        }
    }
}

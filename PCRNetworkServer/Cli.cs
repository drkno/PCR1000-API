using System;
using PCR1000;
using PCR1000.Network;

namespace PCRNetworkServer
{
    public static class Cli
    {
        private static PcrNetworkServer _pcrNetworkServer;

        public static void Run(bool security, string password, int port, string device)
        {
            var title = Console.Title;
            Console.Title = "Network Server";
            var conCol = Console.ForegroundColor;

            _pcrNetworkServer = !string.IsNullOrWhiteSpace(password) || security ?
                        new PcrNetworkServer(new PcrSerialComm(device), port, security, password) :
                        new PcrNetworkServer(new PcrSerialComm(device), port);
            #if DEBUG
            _pcrNetworkServer.SetDebugLogger(true);
            #endif

            if (!_pcrNetworkServer.Start())
            {
                throw new InvalidOperationException("Starting the network server failed.");
            }

            Console.WriteLine("Server Started. Press any key to stop...");
            Console.ReadKey(true);

            if (!_pcrNetworkServer.Stop())
            {
                throw new InvalidOperationException("Stopping the network server failed.");
            }
            Console.ForegroundColor = conCol;
            Console.Title = title;
        }
    }
}

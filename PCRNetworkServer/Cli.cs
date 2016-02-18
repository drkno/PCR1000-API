using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using PCR1000;
using PCR1000.Network.Server;

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
            Console.WriteLine("############################################\n" +
                              "#                                          #\n" +
                              "#         PCR1000 Network Server           #\n" +
                              "#  <https://github.com/mrkno/PCR1000-API>  #\n" +
                              "#                                          #\n" +
                              "############################################");

            _pcrNetworkServer = new PcrNetworkServer(new PcrSerialComm(device), port, security, password);

            if (!_pcrNetworkServer.Start())
            {
                throw new InvalidOperationException("Starting the network server failed.");
            }

            Console.WriteLine($"IP:\t\t{GetLocalIPAddress()}");
            Console.WriteLine($"Port:\t\t{port}");
            Console.WriteLine($"TLS:\t\t{(security?"Enabled":"Disabled")}");
            Console.WriteLine($"Password:\t{(string.IsNullOrEmpty(password)?"<none>":password)}");
            Console.WriteLine($"Device:\t\t{device}");

            Console.WriteLine("Server Started. Press any key to stop...");
            Console.ReadKey(true);

            if (!_pcrNetworkServer.Stop())
            {
                throw new InvalidOperationException("Stopping the network server failed.");
            }
            Console.ForegroundColor = conCol;
            Console.Title = title;
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork))
            {
                return ip.ToString();
            }
            return "<ERRNOIP>";
        }
    }
}

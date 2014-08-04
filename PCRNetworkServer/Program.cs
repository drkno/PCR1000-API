using System;
using PCR1000;

namespace PCRNetworkServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var conCol = Console.ForegroundColor;
            try
            {
                var server = new PcrNetworkServer();

                #if DEBUG
                server.SetDebugLogger(true);
                #endif
            }
            finally
            {
                Console.ForegroundColor = conCol;
            }


        }
    }
}

using System;
using PCR1000;

namespace PCRNetworkServer
{
    public static class Cli
    {
        private static PcrNetworkServer _pcrNetworkServer;

        private static void PrintHelp(Exception ex = null)
        {
            if (ex != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: The program failed with an error.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(ex.Message);
                #if DEBUG
                Console.ReadKey();
                #endif
                return;
            }

            var name = AppDomain.CurrentDomain.FriendlyName;
            name = name.Substring(0, name.LastIndexOf('.'));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("NAME");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\t" + name);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nSYNOPSIS");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\t" + name + " [OPTION]...");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nDESCRIPTION");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\tStarts a new PCR1000 network server. This allows remote control of\n" +
                              "\tthe receiver.\n");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t-h, --help");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\t\tDisplays this help.\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t-n, --nport");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\t\tNetwork port to use. Defaults to 4456.\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t-s, --sport");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\t\tSerial port to use. Defaults to COM1.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nAUTHOR");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\tWritten by Matthew Knox.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nCOPYRIGHT");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\tCopyright (c) Matthew Knox. Released under MIT license.");

            #if DEBUG
            Console.ReadKey();
            #endif
        }

        public static void Run()
        {
            var conCol = Console.ForegroundColor;
            if (Arguments.GetArgumentBool("help") || Arguments.GetArgumentBool("h"))
            {
                PrintHelp();
                return;
            }

            try
            {
                int nport;
                if (!int.TryParse(Arguments.GetArgument("nport"), out nport))
                {
                    if (!int.TryParse(Arguments.GetArgument("n"), out nport))
                    {
                        nport = 4456;
                    }
                }

                string sport;
                if (string.IsNullOrWhiteSpace(sport = Arguments.GetArgument("sport")))
                {
                    if (string.IsNullOrWhiteSpace(sport = Arguments.GetArgument("s")))
                    {
                        sport = "COM1";
                    }
                }

                _pcrNetworkServer = new PcrNetworkServer(new PcrSerialComm(sport), nport);
                #if DEBUG
                _pcrNetworkServer.SetDebugLogger(true);
                #endif

                if (!_pcrNetworkServer.Start())
                {
                    throw new Exception("Starting the network server failed.");
                }

                Console.WriteLine("Server Started. Press any key to stop...");
                Console.ReadKey(true);

                if (!_pcrNetworkServer.Stop())
                {
                    throw new Exception("Stopping the network server failed.");
                }
            }
            catch (Exception ex)
            {
                PrintHelp(ex);
            }
            Console.ForegroundColor = conCol;
        }
    }
}

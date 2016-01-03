using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PCRNetworkServer
{
    public static class Program
    {
        private static readonly OptionSet OptionSet = new OptionSet();
        private static readonly ConsoleColor DefaultColour = Console.ForegroundColor;

        private static void ShowHelp()
        {
            #if DEBUG
            const bool shouldWait = true;
            #else
            const bool shouldWait = false;
            #endif
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            OptionSet.ShowHelp("Server for remote control of a PCR1000 radio receiver.", "{appName} [OPTION]...",
                       "If no options are specified defaults will be used.",
                       null,
                       "Written by Matthew Knox.",
                       "Version:\t" + fvi.ProductVersion + " " + (Environment.Is64BitProcess ? "x64" : "x32") +
                       "\nCLR Version:\t" + Environment.Version +
                       "\nOS Version:\t" + Environment.OSVersion.VersionString +
                       "\nReport {appName} bugs and above information to the bug tracker at\n" +
                       "<https://github.com/mrkno/PCR1000-API>",
                       "Copyright © " + DateTime.Now.Year + " Matthew Knox.\n"
                       + "The MIT License (MIT) <http://opensource.org/licenses/MIT>\n"
                       + "This is free software: you are free to change and redistribute it.\n"
                       + "There is NO WARRANTY, to the extent permitted by law.",
                       shouldWait
            );
            Console.ForegroundColor = DefaultColour;
            Environment.Exit(0);
        }

        private static void ListDevices()
        {
            var portNames = SerialPort.GetPortNames();
            foreach (var portName in portNames)
            {
                Console.WriteLine(portName);
            }
            #if DEBUG
            Console.ReadKey();
            #endif
            Console.ForegroundColor = DefaultColour;
            Environment.Exit(0);
        }

        [STAThread]
        public static void Main(string[] args)
        {
#if DEBUG
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
#endif
            try
            {
                string ui = "cli", password = "", device = null;
                var security = false;
                var port = 4456;

                OptionSet.Add("h|help", "Displays this help.", s => ShowHelp());
                OptionSet.Add("u|ui", "Determines what {ui} to display (gui/cli). Defaults to cli.", t => ui = t, false);
                OptionSet.Add("s|ssl|tls", "Enable transport layer security on the server. Defaults to disabled.", t => security = true, false);
                OptionSet.Add("p|password", "{Password} to use to authenticate the connection. None by default.", t => password = t, false);
                OptionSet.Add("n|port", "Network {port} to use. Defaults to 4456.", t =>
                {
                    if (!int.TryParse(t, out port))
                    {
                        throw new OptionException("Provided network port was invalid.", "port");
                    }
                }, false);
                OptionSet.Add("d|device", "Serial {device} to use. Defaults to first serial device found.", t =>
                {
                    if (!SerialPort.GetPortNames().Contains(t))
                    {
                        throw new OptionException("Provided serial device was invalid.", "device");
                    }
                    device = t;
                }, false);
                OptionSet.Add("l|list|devices", "List all avalible devices.", t => ListDevices(), false);
                OptionSet.ParseExceptionally(args);

                if (string.IsNullOrWhiteSpace(device))
                {
                    var portNames = SerialPort.GetPortNames();
                    if (portNames.Length == 0)
                    {
                        throw new InvalidOperationException("No serial ports are avalible for use!");
                    }
                    device = portNames[0];
                }

                switch (ui)
                {
                    case "gui":
                        {
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new Gui(security, password, port, device));
                            break;
                        }
                    case "cli":
                        {
                            Cli.Run(security, password, port, device);
                            break;
                        }
                    default:
                        {
                            goto case "cli";
                        }
                }
            }
            catch (Exception e)
            {
                var name = AppDomain.CurrentDomain.FriendlyName;
                if (e is OptionException)
                {
                    if (e.Message.Contains("\n"))
                    {
                        Console.Error.WriteLine(name + ": Multiple Errors Occured");
                        foreach (var err in e.Message.Split('\n'))
                        {
                            Console.Error.WriteLine("-\t" + err);
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine(name + ": " + e.Message);
                    }
                }
                else if (e is InvalidOperationException)
                {
                    Console.Error.WriteLine("A fatal error occured while running " + name +
                        ". " + e.Message);
                }
                else
                {
                    Console.Error.WriteLine("A fatal error occured while running " + name +
                        ". One of your options was probably malformed. ");
#if DEBUG
                    Console.Error.WriteLine(e.StackTrace);
#endif
                }

                Console.Error.WriteLine("Try '" + name + " --help' for more information.");

                #if DEBUG
                Console.ReadKey();
                #endif
                Console.ForegroundColor = DefaultColour;
                Environment.Exit(-1);
            }
        }
    }
}

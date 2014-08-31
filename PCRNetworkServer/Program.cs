using System;
using System.Windows.Forms;

namespace PCRNetworkServer
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Arguments.LoadArguments(args);

            switch (Arguments.GetArgument("ui"))
            {
                case "gui":
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Gui());
                    break;
                }
                case "cli":
                {
                    Cli.Run();
                    break;
                }
                default:
                {
                    goto case "cli";
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCR1000;
using PCR1000.Network;

namespace PCR1000LibTest
{
    public partial class Control : Form
    {
        public Control()
        {
            InitializeComponent();
        }

        readonly PcrControl _pcrControl = new PcrControl(new PcrNetworkClient("192.168.1.9"));

        private void button1_Click(object sender, EventArgs e)
        {
#if DEBUG
            _pcrControl.SetComDebugLogging(true);
#endif
            Console.WriteLine(_pcrControl.PcrPowerUp());
            Console.WriteLine(_pcrControl.PcrIsOn());
            _pcrControl.PcrSetSquelch(0);
            Console.WriteLine(_pcrControl.PcrIsOn());
            _pcrControl.PcrSetVolume(50);
            Console.WriteLine(_pcrControl.PcrIsOn());
            _pcrControl.PcrSetFreq(99800000);
            Console.WriteLine(_pcrControl.PcrIsOn());
            _pcrControl.PcrSetFilter(230);
            Console.WriteLine(_pcrControl.PcrIsOn());
            _pcrControl.PcrSetMode("WFM");
            Console.WriteLine(_pcrControl.PcrIsOn());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _pcrControl.PcrPowerDown();
        }
    }
}

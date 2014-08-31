using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using PCR1000;

namespace PCRNetworkServer
{
    public partial class Gui : Form
    {
        private PcrNetworkServer _pcrNetworkServer;

        public Gui()
        {
            InitializeComponent();

            var ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                comboBoxSerialPort.Items.Add(port);
            }
            comboBoxSerialPort.SelectedValue = Arguments.GetArgument("sport");

            int n;
            if (int.TryParse(Arguments.GetArgument("nport"), out n))
            {
                textBoxNetwork.Text = Arguments.GetArgument("nport");
            }
        }

        private bool CheckSettings()
        {
            int port;
            if (int.TryParse(textBoxNetwork.Text, out port))
            {
                return false;
            }

            if (comboBoxSerialPort.SelectedIndex == -1)
            {
                return false;
            }

            return true;
        }

        private bool _isOn;
        private void ButtonOnOffClick(object sender, EventArgs e)
        {
            switch (_isOn)
            {
                case true:
                {
                    try
                    {
                        _pcrNetworkServer.Stop();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured while stopping the server:\n" + ex.Message);
                    }

                    buttonOnOff.Text = "OFF";
                    buttonOnOff.BackColor = Color.FromArgb(255, 128, 128);
                    textBoxNetwork.Enabled = true;
                    comboBoxSerialPort.Enabled = true;
                    _isOn = false;
                    break;
                }
                case false:
                {
                    if (!CheckSettings())
                    {
                        MessageBox.Show("Invalid Settings Provided.", "Start Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    try
                    {
                        var port = int.Parse(textBoxNetwork.Text);
                        var comp = comboBoxSerialPort.SelectedItem.ToString();
                        IComm comm = new PcrSerialComm(comp);
                        _pcrNetworkServer = new PcrNetworkServer(comm, port);
                        #if DEBUG
                        _pcrNetworkServer.SetDebugLogger(true);
                        #endif
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured while starting the server:\n" + ex.Message, "Start Server",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    buttonOnOff.Text = "ON";
                    buttonOnOff.BackColor = Color.FromArgb(192, 255, 192);
                    textBoxNetwork.Enabled = false;
                    comboBoxSerialPort.Enabled = false;
                    _isOn = true;
                    break;
                }
            }
        }
    }
}

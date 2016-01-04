using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using PCR1000;
using PCR1000.Network.Server;

namespace PCRNetworkServer
{
    public partial class Gui : Form
    {
        private PcrNetworkServer _pcrNetworkServer;

        public Gui(bool security, string password, int port, string device)
        {
            InitializeComponent();

            var ports = SerialPort.GetPortNames();
            foreach (var dev in ports)
            {
                comboBoxSerialPort.Items.Add(dev);
            }

            comboBoxSerialPort.SelectedIndex = comboBoxSerialPort.Items.IndexOf(device);
            textBoxNetwork.Text = port.ToString();
            textBoxPassword.Text = password;
            checkBoxSsl.Checked = security;

            if (checkBoxSsl.Checked || !string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                checkBoxUseSecurity.Checked = true;
            }
        }

        private bool CheckSettings()
        {
            int port;
            if (!int.TryParse(textBoxNetwork.Text, out port) || port <= 0)
            {
                return false;
            }

            return comboBoxSerialPort.SelectedIndex != -1;
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
                        if (!_pcrNetworkServer.Stop())
                        {
                            throw new Exception("A fatal error occured while stopping the server. Please check debug messages for details.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured while stopping the server:\n" + ex.Message);
                    }

                    buttonOnOff.Text = "OFF";
                    buttonOnOff.BackColor = Color.FromArgb(255, 128, 128);
                    textBoxNetwork.Enabled = true;
                    comboBoxSerialPort.Enabled = true;
                    checkBoxUseSecurity.Enabled = true;
                    checkBoxSsl.Enabled = checkBoxUseSecurity.Checked;
                    textBoxPassword.Enabled = checkBoxUseSecurity.Checked;
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
                        _pcrNetworkServer = checkBoxUseSecurity.Checked ? 
                            new PcrNetworkServer(comm, port, checkBoxSsl.Checked, textBoxPassword.Text) : 
                            new PcrNetworkServer(comm, port);

                        if (!_pcrNetworkServer.Start())
                        {
                            throw new Exception("Server startup failed. Review debug messages for details.");
                        }
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
                    checkBoxUseSecurity.Enabled = false;
                    checkBoxSsl.Enabled = false;
                    textBoxPassword.Enabled = false;
                    _isOn = true;
                    break;
                }
            }
        }

        private void CheckBoxUseSecurityCheckedChanged(object sender, EventArgs e)
        {
            textBoxPassword.Enabled = checkBoxUseSecurity.Checked;
            checkBoxSsl.Enabled = checkBoxUseSecurity.Checked;
        }

        private void GuiFormClosing(object sender, FormClosingEventArgs e)
        {
            if (buttonOnOff.Text != "ON") return;
            e.Cancel = true;
            MessageBox.Show("Please turn off the server before quitting.", "Quit", 
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}

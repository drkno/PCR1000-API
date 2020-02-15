using System;
using System.IO.Ports;
using System.Text;
#pragma warning disable 1591

namespace PCR1000.SerialPorts
{
    public abstract class PcrSerialPort : IDisposable
    {
        public event EventHandler<SerialDataReceivedEventArgs> DataReceived;
        protected readonly SerialPort SerialPort;

        protected PcrSerialPort(string port, int baud, Parity parity, int dataBits, StopBits stopBits)
        {
            SerialPort = new SerialPort(port, baud, parity, dataBits, stopBits);
            SerialPort.DataReceived += SerialPortDataReceived;
        }

        protected void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }

        public void Close()
        {
            SerialPort.Close();
        }

        public abstract void Open();

        public void Write(string cmd)
        {
            SerialPort.Write(cmd);
        }

        public int Read(byte[] buff, int offset, int count)
        {
            return SerialPort.Read(buff, offset, count);
        }

        public void DiscardInBuffer()
        {
            SerialPort.DiscardInBuffer();
        }

        public bool DtrEnable
        {
            get { return SerialPort.DtrEnable; }
            set { SerialPort.DtrEnable = value; }
        }
        public Handshake Handshake
        {
            get { return SerialPort.Handshake; }
            set { SerialPort.Handshake = value; }
        }

        public bool IsOpen => SerialPort.IsOpen;
        public string PortName => SerialPort.PortName;
        public int ReadBufferSize => SerialPort.ReadBufferSize;
        public Encoding Encoding => SerialPort.Encoding;

        public abstract void Dispose();
    }
}

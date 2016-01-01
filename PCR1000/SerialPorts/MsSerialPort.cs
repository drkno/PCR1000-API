using System.IO.Ports;
#pragma warning disable 1591

namespace PCR1000.SerialPorts
{
    public class MsSerialPort : PcrSerialPort
    {
        public MsSerialPort(string port, int baud, Parity parity, int dataBits, StopBits stopBits)
            : base(port, baud, parity, dataBits, stopBits)
        {
        }

        public override void Open()
        {
            SerialPort.Open();
        }

        public override void Dispose()
        {
            SerialPort.Dispose();
        }
    }
}

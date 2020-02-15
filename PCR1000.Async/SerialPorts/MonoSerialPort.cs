using System;
using System.Diagnostics;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
// ReSharper disable PossibleNullReferenceException
#pragma warning disable 1591

namespace PCR1000.SerialPorts
{
    public class MonoSerialPort : PcrSerialPort
    {
        private int _fd;
        private FieldInfo _disposedFieldInfo;
        private Thread _listenThread;

        public MonoSerialPort(string port, int baud, Parity parity, int dataBits, StopBits stopBits)
            : base(port, baud, parity, dataBits, stopBits)
        {
            Debug.WriteLine("MonoSerialPort -> _cstor");
            SerialPort.ReadTimeout = 100;
        }

        public override void Dispose()
        {
            Debug.WriteLine("MonoSerialPort -> Dispose");
            if (_listenThread != null && _listenThread.IsAlive)
            {
                _listenThread.Abort();
                _listenThread.Join();
            }
            SerialPort.Dispose();
        }

        public override void Open()
        {
            Debug.WriteLine("MonoSerialPort -> Open");
            Debug.Assert(SerialPort != null, "Somehow the serial port was never setup correctly.");
            SerialPort.Open();
            var fieldInfo = SerialPort.BaseStream.GetType().GetField("fd", BindingFlags.Instance | BindingFlags.NonPublic);
            _fd = (int) fieldInfo.GetValue(SerialPort.BaseStream);
            _disposedFieldInfo = SerialPort.BaseStream.GetType().GetField("disposed", BindingFlags.Instance | BindingFlags.NonPublic);
            _listenThread = new Thread(EventThreadFunction) {IsBackground = true};
            _listenThread.Start();
        }

        private void EventThreadFunction()
        {
            do
            {
                try
                {
                    var stream = SerialPort.BaseStream;
                    if (stream == null)
                    {
                        return;
                    }

                    if (Poll(stream))
                    {
                        SerialPortDataReceived(SerialPort, null);
                    }
                }
                catch
                {
                    return;
                }
            }
            while (IsOpen);
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        private static extern bool poll_serial(int fd, out int error, int timeout);

        private bool Poll(Stream stream)
        {
            CheckDisposed(stream);
            if (IsOpen == false)
            {
                throw new Exception("The serial port is closed.");
            }
            int error;

            var pollResult = poll_serial(_fd, out error, SerialPort.ReadTimeout);
            if (error == -1)
            {
                ThrowIoException();
            }
            return pollResult;
        }

        [DllImport("libc")]
        private static extern IntPtr strerror(int errnum);

        private static void ThrowIoException()
        {
            Debug.WriteLine("MonoSerialPort -> ThrowIoException");
            var errnum = Marshal.GetLastWin32Error();
            var errorMessage = Marshal.PtrToStringAnsi(strerror(errnum));
            throw new IOException(errorMessage);
        }

        private void CheckDisposed(Stream stream)
        {
            if ((bool)_disposedFieldInfo.GetValue(stream))
            {
                throw new ObjectDisposedException(stream.GetType().FullName);
            }
        }
    }
}

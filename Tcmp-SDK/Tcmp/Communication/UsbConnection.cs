using System.Collections.Generic;
using System.IO.Ports;
using System.Diagnostics;
using TapTrack.Tcmp.Communication.Exceptions;
using System;

namespace TapTrack.Tcmp.Communication
{
    internal class UsbConnection : Connection
    {
        private SerialPort port;

        public UsbConnection()
        {
            port = new SerialPort();

            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            port.BaudRate = 115200;
            port.RtsEnable = false;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            OnDataReceived(e);
        }

        public override bool Connect(string portName)
        {
            try
            {
                Disconnect();
                port.PortName = portName;
                port.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void Disconnect()
        {
            if (port.IsOpen)
                port.Close();
        }

        public override bool IsOpen()
        {
            return port.IsOpen;
        }

        public override string[] GetAvailableDevices()
        {
            return SerialPort.GetPortNames();
        }

		public override string[] GetAvailableDevices(int timeout)
		{
			return SerialPort.GetPortNames();
		}

		public override string[] GetAvailableDevices(int timeout, bool scanForBlueGiga)
		{
			return SerialPort.GetPortNames();
		}

		public override void Send(byte[] data)
        {
            Debug.Write("   sending: ");
            Debug.Write(BitConverter.ToString(data));
            Debug.WriteLine("");

            try
            {
                port.Write(data, 0, data.Length);
            }
            catch (InvalidOperationException e)
            {
                throw new HardwareException("There is no TappyUSB connected");
            }
        }

        public override int Read(List<byte> buffer)
        {
            byte temp;
            int count = 0;

            Debug.Write("   received: ");

            while (port.BytesToRead > 0)
            {
                temp = (byte)port.ReadByte();
                buffer.Add(temp);
                count++;
                Debug.Write(string.Format("{0:X}", temp).PadLeft(2, '0') + " ");
            }

            Debug.WriteLine("");

            return count;
        }

		public override bool getConnectionStatus()
		{
			throw new System.NotImplementedException();

		}

		public override bool getBlueGigaStatus()
		{
			throw new System.NotImplementedException();

		}


		public override void setDisconnectCallback(Bluegiga.BLE.Events.Connection.DisconnectedEventHandler disconnectCallback)
		{
			throw new System.NotImplementedException();
		}

		public override void DisconnectBlueGiga()
		{
			throw new NotImplementedException();
		}

		public override void Flush()
        {
            if (port.IsOpen)
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
            }
        }

        public override void Dispose()
        {
            port.Dispose();
        }
    }
}

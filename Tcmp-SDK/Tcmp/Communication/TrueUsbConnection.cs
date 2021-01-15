using Bluegiga.BLE.Events.Connection;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Collections.Generic;
using System.Linq;

namespace TapTrack.Tcmp.Communication
{
    internal class TrueUsbConnection : Connection
    {
        private UsbDevice device;
        private UsbEndpointReader deviceReader;
        private UsbEndpointWriter deviceWriter;

        private List<byte> buffer;

        public TrueUsbConnection()
        {
            buffer = new List<byte>();
        }

        public override bool Connect(string deviceName)
        {
            // Search for a registry with this deviceName
            UsbRegDeviceList devices = UsbDevice.AllWinUsbDevices;
            UsbRegistry registry = devices.Find(reg => reg.FullName == deviceName);

            // Failed to connect - device not found
            if (registry is null) return false;

            // Keep this device
            device = registry.Device;
            if (device is null) return false;

            // Set up endpoints
            deviceWriter = device.OpenEndpointWriter(WriteEndpointID.Ep01);
            deviceReader = device.OpenEndpointReader(ReadEndpointID.Ep01);

            // Failed to set up endpoints
            if (deviceWriter is null) return false;
            if (deviceReader is null) return false;

            // Add event handler on data received
            deviceReader.DataReceived += DeviceReader_DataReceived;
            deviceReader.DataReceivedEnabled = true;

            return true;
        }

        private void DeviceReader_DataReceived(object sender, EndpointDataEventArgs e)
        {
            // Store event buffer in local buffer
            buffer.AddRange(e.Buffer);

            // Pass event along to OnDataReceived
            OnDataReceived(e);
        }

        public override void Disconnect()
        {
            device?.Close();
        }

        public override void DisconnectBlueGiga()
        {
            return;
        }

        public override void Dispose()
        {
            // TODO: Do we need to call any other methods here?
            if (!(deviceReader is null))
            {
                // Remove event handler
                deviceReader.DataReceived -= DeviceReader_DataReceived;
                deviceReader.DataReceivedEnabled = false;
                deviceReader.Dispose();
            }
            deviceWriter?.Dispose();
            device?.Close();
        }

        public override void Flush()
        {
            // TODO: Do we need to call any other methods here?
            deviceReader?.Flush();
            deviceWriter?.Flush();
        }

        public override string[] GetAvailableDevices()
        {
            // Get UsbRegDeviceList
            UsbRegDeviceList devices = UsbDevice.AllWinUsbDevices;

            // Select FullName only
            return devices.Select(reg => reg.FullName).ToArray();
        }

        public override string[] GetAvailableDevices(int timeout)
        {
            return GetAvailableDevices();
        }

        public override string[] GetAvailableDevices(int timeout, bool scanForBlueGiga)
        {
            return GetAvailableDevices();
        }

        public override bool getBlueGigaStatus()
        {
            return false;
        }

        public override bool getConnectionStatus()
        {
            return IsOpen();
        }

        public override bool IsOpen()
        {
            return device?.IsOpen ?? false;
        }

        public override int Read(List<byte> data)
        {
            int count = buffer.Count;
            data.AddRange(buffer);
            buffer.Clear();

            return count;
        }

        public override void Send(byte[] data)
        {
            deviceWriter.Write(data, 2000, out int count);
        }

        public override void setDisconnectCallback(DisconnectedEventHandler disconnectCallback)
        {
            return;
        }
    }
}

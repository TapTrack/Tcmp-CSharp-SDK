using System;
using System.Collections.Generic;
using System.Management;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using Bluegiga.BLE.Events.GAP;
using Bluegiga.BLE.Events.ATTClient;
using Bluegiga.BLE.Responses.GAP;
using Bluegiga.BLE.Events.Connection;
using Bluegiga.BLE.Responses.ATTClient;

namespace TapTrack.Tcmp.Communication.Bluetooth
{
    internal class BluetoothConnection : Connection
    {
        private enum Handle : ushort
        {
            DeviceName = 4,
            Write = 21,
            Read = 24
        }

        private Bluegiga.BGLib bluetooth;
        private SerialPort port;
        private List<byte> tappyBuffer;
        private BluetoothDevice device;
        private List<BluetoothDevice> discoveredDevices;

        private AutoResetEvent connectionRespEvent;


        public BluetoothConnection()
        {
            string portName = GetBluegigaDevice();

            if (portName == null)
                throw new InvalidOperationException("There is no BLED112 - Bluegiga Bluetooth Low Energy connected to this computer");

            Debug.WriteLine($"Connecting to BLED112 on port {portName}");

            port = new SerialPort(portName);
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            port.Handshake = Handshake.RequestToSend;
            port.BaudRate = 115200;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;

            port.Open();
            port.DiscardInBuffer();
            port.DiscardOutBuffer();

            connectionRespEvent = new AutoResetEvent(false);

            bluetooth = new Bluegiga.BGLib();

            discoveredDevices = new List<BluetoothDevice>();
            tappyBuffer = new List<byte>();

            // Bluegiga Events

            bluetooth.BLEEventATTClientAttributeValue += DataReceivedFromTappy;

            // Bluegiga Responses //

            bluetooth.BLEResponseGAPConnectDirect += ConnectResponse;

            try
            {
                bluetooth.SendCommand(port, bluetooth.BLECommandSystemReset(0));
            }
            finally
            {
                Thread.Sleep(1250);
                port.Close();
                port.Open();
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] inData = new byte[sp.BytesToRead];

            sp.Read(inData, 0, sp.BytesToRead);

            Debug.WriteLine("<= RX ({0}) [ {1}]", inData.Length, BitConverter.ToString(inData));

            for (int i = 0; i < inData.Length; i++)
                bluetooth.Parse(inData[i]);
        }

        public void DataReceivedFromTappy(object sender, AttributeValueEventArgs e)
        {
            if (e.value == null)
                return;

            tappyBuffer.AddRange(e.value);
            OnDataReceived(e);
        }

        private string GetBluegigaDevice()
        {
            ManagementObjectCollection comPortDevices;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From WIN32_SerialPort");
            comPortDevices = searcher.Get();

            foreach (ManagementObject device in comPortDevices)
            {
                string name = device["Name"] as string;

                if (name?.Contains("Bluegiga Bluetooth Low Energy") ?? false)
                {
                    Debug.WriteLine($"Found {device["Name"]}");
                    Match match = Regex.Match(name, @"\(([^)]*)\)");
                    if (match.Groups.Count > 1)
                        return match.Groups[1].Value;
                }
            }

            return null;
        }

        public override bool Connect(string deviceName)
        {
            bool commandStarted = false;
            bool connectionEst = false;
            byte connectionHandle = 0;
            AutoResetEvent resp = new AutoResetEvent(false);

            ConnectDirectEventHandler connectResp = delegate (object sender, ConnectDirectEventArgs e)
            {
                if (e.result == 0)
                {
                    commandStarted = true;
                    connectionHandle = e.connection_handle;
                    resp.Set();
                }
            };

            StatusEventHandler connectionStatus = delegate (object sender, StatusEventArgs e)
            {
                connectionEst = true;
                resp.Set();
            };

            bluetooth.BLEResponseGAPConnectDirect += connectResp;
            bluetooth.BLEEventConnectionStatus += connectionStatus;

            foreach (BluetoothDevice device in discoveredDevices)
            {
                if (device.Name.Equals(deviceName))
                {
                    this.device = device;
                    this.device.ConnectionHandle = connectionHandle;
                    bluetooth.SendCommand(this.port, bluetooth.BLECommandGAPConnectDirect(device.BluetoothAddress, 0, 10, 200, 1000, 10));
                    resp.WaitOne(200);
                    if (commandStarted)
                    {
                        resp.WaitOne(1000);
                    }
                    break;
                }
            }

            bluetooth.BLEResponseGAPConnectDirect -= connectResp;
            bluetooth.BLEEventConnectionStatus -= connectionStatus;

            return connectionEst;
        }

        private void ConnectResponse(object sender, ConnectDirectEventArgs e)
        {
            this.device = new BluetoothDevice();
            device.ConnectionHandle = e.connection_handle;
        }

        public override void Disconnect()
        {
            if (device?.ConnectionHandle != null)
                bluetooth.SendCommand(port, bluetooth.BLECommandConnectionDisconnect((byte)device.ConnectionHandle));
        }

        public override string[] GetAvailableDevices()
        {
            return GetAvailableDevices(1000);
        }

        public string[] GetAvailableDevices(int scanTime)
        {
            List<string> names = new List<string>();
            bool commandStarted = false;
            AutoResetEvent resp = new AutoResetEvent(false);
            discoveredDevices.Clear();

            DiscoverEventHandler discoverResp = delegate (object sender, DiscoverEventArgs e)
            {
                if (e.result == 0)
                    commandStarted = true;
                resp.Set();
            };

            ScanResponseEventHandler deviceFound = delegate (object sender, ScanResponseEventArgs e)
            {
                if (e.data.Length < 8)
                    return;

                string name = Encoding.UTF8.GetString(e.data, e.data.Length - 8, 8);

                if (!name.Contains("TAPPY"))
                    return;

                foreach (BluetoothDevice device in discoveredDevices)
                {
                    if (device.Name.Equals(name))
                        return;
                }

                discoveredDevices.Add(new BluetoothDevice(e.sender, e.bond, e.rssi, name));
            };

            bluetooth.BLEResponseGAPDiscover += discoverResp;
            bluetooth.BLEEventGAPScanResponse += deviceFound;

            bluetooth.SendCommand(port, bluetooth.BLECommandGAPDiscover(2));

            resp.WaitOne(750);
            bluetooth.BLEResponseGAPDiscover -= discoverResp;

            if (!commandStarted)
                return names.ToArray();

            Thread.Sleep(scanTime);
            bluetooth.SendCommand(port, bluetooth.BLECommandGAPEndProcedure());

            bluetooth.BLEEventGAPScanResponse -= deviceFound;

            foreach (BluetoothDevice device in this.discoveredDevices)
                names.Add(device.Name);

            return names.ToArray();
        }

        public override bool IsOpen()
        {
            if (device == null)
                return false;
            if (device.ConnectionHandle == null)
                return false;
            return true;
        }

        public override int Read(List<byte> data)
        {
            int count = this.tappyBuffer.Count;
            Debug.WriteLine($"   received: {BitConverter.ToString(this.tappyBuffer.ToArray())}");
            data.AddRange(this.tappyBuffer);
            this.tappyBuffer.Clear();

            return count;
        }

        public override void Send(byte[] data)
        {
            if (device?.ConnectionHandle != null)
            {
                AutoResetEvent proceed = new AutoResetEvent(false);
                ProcedureCompletedEventHandler complete = delegate (object sender, ProcedureCompletedEventArgs e)
                {
                    Debug.WriteLine("Bluetooth: write complete");
                    proceed.Set();
                };

                bluetooth.BLEEventATTClientProcedureCompleted += complete;

                if (data.Length > 20)
                {
                    
                    List<byte> payload = new List<byte>(data);
                    int total = data.Length;
                    bool signal;

                    for (int i = 0; i < Math.Ceiling((double)data.Length / 20); i++)
                    {
                        bluetooth.SendCommand(this.port,
                            bluetooth.BLECommandATTClientAttributeWrite((byte)device.ConnectionHandle, (ushort)Handle.Write,
                            payload.GetRange(i * 20, (total < 20) ? total : 20).ToArray())
                        );
                        total -= 20;
                        signal = proceed.WaitOne(1000);
                        if (!signal)
                            break;
                    }
                }
                else
                {
                    bluetooth.SendCommand(this.port, bluetooth.BLECommandATTClientAttributeWrite((byte)device.ConnectionHandle, (ushort)Handle.Write, data));
                    proceed.WaitOne(1000);
                }

                bluetooth.BLEEventATTClientProcedureCompleted -= complete;
            }

        }
    }
}

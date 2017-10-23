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
using TapTrack.Tcmp.Communication.Exceptions;

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
        private bool isConnected;
        private object bufferLock;
		private Bluegiga.BLE.Events.Connection.DisconnectedEventHandler disconnectCallback;

        public BluetoothConnection()
        {
			port = null; //if a new SerialPort is instantiated here for some reason the portName is always "COM1" even if that's not a valid port on the PC so we make this nullable instead.	
            bluetooth = new Bluegiga.BGLib();

            discoveredDevices = new List<BluetoothDevice>();
            tappyBuffer = new List<byte>();
            isConnected = false;

            bufferLock = new object();

            // Bluegiga Events

            bluetooth.BLEEventATTClientAttributeValue += DataReceivedFromTappy;
            bluetooth.BLEEventConnectionDisconnected += Bluetooth_BLEEventConnectionDisconnected;

		}

		public BluetoothConnection(Bluegiga.BLE.Events.Connection.DisconnectedEventHandler callback)
		{
			port = null; //if a new SerialPort is instantiated here for some reason the portName is always "COM1" even if that's not a valid port on the PC so we make this nullable instead.	
			bluetooth = new Bluegiga.BGLib();

			discoveredDevices = new List<BluetoothDevice>();
			tappyBuffer = new List<byte>();
			isConnected = false;

			bufferLock = new object();

			// Bluegiga Events

			bluetooth.BLEEventATTClientAttributeValue += DataReceivedFromTappy;
			bluetooth.BLEEventConnectionDisconnected += Bluetooth_BLEEventConnectionDisconnected;
			bluetooth.BLEEventConnectionDisconnected += callback;
		   disconnectCallback += callback;

		}

		private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (!port.IsOpen)
                return;

            byte[] inData = new byte[port.BytesToRead];

            port.Read(inData, 0, inData.Length);

            Debug.WriteLine("<= RX ({0}) [ {1}]", inData.Length, BitConverter.ToString(inData));

            for (int i = 0; i < inData.Length; i++)
                bluetooth.Parse(inData[i]);
        }

        public void DataReceivedFromTappy(object sender, AttributeValueEventArgs e)
        {
            if (e.value == null)
                return;

            lock (bufferLock)
                tappyBuffer.AddRange(e.value);

            OnDataReceived(e);
        }

        private void Bluetooth_BLEEventConnectionDisconnected(object sender, DisconnectedEventArgs e)
        {
            isConnected = false;
            Debug.WriteLine($"Disconnected: {e.reason}");
		    if(disconnectCallback != null)
				disconnectCallback(sender, e);

		}

		private void nullDisconnectEventHandler(object sender, DisconnectedEventArgs e)
		{
		}



		private string Search(string searchLocation)
        {
            ManagementObjectCollection comPortDevices;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher($"Select * From {searchLocation}");
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

        private string GetBluegigaDevice()
        {
            return Search("Win32_SerialPort") ?? Search("Win32_pnpEntity");
        }

        public bool ConnectToBluegiga()
        {
			if (getBlueGigaStatus())
				return true;

			string portName = GetBluegigaDevice();

			if (portName == null)
				return false;

			try
			{
				if (port == null) // no port assigned yet
				{
					port = new SerialPort();
					port.PortName = portName;
					port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
					port.Handshake = Handshake.RequestToSend;
					port.BaudRate = 115200;
					port.DataBits = 8;
					port.StopBits = StopBits.One;
					port.Parity = Parity.None;
					port.Open();
					return true;
				}
				else if (port.IsOpen)
				{
					return true;  //this method should not be switching ports, probably best to creat a separate method for that. 
				}
				else //port is assigned but closed
				{
					port.Open();
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}

		}

        public override bool Connect(string deviceName)
        {
           if (!port.IsOpen)
            {
                if (!ConnectToBluegiga())
                    return false;
            }

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
                    bluetooth.SendCommand(this.port, bluetooth.BLECommandGAPConnectDirect(device.BluetoothAddress, 0, 20, 40, 100, 1));
                    resp.WaitOne(200);
                    if (commandStarted)
                    {
                        this.device.ConnectionHandle = connectionHandle;
                        resp.WaitOne(1000);
                    }
                    break;
                }
            }

            bluetooth.BLEResponseGAPConnectDirect -= connectResp;
            bluetooth.BLEEventConnectionStatus -= connectionStatus;

            if (connectionEst)
                isConnected = true;

            return connectionEst;
        }

		public override void DisconnectBlueGiga()
		{
			try
			{
				port?.Close();
				port = null;
			}
			catch (Exception e)
			{
				return;
			}
		}

		public override void Disconnect()
        {
			if (device?.ConnectionHandle != null && (port?.IsOpen) != null)
			{
				bluetooth.SendCommand(port, bluetooth.BLECommandConnectionDisconnect((byte)device.ConnectionHandle));
				DisconnectBlueGiga();
			}
		}

		public override string[] GetAvailableDevices()
		{
			return GetAvailableDevices(3000);
		}

		public override string[] GetAvailableDevices(int scanTime)
        {

			return GetAvailableDevices(scanTime, true);

		}

		public override string[] GetAvailableDevices(int scanTime, bool detectBlueGiga)
		{
			List<string> names = new List<string>();
			bool commandStarted = false;
			AutoResetEvent resp = new AutoResetEvent(false);
			discoveredDevices.Clear();

			if (detectBlueGiga)
			{
				if (/*port == null ||*/ !getBlueGigaStatus()) //only try to find a BlueGiga dongle if one is not already assigned. 
				{
					if (!ConnectToBluegiga())
						return names.ToArray();
				}
			}
	

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

			bluetooth.SendCommand(port, bluetooth.BLECommandGAPDiscover(2));

			resp.WaitOne(200);

			bluetooth.BLEEventGAPScanResponse += deviceFound;
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
            if (!port.IsOpen)
                return false;
            if (device?.ConnectionHandle == null)
                return false;
            return isConnected;
        }

        public override int Read(List<byte> data)
        {
            lock (bufferLock)
            {
                int count = this.tappyBuffer.Count;
                Debug.WriteLine($"   received: {BitConverter.ToString(this.tappyBuffer.ToArray())}");
                data.AddRange(this.tappyBuffer);
                this.tappyBuffer.Clear();

                return count;
            }
        }

        public override void Send(byte[] data)
        {
            try
            {
                if (device?.ConnectionHandle != null && isConnected)
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
                            signal = proceed.WaitOne(200);
                            if (!signal)
                                break;
                        }
                    }
                    else
                    {
                        bluetooth.SendCommand(this.port, bluetooth.BLECommandATTClientAttributeWrite((byte)device.ConnectionHandle, (ushort)Handle.Write, data));
                        proceed.WaitOne(200);
                    }

                    bluetooth.BLEEventATTClientProcedureCompleted -= complete;
                }
                else
                {
                    throw new HardwareException("There is no reader connected");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new HardwareException("There is no TappyUSB connected");
            }
        }

		public override bool getConnectionStatus()
		{
			return isConnected;
		}

		public override  bool getBlueGigaStatus()
		{
			if (port == null)
				return false;
			else
				return true;										

		}

		public override void setDisconnectCallback(Bluegiga.BLE.Events.Connection.DisconnectedEventHandler callback)
		{
			disconnectCallback += callback;	
		}

        public override void Flush()
        {
            if (port?.IsOpen != null)
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
            }
            tappyBuffer.Clear();
        }

        public override void Dispose()
        {
            port.Dispose();
        }
    }
}

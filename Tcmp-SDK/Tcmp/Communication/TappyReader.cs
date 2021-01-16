using System;
using System.Collections.Generic;
using System.Diagnostics;
using TapTrack.Tcmp.Communication.Exceptions;
using System.Threading;
using TapTrack.Tcmp.Communication.Bluetooth;
using TapTrack.Tcmp.CommandFamilies;
using TapTrack.Tcmp.CommandFamilies.BasicNfc;
using TapTrack.Tcmp.CommandFamilies.System;
using System.Threading.Tasks;
using Bluegiga.BLE.Events.Connection;

namespace TapTrack.Tcmp.Communication
{
    public delegate void Callback(ResponseFrame frame, Exception e);

    /// <summary>
    /// The Driver class is used to communicate(send commands and receive data) with a Tappy device
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///     Reading a UID from a NFC tag
    /// </para>
    /// <code language="cs">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Threading.Tasks;
    /// 
    /// using TapTrack.Tcmp.CommandFamilies;
    /// using TapTrack.Tcmp.Communication;
    /// using TapTrack.Tcmp.CommandFamilies.BasicNfc;
    /// using TapTrack.Tcmp;
    /// 
    /// namespace TapTrack.Tcmp.Demo
    /// {
    ///     class Program
    ///     {
    ///         static void Main(string[] args)
    ///         {
    ///             TappyReader tappyReader = new TappyReader(CommunicationProtocol.Usb);
    /// 
    ///             if (tappyReader.AutoDetect())                               // Find and connect to a Tappy
    ///             {
    ///                 Command readUid = new DetectSingleTagUid(0, DetectTagSetting.Type2Type4AandMifare); // No time out and no locking detect uid command    
    /// 
    ///                 Callback responseCallback = (ResponseFrame frame, Exception e) =>
    ///                 {
    ///                     if(TcmpFrame.IsValidFrame(frame) &amp;&amp; frame.ResponseCode == 0x01)
    ///                     {
    ///                         Tag tag = new Tag(frame.Data);
    /// 
    ///                         string uid = BitConverter.ToString(tag.UID);
    /// 
    ///                         Console.WriteLine($"UID: {uid}");
    ///                     }
    ///                 };
    /// 
    ///                 Console.WriteLine("Waiting for a tag");
    ///                 tappyReader.SendCommand(readUid, responseCallback);
    ///             }
    ///             else
    ///             {
    ///                 Console.WriteLine("No Tappy found");
    ///             }
    /// 
    ///             Console.ReadKey();
    ///         }
    ///     }
    /// }
    /// </code>
    /// <para>
    ///     Writing text to a tag
    /// </para>
    /// <code language='cs'>
    /// using System;
    /// 
    /// using TapTrack.Tcmp.CommandFamilies;
    /// using TapTrack.Tcmp.Communication;
    /// using TapTrack.Tcmp.CommandFamilies.BasicNfc;
    /// 
    /// namespace TapTrack.Tcmp.Demo
    /// {
    ///     class Program
    ///     {
    ///         static void Main(string[] args)
    ///         {
    ///             TappyReader tappyReader = new TappyReader(CommunicationProtocol.Usb);
    /// 
    ///             if (tappyReader.AutoDetect())                               // Find and connect to a Tappy
    ///             {
    ///                 Command cmd = new WriteText(0, false, "Hello world!");  // No time out and no locking write command
    /// 
    ///                 Callback responseCallback = (ResponseFrame frame, Exception e) =>
    ///                 {
    ///                     if (TcmpFrame.IsValidFrame(frame) &amp;&amp; frame.ResponseCode == 0x05)
    ///                         Console.WriteLine("Write successful");
    ///                     else
    ///                         Console.WriteLine("Write was unsuccessful");
    ///                 };
    /// 
    ///                 Console.WriteLine("Waiting for a tag");
    ///                 tappyReader.SendCommand(cmd, responseCallback);
    ///             }
    ///             else
    ///             {
    ///                 Console.WriteLine("No Tappy found");
    ///             }
    /// 
    ///             Console.ReadKey();
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public class TappyReader : IDisposable
    {
        private Connection conn;
        private Callback responseCallback;

        private List<byte> buffer = new List<byte>();
        private CommunicationProtocol currentProtocol;

        public CommunicationProtocol Protocol 
        {
            get
            {
                return currentProtocol;
            }
            set
            {
                SwitchProtocol(value);
            }
        }

        /// <summary>
        /// Gets the name of the device the driver is currently connected to 
        /// (USB port name or bluetooth device name depending on the current 
        /// communcation protocol).
        /// Returns null if there is no device connected.
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocol">Which protocol the driver should communicate over</param>
        /// <param name="disconnectCallback"></param>
        public TappyReader(CommunicationProtocol protocol, DisconnectedEventHandler disconnectCallback = null)
        {
            Protocol = protocol;
            if (!(disconnectCallback is null))
            {
                conn.setDisconnectCallback(disconnectCallback);
            }
        }

        private void InitializeConnection(DisconnectedEventHandler disconnectCallback = null)
        {
            switch (currentProtocol)
            {
            case CommunicationProtocol.Usb:
                {
                    conn = new UsbConnection();
                    break;
                }
            case CommunicationProtocol.TrueUsb:
                {
                    conn = new TrueUsbConnection();
                    break;
                }
            case CommunicationProtocol.Bluetooth:
                {
                    conn = new BluetoothConnection(disconnectCallback);
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

		private void DataReceivedHandler(object sender, EventArgs e)
        {
            Debug.WriteLine("Data is being recieve");
			if (!conn.IsOpen() && responseCallback != null)
				responseCallback(null, new HardwareException("Connection to device is not open"));

            Debug.WriteLine($"     Before: {BitConverter.ToString(buffer.ToArray())}");

            if (conn.Read(this.buffer) == 0)
                return;

            Debug.WriteLine($"      After: {BitConverter.ToString(buffer.ToArray())}");

            ResponseFrame resp;

            buffer = TcmpFrame.RemoveEscapeCharacters(buffer.ToArray());

            for (int i = 0; i < buffer.Count; i++)
            {
                resp = null;
                try
                {
                    if (buffer[i] == 0x7E)
                    {
                        resp = ExtractFrame(i);

                        if (resp != null)
                        {
                            buffer.RemoveRange(0, resp.FrameLength + i);

                            Debug.WriteLine(string.Format("Command family: {0:X}, {1:X}\nResponse Code: {2:X}", resp.CommandFamily[0], resp.CommandFamily[1], resp.ResponseCode));
                            responseCallback?.Invoke(resp, null);
                            i -= 1;
                        }
                    }
                }
                catch (LackOfDataException exc)
                {
                    if (buffer.Count > 1 && buffer[buffer.Count - 1] == 0x7E)
                    {
                        responseCallback?.Invoke(null, exc);
                        FlushBuffer();
                    }
                }
                catch (LcsException exc)
                {
                    responseCallback?.Invoke(null, new LcsException(this.buffer.ToArray(), "There is an error in the length bytes since Len1+Len0+Lcs != 0"));
                }
                catch (HardwareException exc)
                {
                    responseCallback?.Invoke(null, new HardwareException("Connection to device is not open"));
                }
            }
        }

        private ResponseFrame ExtractFrame(int start)
        {
            if (buffer.Count - start < 10)
                return null;

            byte len1 = buffer[start + 1];
            byte len0 = buffer[start + 2];
            byte lcs = buffer[start + 3];

            int payLoadLength = len1 * 256 + len0 - 5;

            if (buffer.Count - start < len1 * 256 + len0 + 5)
            {
                //Debug.WriteLine($"Buffer Count: {buffer.Count}");
                //Debug.WriteLine($"Start: {start}");
                //Debug.WriteLine($"Length: {len1 * 256 + len0 + 5}");
                throw new LackOfDataException("Buffer length is less than the length of expected data");
            }

            if ((byte)(lcs + len1 + len0) != 0)
                throw new LcsException(this.buffer.ToArray());

            byte[] frame = new byte[payLoadLength + 10];
            Array.Copy(buffer.ToArray(), start, frame, 0, frame.Length);

            ResponseFrame result = new ResponseFrame(frame);

            if (result.IsApplicationErrorFrame())
                result = new ApplicationErrorFrame(result.ToArray());

            return result;
        }

        /// <summary>
        /// Clears the contents of the driver buffer
        /// </summary>
        public void FlushBuffer()
        {
            this.buffer.Clear();
            this.conn.Flush();
        }

        /// <summary>
        /// Connect to a Tappy device by the device name (TAPPY123)
        /// </summary>
        /// <returns>True if connection to a Tappy device was successful, false otherwise</returns>

        public bool ConnectByName(string tappyName)
        {
            foreach (string name in conn.GetAvailableDevices())
            {
                if (name.ToUpper() == tappyName.ToUpper())
                {
                    return Connect(tappyName);
                }
            }
            return false;
        }


        /// <summary>
        /// Connect to a Tappy device by the device name (TAPPY123) when it is in kiosk/keyboard wedge mode
        /// </summary>
        /// <returns>True if connection to a Tappy device was successful, false otherwise</returns>
        /// 
        public bool ConnectKioskKeyboardWedgeByName(string tappyName)
		{
			foreach (string name in conn.GetAvailableDevices())
			{
				if (name.ToUpper() == tappyName.ToUpper())
				{
					return ConnectKioskKeyboardWedge(tappyName);
				}
			}
			return false;
		}



		/// <summary>
		/// Connect to the first Tappy device the driver finds
		/// </summary>
		/// <returns>True if connection to a Tappy device was successful, false otherwise</returns>
		public bool AutoDetect()
        {
            foreach (string name in conn.GetAvailableDevices())
            {
                if (Connect(name)) return true;
            }

            return false;
        }

        /// <summary>
		/// Return a list of strings representing TappyBLEs nearby
		/// </summary>
		/// <returns>A list of strings representing TappyBLE device names</returns>
		///  /// <param name="timeout">Amount of time to scan for in ms</param>		
		public string[] FindNearbyTappyBLEs(int timeout)
		{
			if (conn.getBlueGigaStatus())
				return conn.GetAvailableDevices(timeout,false);
			else
				return conn.GetAvailableDevices(timeout, true);

			
		}


		/// <summary>
		/// Switch between USB or Bluetooth modes
		/// </summary>
		/// <param name="protocol"></param>
		public void SwitchProtocol(CommunicationProtocol protocol)
        {
            conn?.Disconnect();
            currentProtocol = protocol;
            InitializeConnection();
            FlushBuffer();

            conn.DataReceived += new EventHandler(DataReceivedHandler);
        }

        /// <summary>
        /// Connect to a given reader or port.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public bool Connect(string deviceName)
        {
			return ConnectWithTimeout(deviceName, 250);
        }

		/// <summary>
		/// Connect to a given reader or port when it is in kiosk/keyboard wedge mode where it will send heartbeat pings
		/// </summary>
		/// <param name="deviceName"></param>
		/// <returns></returns>
		public bool ConnectKioskKeyboardWedge(string deviceName)
		{
			return ConnectWithTimeout(deviceName,12000);
		}
		/// <summary>
		/// Connect to a given reader or port with a specified timeout on the ping response
		/// </summary>
		/// <param name="deviceName">Name of Device</param>
		/// <param name="timeout">Timeout in ms</param>
		/// <returns></returns>
		public bool ConnectWithTimeout(string deviceName,int timeout)
		{
			Command cmd = new Ping();
			AutoResetEvent receivedResp = new AutoResetEvent(false);
			bool success = false;

			if (!conn.Connect(deviceName))
				return false;

			Callback resp = (ResponseFrame frame, Exception e) =>
			{
				if (TcmpFrame.IsValidFrame(frame))
					success = true;
				receivedResp.Set();
			};

			SendCommand(cmd, resp);
			receivedResp.WaitOne(timeout);

			if (success)
				DeviceName = deviceName;

			return success;
		}

		/// <summary>
		/// Disconnect from the current reader
		/// </summary>
		public void Disconnect()
        {
            conn.Disconnect();
            DeviceName = null;
        }

        /// <summary>
        /// Get all the devices
        /// </summary>
        /// <returns>Array of device/port names</returns>
        public string[] GetAvailableDevices()
        {
            return conn.GetAvailableDevices();
        }

        /// <summary>
        /// Send a command to the Tappy
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="responseCallback">Method to be called when a data is receieved or a error has occurred</param>
        public void SendCommand(Command command, Callback responseCallback = null)
        {
            CommandFrame frame = new CommandFrame(command);
            _Send(frame, responseCallback);
        }

        /// <summary>
        /// Send a command to the Tappy 
        /// </summary>
        /// <typeparam name="T">Type of command to send</typeparam>
        /// <param name="responseCallback">Method to be called when a data is receieved or a error has occurred</param>
        /// <param name="parameters">Parameters of the command</param>
        public void SendCommand<T>(Callback responseCallback, params object[] parameters) where T : Command
        {
            CommandFrame frame = new CommandFrame((Command)Activator.CreateInstance(typeof(T), parameters));
            _Send(frame, responseCallback);
        }

        /// <summary>
        /// Send a command with no response call back
        /// </summary>
        /// <typeparam name="T">Type of command to send</typeparam>
        /// <param name="parameters">Parameters of the command</param>
        public void SendCommand<T>(params object[] parameters) where T : Command
        {
            CommandFrame frame = new CommandFrame((Command)Activator.CreateInstance(typeof(T), parameters));
            _Send(frame);
        }

        private void _Send(CommandFrame frame, Callback responseCallback = null)
        {
            this.responseCallback = responseCallback;

            try
            {
                conn.Send(frame.ToArray());
            }
            catch (HardwareException e)
            {
                this.responseCallback?.Invoke(null, e);
            }
        }

		/// <summary>
		/// Get the BLE connetion status of the TappyBLE
		/// </summary>
		public bool isConnected()
		{
			return conn.getConnectionStatus();
		}
		/// <summary>
		/// Get a boolean indicating if the Bluegiga dongle is connected
		/// </summary>

		public bool isBlueGigaConnected()
		{
			return conn.getBlueGigaStatus();
		}

		/// <summary>
		/// Add a method to be called if the BLE is disconnected
		/// </summary>
		public void setDisconnectCallback(DisconnectedEventHandler disconnectCallback)
		{
			conn.setDisconnectCallback(disconnectCallback);
		}

		/// <summary>
		/// Disconnect the BlueGiga dongle
		/// </summary>
		public void DisconnectBlueGiga()
		{
			conn.DisconnectBlueGiga();
		}

		public void Dispose()
        {
            conn.Dispose();
        }
    }
}

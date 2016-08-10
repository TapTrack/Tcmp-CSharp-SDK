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
    ///             Driver tappyDriver = new Driver(CommunicationProtocol.Usb); // Only USB is currently supported on Windows
    /// 
    ///             if (tappyDriver.AutoDetect())                               // Find and connect to a Tappy
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
    ///                 tappyDriver.SendCommand(readUid, responseCallback);
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
    ///             Driver tappyDriver = new Driver(CommunicationProtocol.Usb); // Only USB is currently supported on Windows
    /// 
    ///             if (tappyDriver.AutoDetect())                               // Find and connect to a Tappy
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
    ///                 tappyDriver.SendCommand(cmd, responseCallback);
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
    public class Driver
    {
        private Connection conn;
        private List<byte> buffer;

        private Callback responseCallback;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocol">Which protocol the driver should communicate over</param>
        public Driver(CommunicationProtocol protocol)
        {
            if (protocol == CommunicationProtocol.Usb)
            {
                conn = new UsbConnection();
            }
            else if (protocol == CommunicationProtocol.Bluetooth)
            {
                conn = new BluetoothConnection();
            }

            buffer = new List<byte>();
            conn.DataReceived += new EventHandler(DataReceivedHandler);
            DeviceName = null;
        }

        private void DataReceivedHandler(object sender, EventArgs e)
        {
            Debug.WriteLine("Data is being recieve");
            if (!conn.IsOpen())
                responseCallback(null, new HardwareException("Connection to device is not open"));

            Debug.WriteLine($"     Before: {BitConverter.ToString(buffer.ToArray())}");

            if (conn.Read(this.buffer) == 0)
                return;

            Debug.WriteLine($"      After: {BitConverter.ToString(buffer.ToArray())}");

            ResponseFrame resp;

            buffer = TcmpFrame.RemoveEscapseCharacters(buffer.ToArray());

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
                    responseCallback(null, new HardwareException("Connection to device is not open"));
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
        /// Connect to the first Tappy device the driver finds
        /// </summary>
        /// <returns>True if connection to a Tappy device was successful, false otherwise</returns>
        public bool AutoDetect()
        {
            bool success;
            foreach (string name in conn.GetAvailableDevices())
            {
                success = Connect(name);

                if (success)
                    return true;
            }
            return false;
        }

        public void SwitchProtocol(CommunicationProtocol protocol)
        {
            conn?.Disconnect();

            if (protocol == CommunicationProtocol.Usb)
                conn = new UsbConnection();
            else if (protocol == CommunicationProtocol.Bluetooth)
                conn = new BluetoothConnection();

            FlushBuffer();

            conn.DataReceived += new EventHandler(DataReceivedHandler);
        }

        public bool Connect(string deviceName)
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
            receivedResp.WaitOne(250);

            if (success)
                DeviceName = deviceName;

            return success;
        }

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
        /// Gets the name of the device the driver is currently connected to (USB port name or bluetooth device name depending on the current communcation protocal).
        /// Returns null if there is no device connected.
        /// </summary>
        public string DeviceName
        {
            get;
            internal set;
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
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="willLock"></param>
        /// <param name="url"></param>
        /// <param name="responseCallback"></param>
        public void WriteUrlWithTagMirror(byte timeout, bool willLock, string url, Callback responseCallback = null)
        {
            Command detectTag = new DetectSingleTagUid(timeout, DetectTagSetting.Type2Type4AandMifare);

            Callback detectTagCallback = (ResponseFrame frame, Exception e) =>
            {
                if (e == null && !frame.IsApplicationErrorFrame() && frame.ResponseCode == 0x01)
                {
                    Tag tag = new Tag(frame.Data);
                    Command write = new WriteUri(timeout, willLock, url.Replace("[uid]", tag.UidToString()));
                    Task.Run(() => SendCommand(write, responseCallback));
                }
                else
                {
                    responseCallback.Invoke(frame, e);
                }
            };

            SendCommand(detectTag, detectTagCallback);
        }
    }
}

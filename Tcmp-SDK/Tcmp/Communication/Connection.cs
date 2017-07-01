using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    internal abstract class Connection : IDisposable
    {
        public event EventHandler DataReceived;

        protected virtual void OnDataReceived(EventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public abstract bool Connect(string deviceName);
        public abstract void Disconnect();
        public abstract void Send(byte[] data);
        public abstract int Read(List<byte> data);
        public abstract string[] GetAvailableDevices();
		public abstract string[] GetAvailableDevices(int timeout);
		public abstract string[] GetAvailableDevices(int timeou, bool scanForBlueGiga);
		public abstract bool IsOpen();
        public abstract void Flush();
        public abstract void Dispose();
		public abstract bool getConnectionStatus();
		public abstract bool getBlueGigaStatus();
		public abstract void setDisconnectCallback(Bluegiga.BLE.Events.Connection.DisconnectedEventHandler disconnectCallback);
		public abstract void DisconnectBlueGiga();

	}
}

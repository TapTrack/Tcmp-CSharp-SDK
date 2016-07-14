using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    internal abstract class Connection
    {
        public event EventHandler DataReceived;

        protected virtual void OnDataReceived(EventArgs e)
        {
            EventHandler handler = DataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public abstract void Connect(string deviceName);
        public abstract void Disconnect();
        public abstract void Send(byte[] data);
        public abstract int Read(List<byte> data);
        public abstract string[] GetAvailableDevices();
        public abstract bool IsOpen();
    }
}

using System;
using System.Text;

namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to write text to a NFC tag
    /// </summary>
    public class WriteText : BasicNfcCommand
    {
        private const byte commandCode = 0x06;

        public WriteText(byte timeout, bool willLock, string text)
        {
            this.parameters.Add(timeout);
            this.parameters.Add(Convert.ToByte(willLock));
            this.parameters.AddRange(Encoding.UTF8.GetBytes(text));
        }

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }
    }
}

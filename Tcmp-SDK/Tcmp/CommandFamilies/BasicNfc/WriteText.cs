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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="willLock">If true, the Tappy will lock the tag after writing</param>
        /// <param name="text">Text to write to a tag</param>
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

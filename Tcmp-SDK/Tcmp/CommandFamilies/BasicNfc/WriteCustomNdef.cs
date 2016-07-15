using System;
using TapTrack.Ndef;

namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to write a custom NDEF message to an NFC tag
    /// </summary>
    public class WriteCustomNdef : BasicNfcCommand
    {
        private const byte commandCode = 0x07;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="willLock">If true, the Tappy will lock the tag after writing</param>
        /// <param name="ndefMessage">bytes of the ndef message to write to a tag</param>
        public WriteCustomNdef(byte timeout, bool willLock, byte[] ndefMessage)
        {
            this.parameters.Add(timeout);
            this.parameters.Add(Convert.ToByte(willLock));
            this.parameters.AddRange(ndefMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="willLock">If true will lock the tag</param>
        /// <param name="ndefMessage">Ndef message to write to a tag</param>
        public WriteCustomNdef(byte timeout, bool willLock, NdefMessage message)
        {
            this.parameters.Add(timeout);
            this.parameters.Add(Convert.ToByte(willLock));
            this.parameters.AddRange(message.GetByteArray());
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

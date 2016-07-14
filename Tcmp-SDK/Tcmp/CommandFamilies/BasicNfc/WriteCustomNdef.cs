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

        public WriteCustomNdef(byte timeout, bool willLock, byte[] ndefMessage)
        {
            this.parameters.Add(timeout);
            this.parameters.Add(Convert.ToByte(willLock));
            this.parameters.AddRange(ndefMessage);
        }

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

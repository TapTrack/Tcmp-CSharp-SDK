using System;
using System.Text;
using TapTrack.Ndef;

namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to write a URI to a NFC tag
    /// </summary>
    public class WriteUri : BasicNfcCommand
    {
        private const byte commandCode = 0x05;

        public WriteUri(byte timeout, bool willLock, NdefUri uri)
        {
            parameters.Add(timeout);
            parameters.Add(Convert.ToByte(willLock));
            parameters.Add(uri.Scheme);
            parameters.AddRange(Encoding.UTF8.GetBytes(uri.Path));
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

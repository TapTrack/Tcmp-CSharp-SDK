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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="willLock">If true, the Tappy will lock the tag after writing</param>
        /// <param name="uri">Uri to be written to a tag</param>
        public WriteUri(byte timeout, bool willLock, NdefUri uri)
        {
            parameters.Add(timeout);
            parameters.Add(Convert.ToByte(willLock));
            parameters.Add(uri.Scheme);
            parameters.AddRange(Encoding.UTF8.GetBytes(uri.Path));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="willLock">If true, the Tappy will lock the tag after writing</param>
        /// <param name="uri">Uri to be written to a tag</param>
        public WriteUri(byte timeout, bool willLock, string uri)
        {
            NdefUri _uri = new NdefUri(uri);
            parameters.Add(timeout);
            parameters.Add(Convert.ToByte(willLock));
            parameters.Add(_uri.Scheme);
            parameters.AddRange(Encoding.UTF8.GetBytes(_uri.Path));
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

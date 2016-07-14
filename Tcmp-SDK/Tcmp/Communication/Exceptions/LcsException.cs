using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    /// <summary>
    /// Caused when the length check sum (LCS) plus the length bytes is not equal to 0
    /// </summary>
    public class LcsException : Exception
    {
        public LcsException()
        {

        }

        public LcsException(byte[] buffer)
        {
            Buffer = buffer;
        }

        public LcsException (byte[] buffer, string message) : base(message)
        {
            Buffer = buffer;
        }

        public byte[] Buffer
        {
            get;
            set;
        }
    }
}

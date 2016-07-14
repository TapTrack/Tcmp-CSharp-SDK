using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationErrorFrame : ResponseFrame
    {
        public ApplicationErrorFrame(byte[] raw) : base(raw)
        {
        }

        public byte ErrorCode
        {
            get
            {
                return this.contents[7];
            }
        }

        public byte ErrorByte
        {
            get
            {
                return this.contents[8];
            }
        }

        public byte NfcStatus
        {
            get
            {
                return this.contents[9];
            }
        }

        public string ErrorString
        {
            get
            {
                return Encoding.UTF8.GetString(Data, 3, Length - 3 - 5);
            }
        }
    }
}

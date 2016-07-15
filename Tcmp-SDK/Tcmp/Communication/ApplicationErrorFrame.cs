using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    /// <summary>
    /// Represents an application error frame that is sent by the Tappy
    /// </summary>
    public class ApplicationErrorFrame : ResponseFrame
    {
        public ApplicationErrorFrame(byte[] raw) : base(raw)
        {
        }

        /// <summary>
        /// Error code associated with the error
        /// </summary>
        public byte ErrorCode
        {
            get
            {
                return this.contents[7];
            }
        }

        /// <summary>
        /// Diagnostic information for Taptrack
        /// </summary>
        public byte ErrorByte
        {
            get
            {
                return this.contents[8];
            }
        }

        /// <summary>
        /// Diagnostic information for Taptrack
        /// </summary>
        public byte NfcStatus
        {
            get
            {
                return this.contents[9];
            }
        }

        /// <summary>
        /// Human readable error
        /// </summary>
        public string ErrorString
        {
            get
            {
                return Encoding.UTF8.GetString(Data, 3, Length - 3 - 5);
            }
        }
    }
}

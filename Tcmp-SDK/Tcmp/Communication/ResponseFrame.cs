using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    /// <summary>
    /// Represents a frame recieved from the Tappy
    /// </summary>
    public class ResponseFrame : TcmpFrame
    {
        public ResponseFrame(byte[] raw)
        {
            contents = RemoveEscapseCharacters(raw);
        }

        /// <summary>
        /// See the list of response in the TCMP documentation at https://docs.google.com/document/d/1MjHizibAd6Z1PGZAWnbStXnCBVggptx3TIh2HRqEluk/edit#
        /// </summary>
        public byte ResponseCode
        {
            get
            {
                return contents[6];
            }
        }

        /// <summary>
        /// Checks if this frame is an application error frame
        /// </summary>
        /// <returns>True if application error frame, false otherwise</returns>
        public bool IsApplicationErrorFrame()
        {
            if (ResponseCode == 0x7F)
                return true;
            return false;
        }
    }
}

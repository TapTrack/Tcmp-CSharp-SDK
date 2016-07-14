using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    public class ResponseFrame : TcmpFrame
    {
        public ResponseFrame(byte[] raw)
        {
            contents = RemoveEscapseCharacters(raw);
        }

        public byte ResponseCode
        {
            get
            {
                return contents[6];
            }
        }

        public bool IsApplicationErrorFrame()
        {
            if (ResponseCode == 0x7F)
                return true;
            return false;
        }
    }
}

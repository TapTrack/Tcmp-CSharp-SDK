using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Type4
{
    /// <summary>
    /// Command to sent APDU bytes to a tag
    /// </summary>
    public class TransceiveAPDU : Type4Command
    {
        private const byte commandCode = 0x02;

        public TransceiveAPDU(byte[] data)
        {
            parameters.AddRange(data);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Type4
{
    /// <summary>
    /// Command to get the ATQB and ATTRIB from the type B tag
    /// </summary>
    public class DetectType4B : Type4Command
    {
        private const byte commandCode = 0x03;

        public DetectType4B(byte timeout)
        {
            parameters.Add(timeout);
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

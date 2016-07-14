using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Type4
{
    /// <summary>
    /// Command to get the version of the Type 4 command family
    /// </summary>
    public class GetType4CmdFamilyVersion : Type4Command
    {
        private const byte commandCode = 0xFF;

        public GetType4CmdFamilyVersion()
        {

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

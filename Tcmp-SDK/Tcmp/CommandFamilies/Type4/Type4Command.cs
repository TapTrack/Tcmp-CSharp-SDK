using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Type4
{
    public abstract class Type4Command : Command
    {
        public static readonly byte[] commandFamily = new byte[] { 0x00, 0x04 };

        public override byte[] CommandFamily
        {
            get
            {
                return commandFamily;
            }
        }
    }
}

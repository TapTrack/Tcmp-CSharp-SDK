using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public abstract class Ntag21xCommand : Command
    {
        public static readonly byte[] commandFamily = new byte[] { 0x00, 0x06 };

        public override byte[] CommandFamily
        {
            get
            {
                return commandFamily;
            }
        }
    }
}

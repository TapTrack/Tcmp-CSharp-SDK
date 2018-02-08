using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Command to get the number of checkins stored in the Tappy
/// </summary>

namespace TapTrack.Tcmp.CommandFamilies.StandaloneCheckin
{

    public class getNumCheckins : StandaloneCheckin
    {
        private const byte commandCode = 0x02;

        public getNumCheckins()
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

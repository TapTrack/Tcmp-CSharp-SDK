using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    public enum DetectTagSetting : byte
    {
        /// <summary>
        /// Poll for type 1 tags
        /// </summary>
        Type1 = 0x01,

        /// <summary>
        /// Poll for Type 2, Type 4A, and Mifare Classic tags
        /// </summary>
        Type2Type4AandMifare = 0x02,

        /// <summary>
        /// Poll for type 4B tags
        /// </summary>
        Type4B = 0x04
    }
}

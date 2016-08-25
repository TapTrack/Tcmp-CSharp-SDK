using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.System
{
    /// <summary>
    /// Command to enable or disable Type 2 tag idenification during the detect tag command.
    /// </summary>
    public class SetType2TagIdentification : ConfigureSetting
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enableIdentification">If true the detect tag command will return the exact model and manufacturer of a Type 2 tag, otherwise it will not</param>
        public SetType2TagIdentification(bool enableIdentification) : base(TappySetting.Type2TagIdentification, Convert.ToByte(enableIdentification))
        {
        }
    }
}

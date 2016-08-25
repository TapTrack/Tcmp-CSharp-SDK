using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.System
{
    public class EnableDataThrottling : ConfigureSetting
    {
        /// <summary>
        /// <para>Throttles the data that is sent by the Tappy.</para>
        /// <note type="note">
        ///     Should be used with the TappyBLE.
        /// </note>
        /// </summary>
        /// <param name="delay">Time to delay between numPackets. Time is in milliseconds</param>
        /// <param name="numPackets">Number of packets to send before pausing the time specified by the delay parameter</param>
        public EnableDataThrottling(byte delay, byte numPackets) : base(TappySetting.EnableDataThrottling, delay, numPackets)
        {
        }
    }
}

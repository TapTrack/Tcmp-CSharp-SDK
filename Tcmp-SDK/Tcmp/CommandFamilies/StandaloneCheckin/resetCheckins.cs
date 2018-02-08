using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Command to reset the checkins (i.e clear/erase the checkins after download if desired
/// </summary>

namespace TapTrack.Tcmp.CommandFamilies.StandaloneCheckin
{

	public class resetCheckins : StandaloneCheckin
	{
		private const byte commandCode = 0x06;

		public resetCheckins()
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

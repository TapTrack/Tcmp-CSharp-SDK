using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Command to read the station name from the Tappy
/// </summary>

namespace TapTrack.Tcmp.CommandFamilies.StandaloneCheckin
{
	public class GetStationInfo : StandaloneCheckin
	{
		private const byte commandCode = 0x07; 

		public GetStationInfo()
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

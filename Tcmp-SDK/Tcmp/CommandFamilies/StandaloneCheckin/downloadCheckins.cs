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

	public class downloadCheckins : StandaloneCheckin
	{
		private const byte commandCode = 0x01;

		public downloadCheckins(UInt16 firstRecord, UInt16 lastRecord)
		{			
			parameters.AddRange(BitConverter.GetBytes(firstRecord));
			parameters.AddRange(BitConverter.GetBytes(lastRecord));

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
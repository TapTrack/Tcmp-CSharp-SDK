using System;

namespace TapTrack.Tcmp.CommandFamilies.System
{

	/// <summary>
	/// Command to configure thee Tappy when in keyboard wedge/ kiosk mode
	/// </summary>
	public class ConfigureKioskKeyboardWedgeMode : SystemCommand
	{
		public const byte commandCode = 0x06;

		public override byte CommandCode
		{
			get
			{
				return commandCode;
			}
		}
		public ConfigureKioskKeyboardWedgeMode(byte dualTagDetection, byte ndefReading, byte  hearbeatPeriod, byte enableScanErrMsgs, short postSuccessDelay,  short postFailDelay, short postScanStagger)
		{
			byte[] successMsDelay = BitConverter.GetBytes(postSuccessDelay);
			byte[] failMsDelay = BitConverter.GetBytes(postFailDelay);
			byte[] staggerMs = BitConverter.GetBytes(postScanStagger);

			Array.Reverse(successMsDelay);
			Array.Reverse(failMsDelay);
			Array.Reverse(staggerMs);

			this.parameters.Add(dualTagDetection);
			this.parameters.Add(ndefReading);
			this.parameters.Add(hearbeatPeriod);
			this.parameters.Add(enableScanErrMsgs);

			this.parameters.AddRange(successMsDelay);
			this.parameters.AddRange(failMsDelay);
			this.parameters.AddRange(staggerMs);

		}

	}
}

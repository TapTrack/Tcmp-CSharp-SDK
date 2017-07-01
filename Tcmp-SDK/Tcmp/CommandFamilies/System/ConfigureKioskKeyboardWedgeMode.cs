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
		public ConfigureKioskKeyboardWedgeMode(byte dualTagDetection, byte ndefReading, byte  hearbeatPeriod, byte enableScanErrMsgs)
		{
			this.parameters.Add(dualTagDetection);
			this.parameters.Add(ndefReading);
			this.parameters.Add(hearbeatPeriod);
			this.parameters.Add(enableScanErrMsgs);

		}

	}
}

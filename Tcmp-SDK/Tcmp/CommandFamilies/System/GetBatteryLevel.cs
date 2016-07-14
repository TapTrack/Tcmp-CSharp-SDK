namespace TapTrack.Tcmp.CommandFamilies.System
{
    /// <summary>
    /// Command to get the battery level for the Tappy. May not work correct for devices with no battery
    /// </summary>
    public class GetBatteryLevel : SystemCommand
    {
        private const byte commandCode = 0x02;

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }
    }
}

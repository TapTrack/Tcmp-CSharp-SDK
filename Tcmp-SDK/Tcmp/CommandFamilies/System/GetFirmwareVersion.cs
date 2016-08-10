namespace TapTrack.Tcmp.CommandFamilies.System
{
    /// <summary>
    /// Command to get the firmware version of the Tappy
    /// </summary>
    public class GetFirmwareVersion : SystemCommand
    {
        private const byte commandCode = 0xFF;

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }
    }
}

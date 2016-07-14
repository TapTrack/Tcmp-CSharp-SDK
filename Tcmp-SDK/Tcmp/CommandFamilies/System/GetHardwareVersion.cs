namespace TapTrack.Tcmp.CommandFamilies.System
{
    /// <summary>
    /// Command to get the hardware version of the Tappy
    /// </summary>
    public class GetHardwareVersion : SystemCommand
    {
        private const byte commandCode = 0xFE;

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }
    }
}

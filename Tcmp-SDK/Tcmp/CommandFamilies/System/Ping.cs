namespace TapTrack.Tcmp.CommandFamilies.System
{
    /// <summary>
    /// Command to ping the Tappy.
    /// </summary>
    public class Ping : SystemCommand
    {
        private const byte commandCode = 0xFD;

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }
    }
}

namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to stop whatever operations the Tappy is doing
    /// </summary>
    public class Stop : BasicNfcCommand
    {
        private const byte commandCode = 0x00;

        public Stop()
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

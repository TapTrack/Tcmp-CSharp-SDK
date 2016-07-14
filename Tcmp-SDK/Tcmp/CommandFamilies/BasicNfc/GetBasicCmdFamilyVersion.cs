namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to get the version of the basic NFC command family
    /// </summary>
    public class GetBasicCmdFamilyVersion : BasicNfcCommand
    {
        private const byte commandCode = 0xFF;

        public GetBasicCmdFamilyVersion()
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

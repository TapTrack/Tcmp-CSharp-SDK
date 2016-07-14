namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to get the uid of a NFC tag
    /// </summary>
    public class DetectSingleTagUid : DetectCommand
    {
        private const byte commandCode = 0x02;

        public DetectSingleTagUid(byte timeout, DetectTagSetting tagType) : base(timeout, tagType)
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

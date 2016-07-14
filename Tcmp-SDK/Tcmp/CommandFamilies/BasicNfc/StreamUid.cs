namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to continuously get the UID from NFC tags
    /// </summary>
    public class StreamUid : DetectCommand
    {
        private const byte commandCode = 0x01;

        public StreamUid(byte timeout, DetectTagSetting tagType) : base(timeout, tagType)
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

namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to continuously read NDEF messages from NFC tags
    /// </summary>
    public class StreamNdef : DetectCommand
    {
        private const byte commandCode = 0x03;

        public StreamNdef(byte timeout, DetectTagSetting tagType) : base(timeout, tagType)
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

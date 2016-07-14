namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to read a NDEF mesage off a tag once
    /// </summary>
    public class DetectSingleNdef : DetectCommand
    {
        private const byte commandCode = 0x04;

        public DetectSingleNdef(byte timeout, DetectTagSetting tagType) : base(timeout, tagType)
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

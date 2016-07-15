namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to continuously read NDEF messages from NFC tags
    /// </summary>
    public class StreamNdef : DetectCommand
    {
        private const byte commandCode = 0x03;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="tagType">Type of tag to detect</param>
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

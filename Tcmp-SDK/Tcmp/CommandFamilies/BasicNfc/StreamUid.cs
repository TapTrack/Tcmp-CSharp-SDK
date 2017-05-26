namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to continuously get the UID from NFC tags
    /// </summary>
    public class StreamUid : DetectCommand
    {
        private const byte commandCode = 0x01;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="tagType">Type of tag to detect</param>
        public StreamUid(byte timeout, DetectTagSetting tagType, int blah=0) : base(timeout, tagType)
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

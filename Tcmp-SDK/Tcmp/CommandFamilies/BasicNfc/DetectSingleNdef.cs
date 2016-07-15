namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to read a NDEF mesage off a tag once
    /// </summary>
    public class DetectSingleNdef : DetectCommand
    {
        private const byte commandCode = 0x04;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="tagType">Type of tag to detect</param>
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

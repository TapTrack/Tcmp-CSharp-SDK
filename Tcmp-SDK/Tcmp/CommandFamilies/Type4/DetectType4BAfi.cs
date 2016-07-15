namespace TapTrack.Tcmp.CommandFamilies.Type4
{
    /// <summary>
    /// Command to return the ATQB and ATTRIB from the type B tag only if the AFI of the tag matches the one given
    /// </summary>
    public class DetectType4BAfi : Type4Command
    {
        private const byte commandCode = 0x04;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="afi">Only detect Type 4B tags with given AFI</param>
        public DetectType4BAfi(byte timeout, byte afi)
        {
            parameters.Add(timeout);
            parameters.Add(afi);
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

namespace TapTrack.Tcmp.CommandFamilies.Type4
{
    /// <summary>
    /// Command to return the ATQB and ATTRIB from the type B tag only if the AFI of the tag matches the one given
    /// </summary>
    public class DetectType4BAfi : Type4Command
    {
        private const byte commandCode = 0x04;

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

namespace TapTrack.Tcmp.CommandFamilies.Type4
{
    /// <summary>
    /// Command to get the UID and if possible the ATS from a NFC tag
    /// </summary>
    public class DetectType4A : Type4Command
    {
        private const byte commandCode = 0x01;

        public DetectType4A(byte timeout)
        {
            parameters.Add(timeout);
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

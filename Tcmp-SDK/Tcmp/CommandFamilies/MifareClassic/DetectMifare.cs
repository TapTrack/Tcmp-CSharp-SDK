namespace TapTrack.Tcmp.CommandFamilies.MifareClassic
{
    /// <summary>
    /// Command to get the UID of a Mifare classic card
    /// </summary>
    public class DetectMifare : MifareClassicCommand
    {
        private const byte commandCode = 0x02;

        public DetectMifare(byte timeout)
        {
            this.parameters.Add(timeout);
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

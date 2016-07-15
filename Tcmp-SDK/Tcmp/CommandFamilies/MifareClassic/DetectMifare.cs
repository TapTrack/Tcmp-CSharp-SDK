namespace TapTrack.Tcmp.CommandFamilies.MifareClassic
{
    /// <summary>
    /// Command to get the UID of a Mifare classic card
    /// </summary>
    public class DetectMifare : MifareClassicCommand
    {
        private const byte commandCode = 0x02;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
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

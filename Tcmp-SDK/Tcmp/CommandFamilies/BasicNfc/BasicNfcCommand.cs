namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    public abstract class BasicNfcCommand : Command
    {
        public static readonly byte[] commandFamily = new byte[] { 0x00, 0x01 };

        public override byte[] CommandFamily
        {
            get
            {
                return commandFamily;
            }
        }
    }
}

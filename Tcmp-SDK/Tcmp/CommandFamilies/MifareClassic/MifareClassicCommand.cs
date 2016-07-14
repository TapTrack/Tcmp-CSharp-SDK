namespace TapTrack.Tcmp.CommandFamilies.MifareClassic
{
    public abstract class MifareClassicCommand : Command
    {
        public static readonly byte[] commandFamily = new byte[] { 0x00, 0x03 }; 

        public override byte[] CommandFamily
        {
            get
            {
                return commandFamily;
            }
        }
    }
}

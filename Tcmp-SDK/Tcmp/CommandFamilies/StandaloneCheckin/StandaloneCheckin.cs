namespace TapTrack.Tcmp.CommandFamilies.StandaloneCheckin
{
    public abstract class StandaloneCheckin : Command
    {
        public static readonly byte[] commandFamily = new byte[] { 0x00, 0x05 };

        public override byte[] CommandFamily
        {
            get
            {
                return commandFamily;
            }
        }
    }
}

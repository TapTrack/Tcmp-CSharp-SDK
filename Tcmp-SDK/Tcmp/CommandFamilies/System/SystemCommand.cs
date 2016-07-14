namespace TapTrack.Tcmp.CommandFamilies.System
{
    public abstract class SystemCommand : Command
    {
        public static readonly byte[] commandFamily = new byte[] { 0x00, 0x00 };

        public override byte[] CommandFamily
        {
            get
            {
                return commandFamily;
            }
        }
    }
}

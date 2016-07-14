namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    public abstract class DetectCommand : BasicNfcCommand
    {
        public DetectCommand(byte timeout, DetectTagSetting tagType)
        {
            this.parameters.Add(timeout);
            this.parameters.Add((byte)tagType);
        }
    }
}

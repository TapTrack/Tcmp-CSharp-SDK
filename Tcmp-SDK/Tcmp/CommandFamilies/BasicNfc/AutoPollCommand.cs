namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    public enum AutoPollScanMode : byte
    {
        Type2   = 0x00,
        Type1   = 0x01,
        Type4B  = 0x02,
        Felicia = 0x03,
        Type4A  = 0x04,
        Any     = 0x05
    }

    public class AutoPollCommand : BasicNfcCommand
    {
        public override byte CommandCode { get; } = 0x10;
        
        public AutoPollScanMode ScanMode { get; }
        public byte HeartBeatPeriod { get; }
        public bool IsBuzzerEnabled { get; }

        public AutoPollCommand(AutoPollScanMode scanMode, byte heartBeatPeriod, bool enableBuzzer = true)
        {
            ScanMode        = scanMode;
            HeartBeatPeriod = heartBeatPeriod;
            IsBuzzerEnabled = enableBuzzer;

            parameters.Add((byte)ScanMode);
            parameters.Add(HeartBeatPeriod);
            parameters.Add(IsBuzzerEnabled ? (byte)0x00 : (byte)0x01);
        }
    }
}

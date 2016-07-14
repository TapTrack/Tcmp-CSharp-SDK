namespace TapTrack.Tcmp.CommandFamilies.MifareClassic
{
    /// <summary>
    /// Command to read a block of memory from a Mifare classic tag
    /// </summary>
    public class ReadBlock : MifareClassicCommand
    {
        private const byte commandCode = 0x01;

        public ReadBlock(byte timeout, byte startBlock, byte endBlock, KeySetting setting, byte[] key)
        {
            parameters.Add(timeout);
            parameters.Add(startBlock);
            parameters.Add(endBlock);
            parameters.Add((byte)setting);
            parameters.AddRange(key);
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

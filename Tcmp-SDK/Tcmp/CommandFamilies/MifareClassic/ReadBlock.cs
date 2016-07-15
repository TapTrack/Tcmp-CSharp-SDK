namespace TapTrack.Tcmp.CommandFamilies.MifareClassic
{
    /// <summary>
    /// Command to read a block of memory from a Mifare classic tag
    /// </summary>
    public class ReadBlock : MifareClassicCommand
    {
        private const byte commandCode = 0x01;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
        /// <param name="startBlock">Memory block to start reading from</param>
        /// <param name="endBlock">Memory block to stop reading</param>
        /// <param name="setting"></param>
        /// <param name="key"></param>
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

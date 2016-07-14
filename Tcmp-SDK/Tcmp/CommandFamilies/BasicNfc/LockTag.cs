using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to lock a tag
    /// </summary>
    public class LockTag : BasicNfcCommand
    {
        private const byte commandCode = 0x08;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="uid">Uid of the tag to lock</param>
        public LockTag (byte timeout, byte[] uid)
        {
            this.parameters.Add(timeout);
            this.parameters.AddRange(uid);
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

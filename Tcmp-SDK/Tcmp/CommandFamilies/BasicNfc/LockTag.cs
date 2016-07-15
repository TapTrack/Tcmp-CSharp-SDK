using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.BasicNfc
{
    /// <summary>
    /// Command to lock a tag to prevent further writing
    /// </summary>
    public class LockTag : BasicNfcCommand
    {
        private const byte commandCode = 0x08;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">Maximum time the Tappy will wait for a tag. Time out is in seconds. 0 = No time out</param>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public class WritePasswordTextPwdPack : WritePasswordNdefCommandPwdPack
    {
        private const byte commandCode = 0x05;

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }

        public WritePasswordTextPwdPack(string content, byte timeout, PasswordProtectionMode passwordProtectionMode, byte[] pwd, byte[] pack) : base(content, timeout, passwordProtectionMode, pwd, pack)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public class WritePasswordUriPwdPack : WritePasswordNdefCommandPwdPack
    {
        private const byte commandCode = 0x06;

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }

        public WritePasswordUriPwdPack(string content, byte timeout, PasswordProtectionMode passwordProtectionMode, byte[] pwd, byte[] pack) : base(content, timeout, passwordProtectionMode, pwd,pack)
        {

        }
    }
}

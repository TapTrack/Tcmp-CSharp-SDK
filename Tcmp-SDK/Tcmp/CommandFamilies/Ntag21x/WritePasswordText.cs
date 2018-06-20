using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public class WritePasswordText : WritePasswordNdefCommand
    {
        private const byte commandCode = 0x01;

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }

        public WritePasswordText(string password, string content, byte timeout, PasswordProtectionMode passwordProtectionMode) : base(password, content, timeout, passwordProtectionMode)
        {

        }
    }
}

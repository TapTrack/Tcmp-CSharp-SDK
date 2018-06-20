using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public class WritePasswordUri : WritePasswordNdefCommand
    {
        private const byte commandCode = 0x02;

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }

        public WritePasswordUri(string password, string content, byte timeout, PasswordProtectionMode passwordProtectionMode) : base (password, content, timeout, passwordProtectionMode)
        {
            
        }

    }
}

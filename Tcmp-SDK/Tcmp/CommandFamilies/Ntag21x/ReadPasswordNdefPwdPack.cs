using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public class ReadPasswordNdefPwdPack : Ntag21xCommand
    {

        private const byte commandCode = 0x08;
        private byte timeout { get; }
        private byte[] pwd { get; }
        private byte[] pack { get; }

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }

        public ReadPasswordNdefPwdPack(byte timeout, byte[] pwd, byte[] pack)
        {
            if (pwd.Length != 4 || pack.Length != 2)
            {
                throw new Exception("pwd bust be 4 bytes and pack must be 2 bytes");
            }

            this.timeout = timeout;
            this.pwd = pwd;
            this.pack = pack;

            this.parameters.Add(timeout);
            this.parameters.AddRange(pwd);
            this.parameters.AddRange(pack);

       
        }
    }
}

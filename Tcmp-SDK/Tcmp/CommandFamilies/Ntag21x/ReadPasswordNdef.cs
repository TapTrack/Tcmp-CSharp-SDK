using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{

    public class ReadPasswordNdef : Command
    {
        public static readonly byte[] commandFamily = new byte[] { 0x00, 0x06 };
        private const byte commandCode = 0x04;
        private byte timeout { get; }
        private string password { get; }

        
        public override byte[] CommandFamily
        {
            get
            {
                return commandFamily;
            }
        }

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }

        public ReadPasswordNdef(byte timeout, string password)
        {
            this.timeout = timeout;
            this.password = password;
            this.parameters.Add(timeout);

            UInt16 passwordLength = (UInt16)password.Length;
            byte[] temp = BitConverter.GetBytes(passwordLength);
            byte[] passwordLengthBytes = new byte[2];
            passwordLengthBytes[0] = temp[1];
            passwordLengthBytes[1] = temp[0];

            this.parameters.AddRange(passwordLengthBytes);

            this.parameters.AddRange(Encoding.UTF8.GetBytes(password));
        }
    }
}

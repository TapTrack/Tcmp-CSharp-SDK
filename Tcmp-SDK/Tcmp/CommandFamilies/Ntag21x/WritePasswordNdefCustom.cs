using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NdefLibrary.Ndef;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public class WritePasswordNdefCustom : Ntag21xCommand
    {
        private const byte commandCode = 0x03;
        private NdefMessage ndef { get; }
        private string password { get; }
        private int timeout { get; set; }
        private PasswordProtectionMode passwordProtectionMode { get; }

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }

        public WritePasswordNdefCustom(string password, NdefMessage ndef, byte timeout, PasswordProtectionMode passwordProtectionMode)
        {
            this.password = password;
            this.timeout = timeout;
            this.passwordProtectionMode = passwordProtectionMode;


            UInt16 passwordLength = (UInt16)password.Length;
            UInt16 contentLength = (UInt16)ndef.ToByteArray().Length;

            this.parameters.Add(timeout);

            if (passwordProtectionMode == PasswordProtectionMode.PASSWORD_FOR_WRITE)
            {
                this.parameters.Add(0x00);
            }
            else if (passwordProtectionMode == PasswordProtectionMode.PASSWORD_FOR_READWRITE)
            {
                this.parameters.Add(0x01);
            }

            byte[] passwordLengthBytes = new byte[2];
            byte[] contentLengthBytes = new byte[2];
            byte[] temp = new byte[2];

            temp = BitConverter.GetBytes(passwordLength);
            passwordLengthBytes[0] = temp[1];
            passwordLengthBytes[1] = temp[0];

            temp = BitConverter.GetBytes(contentLength);
            contentLengthBytes[0] = temp[1];
            contentLengthBytes[1] = temp[0];

            this.parameters.AddRange(passwordLengthBytes);
            this.parameters.AddRange(Encoding.UTF8.GetBytes(password));
            this.parameters.AddRange(contentLengthBytes);
            this.parameters.AddRange(ndef.ToByteArray());

        }

    }
}

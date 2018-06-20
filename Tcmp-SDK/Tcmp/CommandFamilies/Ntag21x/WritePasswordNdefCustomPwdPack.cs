using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NdefLibrary.Ndef;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public class WritePasswordNdefCustomPwdPack : Ntag21xCommand
    {
        private const byte commandCode = 0x07;
        private NdefMessage ndef { get; }
        private byte[] pwd { get; }
        private byte[] pack { get; }
        private int timeout { get; set; }
        private PasswordProtectionMode passwordProtectionMode { get; }

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }

        public WritePasswordNdefCustomPwdPack(byte[] pwd, byte [] pack, NdefMessage ndef, byte timeout, PasswordProtectionMode passwordProtectionMode)
        {
            if (pwd.Length != 4 || pack.Length != 2)
            {
                throw new Exception("pwd bust be 4 bytes and pack must be 2 bytes");
            }

            this.pwd = pwd;
            this.pack = pack;
            this.timeout = timeout;
            this.passwordProtectionMode = passwordProtectionMode;

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
     
            byte[] contentLengthBytes = new byte[2];
            byte[] temp = new byte[2];

            temp = BitConverter.GetBytes(contentLength);
            contentLengthBytes[0] = temp[1];
            contentLengthBytes[1] = temp[0];

            this.parameters.AddRange(pwd);
            this.parameters.AddRange(pack);
            this.parameters.AddRange(contentLengthBytes);
            this.parameters.AddRange(ndef.ToByteArray());

        }
    }
}

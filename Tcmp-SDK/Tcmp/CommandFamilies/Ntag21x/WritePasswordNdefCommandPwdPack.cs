using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.Ntag21x
{
    public abstract class WritePasswordNdefCommandPwdPack : Ntag21xCommand
    {
        private string password { get; }
        private string content { get; set; }
        private int timeout { get; set; }
        private PasswordProtectionMode passwordProtectionMode { get; }

        private byte[] pwd { get; }
        private byte[] pack { get; }

        public WritePasswordNdefCommandPwdPack(string content, byte timeout, PasswordProtectionMode passwordProtectionMode, byte[] pwd, byte[] pack)
        {
            if(pwd.Length != 4 || pack.Length != 2)
            {
                throw new Exception("pwd bust be 4 bytes and pack must be 2 bytes");
            }

            this.password = password;
            this.content = content;
            this.timeout = timeout;
            this.passwordProtectionMode = passwordProtectionMode;
            this.pwd = pwd;
            this.pack = pack;

            UInt16 contentLength = (UInt16)content.Length;

            this.parameters.Add(timeout);

            if (passwordProtectionMode == PasswordProtectionMode.PASSWORD_FOR_WRITE)
            {
                this.parameters.Add(0x00);
            }
            else if (passwordProtectionMode == PasswordProtectionMode.PASSWORD_FOR_READWRITE)
            {
                this.parameters.Add(0x01);
            }

            this.parameters.AddRange(pwd);
            this.parameters.AddRange(pack);

            byte[] contentLengthBytes = new byte[2];
            byte[] temp = new byte[2];

            temp = BitConverter.GetBytes(contentLength);
            contentLengthBytes[0] = temp[1];
            contentLengthBytes[1] = temp[0];

            this.parameters.AddRange(contentLengthBytes);
            this.parameters.AddRange(Encoding.UTF8.GetBytes(content));

        }
    }
}

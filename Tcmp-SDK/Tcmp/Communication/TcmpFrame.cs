using System;
using System.Collections.Generic;
using System.Linq;

namespace TapTrack.Tcmp.Communication
{
    public class TcmpFrame
    {
        protected List<byte> contents;

        protected TcmpFrame()
        {
        }

        public int FrameLength
        {
            get
            {
                return contents.Count;
            }
        }

        public int Length
        {
            get
            {
                return Len1 * 256 + Len0;
            }
        }

        public byte Len1
        {
            get
            {
                if (contents != null)
                {
                    return contents[1];
                }
                else
                    return 0;
            }
        }

        public byte Len0
        {
            get
            {
                if (contents != null)
                    return contents[2];
                else
                    return 0;
            }
        }

        public byte CommandFamily1
        {
            get
            {
                return contents[4];
            }
        }

        public byte CommandFamily0
        {
            get
            {
                return contents[5];
            }
        }

        public byte[] CommandFamily
        {
            get
            {
                byte[] temp = new byte[2];
                Array.Copy(contents.ToArray(), 4, temp, 0, temp.Length);

                return temp;
            }
        }

        public byte Lcs
        {
            get
            {
                return contents[3];
            }
        }

        public byte[] Crc
        {
            get
            {
                byte[] crc = new byte[2];
                Array.Copy(contents.ToArray(), contents.Count - 3, crc, 0, 2);
                return crc;
            }
        }

        public byte[] Data
        {
            get
            {
                int length = this.Length - 5;
                if (length <= 0)
                    return null;

                byte[] data = new byte[length];
                Array.Copy(contents.ToArray(), 7, data, 0, length);
                return data;
            }
        }

        protected byte CalculateLcs(byte len1, byte len0)
        {
            return (byte)(0 - len1 - len0);
        }

        protected byte CalculateLcs(int length)
        {
            byte checksum = 0;

            foreach (byte b in BitConverter.GetBytes(length))
                checksum = (byte)(checksum - b);

            return checksum;
        }

        protected static bool HasLcsError(byte len1, byte len0, byte checksum)
        {
            if ((byte)(len1 + len0 + checksum) != 0)
                return true;
            else
                return false;
        }

        protected static byte[] CalculateCrc(List<byte> data)
        {
            int crc = 0x6363;

            for (int i = 0; i < data.Count; ++i)
                crc = UpdateCrc(crc, data[i]);

            return BitConverter.GetBytes((short)crc).Reverse().ToArray();
        }

        protected static byte[] CalculateCrc(byte[] data)
        {
            int crc = 0x6363;

            for (int i = 0; i < data.Length; ++i)
                crc = UpdateCrc(crc, data[i]);

            return BitConverter.GetBytes((short)crc).Reverse().ToArray();
        }

        protected static int UpdateCrc(int crc, byte b)
        {
            int tcrc = 0;
            int v = (crc ^ b) & 0xFF;

            for (int i = 0; i < 8; i++)
            {
                if (((tcrc ^ v) & 1) != 0)
                {
                    tcrc = (tcrc >> 1) ^ 0x8408;
                }
                else
                {
                    tcrc >>= 1;
                }
                v >>= 1;
            }

            return ((crc >> 8) ^ tcrc) & 0xFFFF;
        }

        protected List<byte> AddEscapeChars(List<byte> data)
        {
            List<byte> temp = new List<byte>();

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == 0x7E)
                {
                    temp.Add(0x7D);
                    temp.Add(0x5E);
                }
                else if (data[i] == 0x7D)
                {
                    temp.Add(0x7D);
                    temp.Add(0x5D);
                }
                else
                {
                    temp.Add(data[i]);
                }
            }

            return temp;
        }

        protected byte[] AddEscapeChars(params byte[] data)
        {
            List<byte> temp = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x7E)
                {
                    temp.Add(0x7D);
                    temp.Add(0x5E);
                }
                else if (data[i] == 0x7D)
                {
                    temp.Add(0x7D);
                    temp.Add(0x5D);
                }
                else
                {
                    temp.Add(data[i]);
                }
            }

            return temp.ToArray();
        }

        protected List<byte> RemoveEscapseCharacters(byte[] data)
        {
            List<byte> temp = new List<byte>(data);

            for (int i = 0; i < temp.Count - 2; i++)
            {
                if (temp[i] == 0x7D && temp[i + 1] == 0x5E)
                {
                    temp[i] = 0x7E;
                    temp.RemoveAt(i + 1);
                }
                else if (temp[i] == 0x7D && temp[i + 1] == 0x5D)
                {
                    temp[i] = 0x7D;
                    temp.RemoveAt(i + 1);
                }
            }

            return temp;
        }

        protected static bool HasCrcError(TcmpFrame frame, byte[] crc)
        {
            List<byte> data = new List<byte>(frame.ToArray());

            byte[] temp = CalculateCrc(data.GetRange(1, frame.FrameLength - 4));

            if (temp[0] != frame.Crc[0] || temp[1] != frame.Crc[1])
                return true;
            else
                return false;
        }

        public static bool IsValidFrame(TcmpFrame frame)
        {
            if (frame == null)
                return false;

            byte[] contents = frame.ToArray();

            if (contents.Length < 10)
                return false;

            if (contents[0] != 0x7E || contents[contents.Length - 1] != 0x7E)
                return false;

            if (HasLcsError(frame.Len1, frame.Len0, frame.Lcs))
                return false;

            if (frame.Length + 5 != contents.Length)
                return false;

            if (HasCrcError(frame, frame.Crc))
                return false;

            return true;
        }

        public byte[] ToArray()
        {
            return contents.ToArray();
        }

        public bool CompareCommandFamilies(byte[] other)
        {
            return Enumerable.SequenceEqual(CommandFamily, other);
        }
    }
}

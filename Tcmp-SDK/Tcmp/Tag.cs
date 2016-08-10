using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp
{
    /// <summary>
    /// Represents a NFC tag
    /// </summary>
    public class Tag
    {
        private byte typeOfTag;
        private byte[] uid;

        /// <summary>
        /// Get the tag information from response frame data
        /// </summary>
        /// <param name="frameData"></param>
        public Tag(byte[] frameData)
        {
            typeOfTag = frameData[0];
            uid = new byte[frameData.Length - 1];
            Array.Copy(frameData, 1, uid, 0, uid.Length);
        }

        public Tag(byte typeOfTag, byte[] uid)
        {
            this.typeOfTag = typeOfTag;
            this.uid = uid;
        }

        public Tag(byte[] uid, byte typeOfTag)
        {
            this.typeOfTag = typeOfTag;
            this.uid = uid;
        }

        public byte TypeOfTag { get { return typeOfTag; } }

        /// <summary>
        /// The UID of the tag
        /// </summary>
        public byte[] UID { get { return uid; } }

        /// <summary>
        /// Length of the UID
        /// </summary>
        public int Length { get { return uid.Length; } }

        public string UidToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (byte b in uid)
                builder.Append($"{b:X}".PadLeft(2, '0'));

            return builder.ToString();
        }

        public static string TypeLookUp(byte tagType)
        {
            if (tagType == 0x01)
            {
                return "Mifare Ultralight";
            }
            else if (tagType == 0x02)
            {
                return "NTAG 203";
            }
            else if (tagType == 0x03)
            {
                return "Mifare Ultralight C";
            }
            else if (tagType == 0x04)
            {
                return "Mifare Classic Standard - 1k";
            }
            else if (tagType == 0x05)
            {
                return "Mifare Classic Standard - 4k";
            }
            else if (tagType == 0x06)
            {
                return "Mifare DESFire EV1 2k";
            }
            else if (tagType == 0x07)
            {
                return "Generic NFC Forum Type 2 tag";
            }
            else if (tagType == 0x08)
            {
                return "Mifare Plus 2k CL2";
            }
            else if (tagType == 0x09)
            {
                return "Mifare Plus 4k CL2";
            }
            else if (tagType == 0x0A)
            {
                return "Mifare Mini";
            }
            else if (tagType == 0x0B)
            {
                return "Generic NFC Forum Type 4 tag";
            }
            else if (tagType == 0x0C)
            {
                return "Mifare DESFire EV1 4k";
            }
            else if (tagType == 0x0D)
            {
                return "Mifare DESFire EV1 8k";
            }
            else if (tagType == 0x0E)
            {
                return "Mifare DESFire - Unspecified model and capacity";
            }
            else if (tagType == 0x0F)
            {
                return "Topaz 512";
            }
            else if (tagType == 0x10)
            {
                return "NTAG 210";
            }
            else if (tagType == 0x11)
            {
                return "NTAG 212";
            }
            else if (tagType == 0x12)
            {
                return "NTAG 213";
            }
            else if (tagType == 0x13)
            {
                return "NTAG 215";
            }
            else if (tagType == 0x14)
            {
                return "NTAG 216";
            }
            else
            {
                return "Unknown tag";
            }
        }
    }
}

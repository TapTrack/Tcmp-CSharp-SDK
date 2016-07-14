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

        public Tag(byte[] frameData)
        {
            typeOfTag = frameData[0];
            uid = new byte[frameData.Length - 1];
            Array.Copy(frameData, 1, uid, 0, uid.Length);
        }

        public Tag(byte typeOfTag, byte[] serialNumber)
        {
            this.typeOfTag = typeOfTag;
            this.uid = serialNumber;
        }

        public Tag(byte[] serialNumber, byte typeOfTag)
        {
            this.typeOfTag = typeOfTag;
            this.uid = serialNumber;
        }

        public byte TypeOfTag { get { return typeOfTag; } }

        public byte[] UID { get { return uid; } }

        public int Length { get { return uid.Length; } }
    }
}

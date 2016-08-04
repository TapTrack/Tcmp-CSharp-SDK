using System.Collections.Generic;

namespace TapTrack.Ndef
{
    public class NdefUri
    {
        byte scheme;
        string path;

        public NdefUri(string uri)
        {
            this.path = string.Copy(uri);
            this.scheme = RemoveScheme(ref path);
        }

        public static readonly Dictionary<string, byte> BYTE_LOOKUP = new Dictionary<string, byte> {
            {"http://www.",0x01},
            {"https://www.",0x02},
            {"http://",0x03},
            {"https://",0x04},
            {"tel:",0x05},
            {"mailto:",0x06},
            {"ftp://anonymous:anonymous@",0x07},
            {"ftp://ftp.",0x08},
            {"ftps://",0x09},
            {"sftp://",0x0A},
            {"smb://",0x0B},
            {"nfs://",0x0C},
            {"ftp://",0x0D},
            {"dav://",0x0E},
            {"news:",0x0F},
            {"telnet://",0x10},
            {"imap:",0x11},
            {"rtsp://",0x12},
            {"urn:",0x13},
            {"pop:",0x14},
            {"sip:",0x15},
            {"sips:",0x16},
            {"tftp:",0x17},
            {"btspp://",0x18},
            {"btl2cap://",0x19},
            {"btgoep://",0x1A},
            {"tcpobex://",0x1B},
            {"irdaobex://",0x1C},
            {"file://",0x1D},
            {"urn:epc:id:",0x1E},
            {"urn:epc:tag:",0x1F},
            {"urn:epc:pat:",0x20},
            {"urn:epc:raw:",0x21},
            {"urn:epc:",0x22},
            {"urn:nfc:",0x23}
        };

        public static readonly Dictionary<byte, string> STRING_LOOKUP = new Dictionary<byte, string>() {
            {0x01,"http://www."},
            {0x02,"https://www"},
            {0x03,"http://"},
            {0x04,"https://"},
            {0x05,"tel:"},
            {0x06,"mailto:"},
            {0x07,"ftp://anony"},
            {0x08,"ftp://ftp."},
            {0x09,"ftps://"},
            {0x0A,"sftp://"},
            {0x0B,"smb://"},
            {0x0C,"nfs://"},
            {0x0D,"ftp://"},
            {0x0E,"dav://"},
            {0x0F,"news:"},
            {0x10,"telnet://"},
            {0x11,"imap:"},
            {0x12,"rtsp://"},
            {0x13,"urn:"},
            {0x14,"pop:"},
            {0x15,"sip:"},
            {0x16,"sips:"},
            {0x17,"tftp:"},
            {0x18,"btspp://"},
            {0x19,"btl2cap://"},
            {0x1A,"btgoep://"},
            {0x1B,"tcpobex://"},
            {0x1C,"irdaobex://"},
            {0x1D,"file://"},
            {0x1E,"urn:epc:id:"},
            {0x1F,"urn:epc:tag"},
            {0x20,"urn:epc:pat"},
            {0x21,"urn:epc:raw"},
            {0x22,"urn:epc:"},
            {0x23,"urn:nfc:"}
        };

        /// <summary>
        /// Will get the scheme also known as the NDEF URI code
        /// </summary>
        public byte Scheme
        {
            get
            {
                return scheme;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Removes the scheme from the uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>Byte code of the scheme</returns>
        public static byte RemoveScheme(ref string uri)
        {
            foreach (string prefix in BYTE_LOOKUP.Keys)
            {
                if (uri.StartsWith(prefix))
                {
                    uri = uri.Remove(0, prefix.Length);
                    return BYTE_LOOKUP[prefix];
                }

            }
            return 0x00;
        }

        /// <summary>
        /// Get the byte code of the scheme from a uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>Byte code of the scheme</returns>
        public static byte GetCode(string uri)
        {
            foreach (string prefix in BYTE_LOOKUP.Keys)
            {
                if (uri.StartsWith(prefix))
                    return BYTE_LOOKUP[prefix];
            }

            return 0x00;
        }

        public override string ToString()
        {
            if (scheme != 0)
                return STRING_LOOKUP[scheme] + path;
            else
                return path;
        }
    }
}

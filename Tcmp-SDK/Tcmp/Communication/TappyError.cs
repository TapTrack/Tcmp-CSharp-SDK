using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    public class TappyError
    {
        /// <summary>
        /// Converts an error code to a human readable error
        /// </summary>
        /// <param name="commandFamily">command family of frame</param>
        /// <param name="errorCode">Error code of an application frame</param>
        /// <returns></returns>
        public static string LookUp(byte[] commandFamily, byte errorCode)
        {
            if (commandFamily[0] == 0x00 && commandFamily[1] == 0x00)
            {
                if (errorCode == 0x01)
                    return "Invalid or improperly formatted message";
                if (errorCode == 0x02)
                    return "LCS (Length Checksum) error";
                if (errorCode == 0x03)
                    return "CRC error";
                if (errorCode == 0x04)
                    return "Bad length parameter";
                if (errorCode == 0x05)
                    return "Invalid parameter";
                if (errorCode == 0x06)
                    return "Unsupported command family";
                if (errorCode == 0x07)
                    return "Too few parameters";
            }
            if (commandFamily[0] == 0x00 && commandFamily[1] == 0x01)
            {
                if (errorCode == 0x01)
                    return "Invalid parameter";
                if (errorCode == 0x02)
                    return "Reserved for future use";
                if (errorCode == 0x03)
                    return "Polling error";
                if (errorCode == 0x04)
                    return "Two few parameters";
                if (errorCode == 0x05)
                    return "NDEF message is too big to fit on the tag";
                if (errorCode == 0x06)
                    return "Error creating NDEF content";
                if (errorCode == 0x07)
                    return "Error writing NDEF data to tag";
                if (errorCode == 0x08)
                    return "Error locking the tag";
                if (errorCode == 0x09)
                    return "Unsupported command code";
            }
            if (commandFamily[0] == 0x00 && commandFamily[1] == 0x03)
            {
                if (errorCode == 0x01)
                    return "Invalid parameters";
                if (errorCode == 0x02)
                    return "Too few parameters";
                if (errorCode == 0x03)
                    return "Too many parameters";
                if (errorCode == 0x04)
                    return "Polling error";
                if (errorCode == 0x05)
                    return "Tag read error";
                if (errorCode == 0x06)
                    return "Invalid block order (start block > end block)";
                if (errorCode == 0x07)
                    return "Authentication error";
                if (errorCode == 0x08)
                    return "Invalid block number (occurs if the tag is a 1k model and the block requested exceeds 0x3F)";
                if (errorCode == 0x09)
                    return "Invalid key number";
            }
            if (commandFamily[0] == 0x00 && commandFamily[1] == 0x04)
            {
                if (errorCode == 0x01)
                    return "Too few parameters";
                if (errorCode == 0x02)
                    return "Too many parameters";
                if (errorCode == 0x03)
                    return "Transceive error";
                if (errorCode == 0x04)
                    return "Invalid parameter";
                if (errorCode == 0x05)
                    return "No tag present";
                if (errorCode == 0x06)
                    return "NFC reader chip error";
            }
            return "Unknown error";
        }
    }
}

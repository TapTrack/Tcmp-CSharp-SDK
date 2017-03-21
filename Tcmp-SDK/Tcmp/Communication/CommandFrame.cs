using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TapTrack.Tcmp.CommandFamilies;

namespace TapTrack.Tcmp.Communication
{
    /// <summary>
    /// Represents a command frame that is sent to the Tappy
    /// </summary>
    public class CommandFrame : TcmpFrame
    {
        public CommandFrame(Command command) : base()
        {
            contents = new List<byte>();
            byte[] crc;

            // Add the length of the payload
            contents.AddRange(BitConverter.GetBytes((short)(command.Parameters.Length + 5)).Reverse()); 

            // Add the LCS
            contents.Add(CalculateLcs(command.Parameters.Length + 5));

            contents.AddRange(command.CommandFamily);

            contents.Add(command.CommandCode);

            contents.AddRange(command.Parameters);

            crc = CalculateCrc(contents);
            contents.AddRange(crc);

            contents = AddEscapeChars(contents);
     
            contents.Insert(0, 0x7E);
            contents.Add(0x7E);
        }
    }
}

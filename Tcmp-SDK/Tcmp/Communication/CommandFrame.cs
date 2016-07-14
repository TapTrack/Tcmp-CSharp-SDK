using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TapTrack.Tcmp.CommandFamilies;

namespace TapTrack.Tcmp.Communication
{
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

            contents = AddEscapeChars(contents);
            contents.AddRange(crc);
            contents.Insert(0, 0x7E);
            contents.Add(0x7E);
        }
    }
}

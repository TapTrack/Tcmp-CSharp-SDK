using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication
{
    internal class LackOfDataException : Exception
    {
        public LackOfDataException()
        {

        }

        public LackOfDataException(string message) : base(message)
        {

        }
    }
}

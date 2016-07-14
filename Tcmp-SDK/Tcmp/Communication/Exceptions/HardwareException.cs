using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication.Exceptions
{
    /// <summary>
    /// Caused when there is an issue with the connection to the Tappy device e.g there is no connection
    /// </summary>
    public class HardwareException : Exception
    {
        public HardwareException(string message) : base(message)
        {

        }
    }
}

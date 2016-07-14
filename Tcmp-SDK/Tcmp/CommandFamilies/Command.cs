using System.Collections.Generic;

namespace TapTrack.Tcmp.CommandFamilies
{
    public abstract class Command
    {
        protected List<byte> parameters;

        public Command()
        {
            parameters = new List<byte>();
        }

        public abstract byte CommandCode
        {
            get;
        }

        public abstract byte[] CommandFamily
        {
            get;
        }

        public byte[] Parameters
        {
            get
            {
                return parameters.ToArray();
            }
        }
    }
}

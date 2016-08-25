using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.CommandFamilies.System
{
    public enum TappySetting : byte
    {
        Type2TagIdentification = 0x01,
        EnableDataThrottling = 0x02
    }

    public class ConfigureSetting : SystemCommand
    {
        private const byte commandCode = 0x01;

        public ConfigureSetting(TappySetting setting, params byte[] parameters)
        {
            this.parameters.Add((byte)setting);
            this.parameters.AddRange(parameters);
        }

        public ConfigureSetting(byte setting, params byte[] parameters)
        {
            this.parameters.Add(setting);
            this.parameters.AddRange(parameters);
        }

        public override byte CommandCode
        {
            get
            {
                return commandCode;
            }
        }
    }
}

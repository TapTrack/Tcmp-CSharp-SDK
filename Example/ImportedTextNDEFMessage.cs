using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace TapTrack.Demo
{
    [DelimitedRecord(",")]
    class ImportedTextNDEFMessage
    {
        public string text { get; set; }
    }
}

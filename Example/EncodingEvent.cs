using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using System.Data.Linq.Mapping;

namespace TapTrack.Demo
{
    [DelimitedRecord(","), IgnoreFirst]
    [Table(Name = "encodingEvent")]

    class EncodingEvent
    {
        [Column(IsPrimaryKey = true)]
        public int id { get; set; }

        [Column]
        public string type { get; set; }

        [Column]
        public string data { get; set; }

        [Column]
        public string tagCode { get; set; }

        [Column]
        public DateTime encodedDate { get; set; }

        public EncodingEvent()
        {

        }
     
        public EncodingEvent(BatchNDEFType ndefType, string data, string tagCode, DateTime encodedDate)
        {
            if (ndefType == BatchNDEFType.TEXT)
            {
                this.type = "T";
            }
            else if (ndefType == BatchNDEFType.URI)
            {
                this.type = "U";
            }

            this.data = data;
            this.tagCode = tagCode;
            this.encodedDate = encodedDate;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using System.Data.Linq.Mapping;


namespace TapTrack.Demo
{

    [DelimitedRecord(",")]
    [Table(Name = "messageToEncode")]

    class BatchNDEF
    {
        [Column(IsPrimaryKey = true)]
        public int id { get; set; }

        [Column]
        public string type { get; set; }
      
        [Column]
        public string data { get; set; }

        [Column]
        public int encodedSuccessfully { get; set; }

        public BatchNDEF()
        {

        }

        public BatchNDEF(BatchNDEFType ndefType, string data)
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
            this.encodedSuccessfully = 0;
        }


    }
}

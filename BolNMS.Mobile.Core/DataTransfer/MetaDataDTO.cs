using System;
using System.Collections.Generic;
using System.Text;

namespace BolNMS.Mobile.Core.DataTransfer
{
    public class MetaDataDTO
    {
        public int MetaDataId { get; set; }
        public int MetaTypeId { get; set; }
        public string MetaName { get; set; }
        public string MetaValue { get; set; }
    }
}

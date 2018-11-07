using System;
using System.Collections.Generic;
using System.Text;

namespace BolNMS.Mobile.Core.DataTransfer
{
    public class NMSLoginDTO
    {
        public int UserId { get; set; }
        public List<MetaDataDTO> MetaData { get; set; }
        public string FullName { get; set; }
        public string SessionKey { get; set; }
    }
}

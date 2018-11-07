using System;
using System.Collections.Generic;
using System.Text;

namespace BolNMS.Mobile.Core.Models
{
    public class UserDeviceInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }

    }
}

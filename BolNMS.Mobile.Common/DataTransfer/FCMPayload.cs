using System;
using System.Collections.Generic;
using System.Text;

namespace BolNMS.Mobile.Common.DataTransfer
{
    public class FCMPayload
    {
        public string To { get; set; }
        public Notification Notification { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public bool content_available { get; set; }
        public string priorty { get; set; }
    }

    public class Notification
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string Icon { get; set; }

        public string Sound { get; set; }

    }

    public class UnSubscribeTopic
    {
        public string to { get; set; }
        public string[] registration_tokens { get; set; }
    }
    public class NotificationDTO
    {
        public string Title { get; set; }
        public string ClickUrl { get; set; }
        public string Body { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Route { get; set; }
        public string Sound { get; set; }
        public string FCMServerKey { get; set; }

    }
}


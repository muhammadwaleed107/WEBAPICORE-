using BolNMS.Mobile.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BolNMS.Mobile.Common.DataTransfer;

namespace BolNMS.Mobile.Api.Helper
{
    public static class FireBaseNotificationHelper
    {
        public static async Task<object> Subscribe(string deviceToken, string route, string fCMServerKey)
        {
            try
            {
                string url = string.Format("https://iid.googleapis.com/iid/v1/{0}/rel{1}", deviceToken, route);
                var client = new WebClient();
                client.Headers.Add("Authorization", "key=" + fCMServerKey);
                var a = await client.UploadStringTaskAsync(url, "");
                return client;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<object> UnSubscribe(string route, string fCMServerKey, params string[] deviceToken)
        {
            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };

                string url = string.Format("https://iid.googleapis.com/iid/v1:batchRemove");
                using (var client = new WebClient())
                {
                    UnSubscribeTopic data = new UnSubscribeTopic();
                    data.to = route;
                    data.registration_tokens = deviceToken;

                    client.Headers.Add("Authorization", "key=" + fCMServerKey);
                    var res = await client.UploadStringTaskAsync(url, JsonConvert.SerializeObject(data, jsonSerializerSettings));
                    return res;
                }

            }
            catch (WebException e)
            {
                throw e;
            }
        }
        public async static Task<object> SendNotificationToTopic(NotificationDTO notification)
        {
            Uri uri = new Uri("https://fcm.googleapis.com/fcm/send");
            var webclient = new WebClient();
            webclient.UseDefaultCredentials = true;
            webclient.Headers.Add("Authorization", "key=" + notification.FCMServerKey);

            webclient.Headers.Add("Content-Type", "application/json");
            webclient.Encoding = System.Text.Encoding.UTF8;

            FCMPayload data = new FCMPayload();
            data.To = notification.Route;
            data.content_available = true;
            data.priorty = "high";

            if (data.Notification == null)
                data.Notification = new Notification();

            data.Notification.Title = notification.Title;
            data.Notification.Body = notification.Description;
            data.Notification.Sound = notification.Sound;
            data.Notification.Icon = notification.Icon;
            data.Notification.Body = notification.Description;

            data.Data = new Dictionary<string, string>
            {
                    { "title", notification.Title },
                    { "text",  notification.Description },
                    { "click_action", notification.ClickUrl}
            };
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            var json = JsonConvert.SerializeObject(data, jsonSerializerSettings);

            var result = await webclient.UploadStringTaskAsync(uri, json);
            return result;
        }
    }



  

}

using BolNMS.Mobile.Core.DataTransfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Common
{
    public static class HelperUtility
    {

        public static string BolNMSErrorMessage = "BolNMSErrors";
        public static string ErrorMessage = "Something went wrong";
        public static string appSettings = "ApplicationSettings";
        public static async Task<List<UserInfo>> GetUserInfo()
        {
            string api = "http://10.3.12.120/api/admin/ModuleUsers/1";
            string responseBody = string.Empty;
            List<UserInfo> userIndo = new List<UserInfo>();
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(api);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }
            if (responseBody != null)
            {
                var data = JObject.Parse(responseBody);
                if (data != null)
                {
                    var dataSize = ((JArray)data["Data"]).Count;
                    for (int i = 0; i < dataSize; i++)
                    {
                        UserInfo userInfo = new UserInfo();
                        userInfo.UserId = Convert.ToInt32(data["Data"][i]["UserId"].ToString());
                        userInfo.UserName = data["Data"][i]["Fullname"].ToString();
                        userIndo.Add(userInfo);
                    }
                }
            }
            return userIndo;
        }

        public static async Task<NMSLoginDTO> GetNMSLoginDetails(string loginId, string password)
        {
            NMSLoginDTO loginDTO = new NMSLoginDTO();
            string api = "http://10.3.12.117/api/User/LoginMMS/";
            SigninDTO signin = new SigninDTO();
            signin.Login = loginId;
            signin.Password = password;
            string postJson = JsonConvert.SerializeObject(signin);
            StringContent content = new StringContent(postJson, Encoding.UTF8, "application/json");
            string responseBody = string.Empty;
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(api, content);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }
            if (responseBody != null)
            {
                var data = JObject.Parse(responseBody);
                if (data != null)
                {
                    if (Convert.ToBoolean(data["IsSuccess"].ToString()) == true && Convert.ToInt32(data["Data"]["UserId"].ToString()) > 0)
                    {
                        var dataSize = ((JArray)data["Data"]["MetaData"]).Count;

                        loginDTO.FullName = data["Data"]["FullName"].ToString();
                        loginDTO.UserId = Convert.ToInt32(data["Data"]["UserId"].ToString());
                        loginDTO.SessionKey = data["Data"]["SessionKey"].ToString();
                        List<MetaDataDTO> metaDataList = new List<MetaDataDTO>();
                        for (int i = 0; i < dataSize; i++)
                        {
                            MetaDataDTO metaData = new MetaDataDTO();
                            metaData.MetaDataId = Convert.ToInt32(data["Data"]["MetaData"][i]["MetaDataId"].ToString());
                            metaData.MetaName = data["Data"]["MetaData"][i]["MetaName"].ToString();
                            metaData.MetaTypeId = Convert.ToInt32(data["Data"]["MetaData"][i]["MetaTypeId"].ToString());
                            metaData.MetaValue = data["Data"]["MetaData"][i]["MetaValue"].ToString();
                            metaDataList.Add(metaData);
                        }
                        loginDTO.MetaData = metaDataList;
                    }
                }
            }
            return loginDTO;
        }
    }
}

using BolNMS.Mobile.Core.IRepository;
using BolNMS.Mobile.Core.IService;
using BolNMS.Mobile.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using BolNMS.Mobile.Common;
using BolNMS.Mobile.Common.ICache;
using BolNMS.Mobile.Core.DataTransfer;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace BolNMS.Mobile.Service
{
    public class NewsFileService:INewsFileService
    {
        private INewsFileRepository _newsfileRepository;
        private ICacheManager _cacheManager;
        private IConfiguration configuration;
        private IHttpContextAccessor httpContext;
        private string api;
        private string thumbURL;
        private string URL;
        private string mediaResourceUrl;
        private string imageUrl;

        public NewsFileService(INewsFileRepository newsfileRepository, ICacheManager cacheManager,
                               IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            this.mediaResourceUrl = configuration.GetSection("ApplicationSettings")["MediaResourceUrl"];
            this.httpContext = httpContext;
            this.api = "http://"+ this.httpContext.HttpContext.Request.Host.ToString()+this.mediaResourceUrl;

            this.thumbURL = configuration.GetSection("ApplicationSettings")["ThumbUrl"];
            this.URL = configuration.GetSection("ApplicationSettings")["ResourceUrl"];
            this.imageUrl = configuration.GetSection("ApplicationSettings")["NMSImageUrl"];

            this._newsfileRepository = newsfileRepository;
            this._cacheManager = cacheManager;
        }
        public async Task<List<NewsFile>> GetNewsFileByFilter(int categoryId, List<int> filters, int pageOffset, int pageSize,string searchtext)
        {

            //GetClaimsInfo("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIxMzk3IiwiRnVsbE5hbWUiOiJDb250cm9sbGVyIE91dHB1dCIsIlNlc3Npb25LZXkiOiJhMDlhMDAyYS04OWI5LTQ1ZDgtODU1OS1mMjcwNTUzNmYxZDAiLCJNZXRhRGF0YSI6Ilt7XCJNZXRhRGF0YUlkXCI6MTY5NyxcIk1ldGFUeXBlSWRcIjoxMixcIk1ldGFOYW1lXCI6XCJLYXJhY2hpXCIsXCJNZXRhVmFsdWVcIjpcIjFcIn0se1wiTWV0YURhdGFJZFwiOjIyMjAsXCJNZXRhVHlwZUlkXCI6MTYsXCJNZXRhTmFtZVwiOlwiUG9saXRpY3NcIixcIk1ldGFWYWx1ZVwiOlwiMVwifSx7XCJNZXRhRGF0YUlkXCI6MjI1OSxcIk1ldGFUeXBlSWRcIjoxNCxcIk1ldGFOYW1lXCI6XCJOTVNcIixcIk1ldGFWYWx1ZVwiOlwiMVwifSx7XCJNZXRhRGF0YUlkXCI6MjMxMyxcIk1ldGFUeXBlSWRcIjoxOSxcIk1ldGFOYW1lXCI6XCJIZWFkbGluZXNcIixcIk1ldGFWYWx1ZVwiOlwiNTJcIn0se1wiTWV0YURhdGFJZFwiOjIzMTQsXCJNZXRhVHlwZUlkXCI6MTksXCJNZXRhTmFtZVwiOlwiMyBNZWluIDE1XCIsXCJNZXRhVmFsdWVcIjpcIjUzXCJ9LHtcIk1ldGFEYXRhSWRcIjoyMzE1LFwiTWV0YVR5cGVJZFwiOjE5LFwiTWV0YU5hbWVcIjpcIkxhZ2F0YWFyIDEwXCIsXCJNZXRhVmFsdWVcIjpcIjU0XCJ9XSIsImV4cCI6MTU0Mjk4MDczMSwiaXNzIjoiYm9sTk1TIiwiYXVkIjoiaHR0cDovLzUyLjU4LjE0Ni4xNTI6ODkifQ.E_3GOWso8lM483N9OK9_RjtMfY_LjqJLR8EOPje9GZ0");

            var newsFile = await _newsfileRepository.GetNewsFileWithFilterFromTo(categoryId, filters, pageOffset, pageSize, searchtext);
            if (newsFile != null)
            {
                var newsFileIds = newsFile.Select(x => x.NewsFileId).ToList();
                var resources = await _newsfileRepository.GetResources(newsFileIds);
                var userInfo = _cacheManager.GetUserInfo();
                var createdByIds = newsFile.Select(x => x.CreatedById).ToList();
                var selectedUserInfo = userInfo.Where(x => createdByIds.Contains(x.UserId));
                if (resources != null)
                {
                    for (int i = 0; i < newsFile.Count; i++)
                    {
                        var guid = resources.
                            Where(x => x.NewsFileId == newsFile[i].NewsFileId)
                            .Select(x => x.GUID)
                            .FirstOrDefault() ?? string.Empty;

                        if (guid != string.Empty)
                        {
                            newsFile[i].ImageURL = imageUrl + URL + guid;

                            newsFile[i].ImageThumbURL = imageUrl + thumbURL + guid;
                        }

                        newsFile[i].URLs = resources.
                            Where(x => x.NewsFileId == newsFile[i].NewsFileId).
                            Select(x => string.Concat(api + URL, x.GUID) ).
                            ToList();

                        // newsFile[i].URLs = newsFile[i].URLs.Select(r => string.Concat(api + URL, r)).ToList();

                    }
                }
                if (selectedUserInfo != null)
                {
                    for (int i = 0; i < newsFile.Count; i++)
                    {
                        newsFile[i].CreatedBy = selectedUserInfo.Where(x => x.UserId == newsFile[i].CreatedById).Select(x => x.UserName).FirstOrDefault();
                    }
                }
            }
            return newsFile;
        }

        public async Task<List<string>> SearchKeyword(string searchtext, int count)
        {
            var newsFile = await _newsfileRepository.Search(searchtext, count);
            return newsFile;
        }

        private NMSLoginDTO GetClaimsInfo(string jwtToken)
        {
            NMSLoginDTO loginInfo = new NMSLoginDTO();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtInput = jwtToken;

            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if (readableToken != true)
            {
                throw new Exception("Invalid Token");
            }
            if (readableToken == true)
            {
                var jwtSecurityToken = jwtHandler.ReadJwtToken(jwtInput);
                var claims = jwtSecurityToken.Claims;
                loginInfo.FullName = claims.Where(x => x.Type == "FullName").Select(x => x.Value).FirstOrDefault();
                loginInfo.UserId = Convert.ToInt32(claims.Where(x => x.Type == "UserId").Select(x => x.Value).FirstOrDefault());
                loginInfo.SessionKey = claims.Where(x => x.Type == "SessionKey").Select(x => x.Value).FirstOrDefault();
                var data= claims.Where(x => x.Type == "MetaData").Select(x => x.Value).FirstOrDefault();
                var metaData= JsonConvert.DeserializeObject<List<MetaDataDTO>>(data);
                loginInfo.MetaData = metaData;
            }

            return loginInfo;
        }
    }
}

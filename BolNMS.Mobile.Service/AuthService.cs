using BolNMS.Mobile.Common;
using BolNMS.Mobile.Core.DataTransfer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using BolNMS.Mobile.Core.IService;
using Newtonsoft.Json;
using System.Net;
using BolNMS.Mobile.Core.IRepository;

namespace BolNMS.Mobile.Service
{
    public class AuthService: IAuthService
    {
        private IConfiguration _configuration;
        private INewsFileRepository _newsFileRepository;
        private int year = 365;
        private string appSettings = "ApplicationSettings";

        public AuthService(IConfiguration configuration,INewsFileRepository newsFileRepository)
        {
            this._configuration = configuration;
            this._newsFileRepository = newsFileRepository;
        }

        public async Task<UserLoginDTO> Login(SigninDTO signinDTO)
        {
            var loginDetails = await HelperUtility.GetNMSLoginDetails(signinDTO.Login,signinDTO.Password);

            var claims = new Claim[] 
            {
                new Claim("UserId", loginDetails.UserId.ToString()),
                new Claim("FullName", loginDetails.FullName),
                new Claim("SessionKey", loginDetails.SessionKey),
                new Claim("MetaData", JsonConvert.SerializeObject(loginDetails.MetaData))
            };

            UserLoginDTO userLoginDTO = new UserLoginDTO();
            userLoginDTO.accessToken = await CreateToken(claims);
            return userLoginDTO;
        }
        public async Task<UserLoginDTO> SubscribeDeviceToken(string deviceToken)
        {
            string BroadCastRoute = _configuration.GetSection(appSettings)["NMSAPPServerKey"];
            string serverAPIKey = _configuration.GetSection(appSettings)["NMSAPPBroadCastRoute"];

            string url = string.Format("https://iid.googleapis.com/iid/v1/{0}/rel{1}", deviceToken, BroadCastRoute);
            var client = new WebClient();
            client.Headers.Add("Authorization", "key=" + serverAPIKey);
            await client.UploadStringTaskAsync(url, "");
            InsertDeviceInNMS(deviceToken, "", "");
            UserLoginDTO userLoginDTO = new UserLoginDTO();
            return userLoginDTO;
        }
        public void InsertDeviceInNMS(string deviceToken, string device, string loggedInBy)
        {
            _newsFileRepository.InsertDeviceInNMS(deviceToken, device, loggedInBy);

        }

        private async Task<string> CreateToken(Claim[] claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                    (
                    _configuration.GetSection(appSettings)["JWTSecurityKey"]
                    )
                    );

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.GetSection(appSettings)["Issuer"],
            audience: _configuration.GetSection(appSettings)["Audience"],
            expires: DateTime.UtcNow.AddDays(year),
            signingCredentials: signingCredentials,
            claims: claims
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return await Task.FromResult(accessToken);
        }

    }
}

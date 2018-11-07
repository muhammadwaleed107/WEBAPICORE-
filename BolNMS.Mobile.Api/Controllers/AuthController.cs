using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BolNMS.Mobile.Core.IService;
using BolNMS.Mobile.Common.DataTransfer;
using BolNMS.Mobile.Core.DataTransfer;

namespace BolNMS.Mobile.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            this._authService = authService;
        }

        [HttpPost("login")]
        public async Task<DataTransferObject<UserLoginDTO>> Login([FromBody]SigninDTO signinDTO)
        {
            DataTransferObject<UserLoginDTO> response = new DataTransferObject<UserLoginDTO>();
            response.IsSuccess = true;
            try
            {
                response.Data = await _authService.Login(signinDTO);
            }
            catch (Exception exp)
            {
                response.IsSuccess = false;
                response.Message = exp.Message;
            }
            return response;
        }

        [HttpPost("Subscribe")]
        public async Task<DataTransferObject<UserLoginDTO>> SubscribeDeviceToken(string token)
        {
            DataTransferObject<UserLoginDTO> response = new DataTransferObject<UserLoginDTO>();
            response.IsSuccess = true;
            try
            {
                response.Data = await _authService.SubscribeDeviceToken(token);
            }
            catch (Exception exp)
            {
                response.IsSuccess = false;
                response.Message = exp.Message;
            }
            return response;
        }
    }
}

using BolNMS.Mobile.Core.DataTransfer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Core.IService
{
    public interface IAuthService
    {
        Task<UserLoginDTO> Login(SigninDTO signinDTO);
        Task<UserLoginDTO> SubscribeDeviceToken(string token);
        
    }
}

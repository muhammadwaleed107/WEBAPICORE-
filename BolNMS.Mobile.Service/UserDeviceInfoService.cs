using BolNMS.Mobile.Core.IRepository;
using BolNMS.Mobile.Core.IService;
using BolNMS.Mobile.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Service
{
    public class UserDeviceInfoService : IUserDeviceInfoService
    {
        private ICategoryRepository _categoryRepository;
        private IUserDeviceInfoRepository _userDeviceInfoRepository;

        public UserDeviceInfoService(ICategoryRepository categoryRepository, IUserDeviceInfoRepository userDeviceInfoRepository)
        {
            this._categoryRepository = categoryRepository;
            this._userDeviceInfoRepository = userDeviceInfoRepository;
        }
        public async Task<bool> RegisterDevice(UserDeviceInfo input)
        {
            if (input == null)
                throw new ApplicationException("Invalid Object");

            if (string.IsNullOrEmpty(input.UserId))
                throw new ApplicationException("UserId not found");

            if (string.IsNullOrEmpty(input.DeviceId))
                throw new ApplicationException("DeviceId not found");

            if (string.IsNullOrEmpty(input.DeviceToken))
                throw new ApplicationException("DeviceToken not found");
            
            var responce = await _userDeviceInfoRepository.RegisterDevice(input);
           
            return responce;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BolNMS.Mobile.Common.DataTransfer;
using BolNMS.Mobile.Core.IService;
using BolNMS.Mobile.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BolNMS.Mobile.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Notification")]
    public class NotificationController : Controller
    {
        private IFilterService _filterService;
        private ICategoryService _categoryService;
        private INewsFileService _newsFileService;
        private IUserDeviceInfoService _userDeviceInfoService;
        private IConfiguration _configuration;
        private string appSettings = "ApplicationSettings";

        public NotificationController(IFilterService filterService, ICategoryService categoryService, INewsFileService newsFileService, IUserDeviceInfoService userDeviceInfoService, IConfiguration configuration)
        {
            this._filterService = filterService;
            this._categoryService = categoryService;
            this._newsFileService = newsFileService;
            this._userDeviceInfoService = userDeviceInfoService;
            this._configuration = configuration;
        }

        [HttpPost("[action]")]
        public async Task<DataTransferObject<object>> RegisterDevice([FromBody]UserDeviceInfo input)
        {
            DataTransferObject<object> response = new DataTransferObject<object>();

            if (input == null)
                throw new ApplicationException("Invalid object");

            var IsSave = await _userDeviceInfoService.RegisterDevice(input);
            if (IsSave)
            {
                string serverAPIKey = _configuration.GetSection(appSettings)["NMSAPPServerKey"];
                string BroadCastRoute = _configuration.GetSection(appSettings)["NMSAPPBroadCastRoute"];

                if (!string.IsNullOrEmpty(input.CategoryName))
                {
                    BroadCastRoute = input.CategoryName;
                }

                response.Data = await Helper.FireBaseNotificationHelper.Subscribe(input.DeviceToken, BroadCastRoute, serverAPIKey);
            }

            response.IsSuccess = true;
            return response;
        }
        [HttpPost("[action]")]
        public async Task<DataTransfer<object>> SendNotification([FromBody]NotificationDTO input)
        {
            DataTransfer<object> response = new DataTransfer<object>();

            string serverAPIKey = _configuration.GetSection(appSettings)["NMSAPPServerKey"];
            string BroadCastRoute = _configuration.GetSection(appSettings)["NMSAPPBroadCastRoute"];
            string Icon = _configuration.GetSection(appSettings)["Icon"];


            input.ClickUrl = "";
            input.FCMServerKey = serverAPIKey;
            input.Icon = Icon;
            input.Route = BroadCastRoute;
            input.Sound = "default";
            
            response.Data = await Helper.FireBaseNotificationHelper.SendNotificationToTopic(input);

            return response;
        }
    }
}

using BolNMS.Mobile.Core.DataTransfer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Common.ICache
{
    public interface ICacheManager
    {
        List<UserInfo> GetUserInfo();
    }
}

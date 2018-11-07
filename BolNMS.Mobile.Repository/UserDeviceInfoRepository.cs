using BolNMS.Mobile.Core.IRepository;
using BolNMS.Mobile.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Repository
{
    public class UserDeviceInfoRepository : Repository, IUserDeviceInfoRepository
    {
        private string connectionString;
        private IConfiguration _configuration;

        public UserDeviceInfoRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.connectionString = this._configuration.GetConnectionString("BolNMSConnectionString");
        }
        public async Task<bool> RegisterDevice(UserDeviceInfo input)
        {
            bool IsSuccess = true;
            string sql = "RegisterDevice";
            List<SqlParameter> parameters = new List<SqlParameter>() {
                 new SqlParameter("@DeviceId", input.DeviceId),
                 new SqlParameter("@DeviceToken", input.DeviceToken),
                 new SqlParameter("@UserId", input.UserId),
            };

            var list = await ExecuteNonQuery(connectionString, System.Data.CommandType.StoredProcedure, sql, parameters.ToArray());

            return IsSuccess;

        }
        public async Task<UserDeviceInfo> GetUserDevice()
        {
            string sql = @" select TOP(1) * from [dbo].[UserDeviceInfo] WHERE ISACTIVE = 1";
            var list = await ExecuteDataSet(connectionString, System.Data.CommandType.Text, UserDeviceInfoFromDataRow, sql);
            return list[0];
        }
        private UserDeviceInfo UserDeviceInfoFromDataRow(SqlDataReader reader)
        {
            UserDeviceInfo UserDeviceInfo = new UserDeviceInfo();
            UserDeviceInfo.UserId = (string)reader["UserId"];
            UserDeviceInfo.DeviceId = (string)reader["DeviceId"];
            UserDeviceInfo.DeviceToken = (string)reader["CategoryId"];

            return UserDeviceInfo;
        }
    }
}

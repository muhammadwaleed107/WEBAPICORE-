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
    public class FilterRepository:Repository, IFilterRepository
    {

        private string connectionString;
        private string appSettings = "ApplicationSettings";
        private IConfiguration _configuration;

        public FilterRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.connectionString = this._configuration.GetConnectionString("BolNMSConnectionString");
        }

        public async Task<List<Filter>> GetAllFilters()
        {
            List<int> filterTypeIds = new List<int>();
            filterTypeIds.Add(1);
            filterTypeIds.Add(3);
            filterTypeIds.Add(16);
            filterTypeIds.Add(7);
            filterTypeIds.Add(9);
            filterTypeIds.Add(21);
            filterTypeIds.Add(10);

            string sql =        @"
                                SELECT [FilterId]
                                  ,[Name]
                                  ,[CreationDate]
                              FROM [Filter]
                              WHERE ParentId IS NULL
                             AND [FilterTypeId] IN ( "+string.Join(",", filterTypeIds)+ " )";

            var list = await ExecuteDataSet(connectionString, System.Data.CommandType.Text, FilterFromDataRow, sql);
            list.Add(new Filter { FilterId = 0, Name = "Last24Hours" });
            return list;
        }

        public async Task<List<Filter>> GetAllCategories()
        {
            string sql = @"
                             SELECT [FilterId]
                                  ,[Name]
                                  ,[CreationDate]
                              FROM [Filter]
                              WHERE ParentId IS NULL";

            var list = await ExecuteDataSet(connectionString, System.Data.CommandType.Text, FilterFromDataRow, sql);

            return list;
        }


        

        private Filter FilterFromDataRow(SqlDataReader reader)
        {
            Filter newsLive = new Filter
            {
                FilterId = Convert.ToInt32(reader["FilterId"]),
                Name = reader["Name"].ToString(),
            };
            return newsLive;
        }
    }
}

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
    public class CategoryRepository:Repository, ICategoryRepository
    {

        private string connectionString;
        private IConfiguration _configuration;

        public CategoryRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.connectionString = this._configuration.GetConnectionString("BolNMSConnectionString");
        }
        public async Task<List<Category>> GetAllCategories()
        {
            string sql = @"
                        SELECT [CategoryId]
                              ,[Category]
                              ,[ParentId]
                              ,[CreationDate]
                              ,[IsActive]
                          FROM [Category]
                          WHERE ParentId IS NULL";
            var list = await ExecuteDataSet(connectionString, System.Data.CommandType.Text, CategoryFromDataRow, sql);
            return list;
        }

        private Category CategoryFromDataRow(SqlDataReader reader)
        {
            Category newsLive = new Category
            {
                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                CategoryName = reader["Category"].ToString(),
                ParentId = Convert.ToInt32(reader["ParentId"].ToString() == string.Empty ? "0" : reader["ParentId"].ToString()),
                CreationDate = Convert.ToDateTime(reader["CreationDate"].ToString() != string.Empty ? reader["CreationDate"].ToString() : DateTime.UtcNow.ToString()),
            };
            return newsLive;
        }
    }

}

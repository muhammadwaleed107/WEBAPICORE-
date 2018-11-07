using BolNMS.Mobile.Core.DataTransfer;
using BolNMS.Mobile.Core.IRepository;
using BolNMS.Mobile.Core.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;


namespace BolNMS.Mobile.Repository
{
    public class NewsFileRepository : Repository, INewsFileRepository
    {
        private string connectionString;
        private IConfiguration _configuration;


        public NewsFileRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.connectionString = this._configuration.GetConnectionString("BolNMSConnectionString");
        }

        public async Task<bool> InsertDeviceInNMS(string deviceToken, string device, string loggedInBy)
        {
            bool IsSuccess = true;
            string sql = string.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>() {
                 new SqlParameter("@TokenNumber", deviceToken),
                 new SqlParameter("@Device", device),
                 new SqlParameter("@loggedInBy", loggedInBy),
                 new SqlParameter("@LoggedInTime", DateTime.UtcNow)
            };

            sql += "INSERT INTO [dbo].[LoggedInDevice] ([TokenNumber],[Device],[LoggedInTime],[LoggedInBy]) VALUES (@TokenNumber ,@Device,@loggedInBy,@LoggedInTime);";
            var list = await ExecuteDataSet(connectionString, System.Data.CommandType.Text, NewsFileFromDataRow, sql, parameters.ToArray());

            return IsSuccess;

        }

        public async Task<List<NewsFile>> GetNewsFileWithFilterFromTo(int categoryId, List<int> filters, int pageOffset, int pageSize, string searchText)
        {

            string sql = string.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>() {
                 new SqlParameter("@PageOffset", pageOffset),
                 new SqlParameter("@PageSize", pageSize)
            };
            string queryfor24hours = string.Empty;

            if (filters!=null && filters.Contains(0))
            {
                filters.Remove(0);
                queryfor24hours = " and DATEDIFF(D,nf.CreationDate,GETDATE()) = 0";
            }


            sql += "select fd.Text,fd.DescriptionText,nf.*,ct.Category as CategoryName,ct.CategoryId as CategoryId,l.location from newsfile nf " +
               " left join location l on nf.ReporterBureauId = l.locationid left join Category ct on ct.CategoryId = nf.CategoryId" +
               " left join filedetail fd on fd.newsfileid = nf.newsfileid " +
               " where nf.newsfileid in ((select NewsFileId from NewsFileFilter where FilterId in (" + string.Join(",", filters.ToArray()) + ")))"
               + (string.IsNullOrEmpty(queryfor24hours) ? "" : queryfor24hours)
               + (string.IsNullOrEmpty(searchText) ? "" : " and nf.SearchableText like '%" + searchText + "%'") + " and nf.IsDeleted = 0 "
               + " order by nf.CreationDate desc " +
               " OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY;";


            /********
            if (categoryId == -1)
            {
                if (filters == null || filters.Count == 0)
                {
                    sql += "select fd.Text,fd.DescriptionText,nf.*,ct.Category as CategoryName,ct.CategoryId as CategoryId,l.location from newsfile nf " +
                    " left join location l on nf.ReporterBureauId = l.locationid left join Category ct on ct.CategoryId = nf.CategoryId" +
                    " left join filedetail fd on fd.newsfileid = nf.newsfileid " +
                    " where nf.parentid is not null and nf.IsDeleted = 0 " + (string.IsNullOrEmpty(searchText) ? "" : " and nf.SearchableText like '%" + searchText + "%'")
                    + (string.IsNullOrEmpty(queryfor24hours) ? "" : queryfor24hours)
                    + " order by nf.CreationDate desc " +
                    " OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY;";
                }
                else
                {
                    sql += "select fd.Text,fd.DescriptionText,nf.*,ct.Category as CategoryName,ct.CategoryId as CategoryId,l.location from newsfile nf " +
                    " left join location l on nf.ReporterBureauId = l.locationid left join Category ct on ct.CategoryId = nf.CategoryId" +
                    " left join filedetail fd on fd.newsfileid = nf.newsfileid " +
                    " where nf.newsfileid in (select distinct NewsFileId from (select NewsFileId from NewsFileFilter where FilterId in (" + string.Join(",", filters.ToArray()) + ")) NewsFileFilter)"
                    + (string.IsNullOrEmpty(queryfor24hours) ? "" : queryfor24hours)
                    + (string.IsNullOrEmpty(searchText) ? "" : " and nf.SearchableText like '%" + searchText + "%'") + " and nf.parentid is not null and nf.IsDeleted = 0 "
                    + " order by nf.CreationDate desc " +
                    " OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY;";
                }
            }
            else
            {
                parameters.Add(new SqlParameter("@CategoryId", categoryId));

                if (filters == null || filters.Count == 0)
                {
                    sql += "select fd.Text,fd.DescriptionText,nf.*,ct.Category as CategoryName,ct.CategoryId as CategoryId,l.location from newsfile nf " +
                    " left join location l on nf.ReporterBureauId = l.locationid left join Category ct on ct.CategoryId = nf.CategoryId" +
                    " left join filedetail fd on fd.newsfileid = nf.newsfileid " +
                    " where ct.CategoryId=@CategoryId and nf.parentid is not null and nf.IsDeleted = 0" + (string.IsNullOrEmpty(searchText) ? "" : " and nf.SearchableText like '%" + searchText + "%'")
                     + (string.IsNullOrEmpty(queryfor24hours) ? "" : queryfor24hours)
                    + " order by nf.CreationDate desc " +
                    " OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY;";
                }
                else
                {
                    sql += "select fd.Text,fd.DescriptionText,nf.*,ct.Category as CategoryName,ct.CategoryId as CategoryId,l.location from newsfile nf " +
                    " left join location l on nf.ReporterBureauId = l.locationid left join Category ct on ct.CategoryId = nf.CategoryId" +
                    " left join filedetail fd on fd.newsfileid = nf.newsfileid " +
                    " where ct.CategoryId=@CategoryId and nf.newsfileid in (select distinct NewsFileId from (select NewsFileId from NewsFileFilter where FilterId in (" + string.Join(",", filters.ToArray()) + ")) NewsFileFilter) and nf.parentid is not null and nf.IsDeleted = 0 "
                   + (string.IsNullOrEmpty(queryfor24hours) ? "" : queryfor24hours)
                    + (string.IsNullOrEmpty(searchText) ? "" : " and nf.SearchableText like '%" + searchText + "%'")
                    + " order by nf.CreationDate desc " +
                    " OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY;";
                }

            }

            **********/

            var list = await ExecuteDataSet(connectionString, System.Data.CommandType.Text, NewsFileFromDataRow, sql, parameters.ToArray());

            return list;
        }


        private NewsFile NewsFileFromDataRow(SqlDataReader reader)
        {

            NewsFile newsLive = new NewsFile();
            newsLive.NewsFileId = Convert.ToInt32(reader["NewsFileId"]);
            newsLive.Title = reader["Title"].ToString();
            newsLive.CategoryId = Convert.ToInt32(reader["CategoryId"].ToString() == string.Empty ? "0" : reader["CategoryId"].ToString());
            newsLive.CategoryName = reader["CategoryName"].ToString();
            newsLive.NewsStatus = Convert.ToInt32(reader["NewsStatus"].ToString() == string.Empty ? "0" : reader["NewsStatus"].ToString());
            newsLive.IsVerified = Convert.ToBoolean(reader["IsVerified"].ToString() == string.Empty ? false : reader["IsVerified"]);
            newsLive.Source = reader["Source"].ToString();
            newsLive.LanguageCode = reader["LanguageCode"].ToString();
            newsLive.CreatedById = Convert.ToInt32(reader["CreatedBy"].ToString() == string.Empty ? "0" : reader["CreatedBy"].ToString());
            newsLive.CreationDate = Convert.ToDateTime(reader["CreationDate"].ToString() != string.Empty ? reader["CreationDate"].ToString() : DateTime.UtcNow.ToString()).ToString("yyyy-MM-ddTHH:mm:ss.sssZ");
            newsLive.Text = reader["Text"].ToString();
            newsLive.DescriptionText = reader["DescriptionText"].ToString();
            newsLive.Slug = reader["Slug"].ToString();

            return newsLive;
        }
        public async Task<List<NewsFileResource>> GetResources(List<int> newsFileIds)
        {
            string sql = @"select NewsFileId,Guid from FileResource
                           Where NewsFileId in  ( " + string.Join(",", newsFileIds.ToArray()) + " )";
            var list = await ExecuteDataSet(connectionString, System.Data.CommandType.Text, ResourceFromDataRow, sql);

            return list;
        }
        private NewsFileResource ResourceFromDataRow(SqlDataReader reader)
        {
            NewsFileResource newsFileResource  = new NewsFileResource
            {
                NewsFileId = Convert.ToInt32(reader["NewsFileId"]),
                GUID = reader["Guid"].ToString(),
            };
            return newsFileResource;
        }
        public async Task<List<string>> Search(string search, int count)
        {
            List<string> emptylist = new List<string>();
            string sql = string.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>() {
                 new SqlParameter("@SearchKeyword", search),
                   new SqlParameter("@count", count)
            };

            if(!string.IsNullOrEmpty(search))
            {
                sql += "select top (@count) nf.* from newsfile nf where nf.IsDeleted = 0 and nf.SearchableText like '%" + search + "%'";

                var list = await ExecuteDataSet(connectionString, System.Data.CommandType.Text, SearchKeywordDataRow, sql, parameters.ToArray());

                return list;
            }
            return emptylist;
        }

        private string SearchKeywordDataRow(SqlDataReader reader)
        {
            return reader["Title"].ToString();
        }

    }
}

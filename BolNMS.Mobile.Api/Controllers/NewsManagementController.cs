using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BolNMS.Mobile.Core.IService;
using BolNMS.Mobile.Common.DataTransfer;
using BolNMS.Mobile.Core.Models;
using BolNMS.Mobile.Core.IRepository;
using BolNMS.Mobile.Core.DataTransfer;
using Microsoft.Extensions.Configuration;

namespace BolNMS.Mobile.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/NewsManagement")]
    public class NewsManagementController : Controller
    {
        private IFilterService _filterService;
        private ICategoryService _categoryService;
        private INewsFileService _newsFileService;
        private IUserDeviceInfoService _userDeviceInfoService;
        private IConfiguration _configuration;
        private string appSettings = "ApplicationSettings";

        public NewsManagementController(IFilterService filterService, ICategoryService categoryService, INewsFileService newsFileService, IUserDeviceInfoService userDeviceInfoService, IConfiguration configuration)
        {
            this._filterService = filterService;
            this._categoryService = categoryService;
            this._newsFileService = newsFileService;
            this._userDeviceInfoService = userDeviceInfoService;
            this._configuration = configuration;
        }

        [HttpGet("[action]")]
        public async Task<DataTransferObject<List<Category>>> GetAllCategories()
        {
            DataTransferObject<List<Category>> response = new DataTransferObject<List<Category>>();
            response.IsSuccess = true;
            response.Data = await _categoryService.GetAllCategories();
            return response;
        }


        [HttpGet("[action]")]
        public async Task<DataTransferObject<List<Filter>>> GetAllFilters()
        {
            DataTransferObject<List<Filter>> response = new DataTransferObject<List<Filter>>();
            response.IsSuccess = true;
            response.Data = await _filterService.GetAllFilters();
            return response;
        }

        [HttpGet("[action]")]
        public async Task<DataTransferObject<InitialDataDTO>> LoadInitialData()
        {
            DataTransferObject<InitialDataDTO> response = new DataTransferObject<InitialDataDTO>();
            response.IsSuccess = true;
            try
            {
                response.Data = new InitialDataDTO();
                response.Data.Categories = new List<Filter>();
                response.Data.Categories = await _filterService.GetAllCategories() ?? new List<Filter>();

                response.Data.Filters = new List<Filter>();
                response.Data.Filters = await _filterService.GetAllFilters() ?? new List<Filter>();
            }
            catch (Exception ex) {
                response.IsSuccess = false;
            }

            return response;
        }

        [HttpPost("[action]")]
        public async Task<DataTransferObject<List<NewsFile>>> GetNewsFileByFilter([FromBody] NewsFileFilterDTO newsFileFilter)
        {
            DataTransferObject<List<NewsFile>> response = new DataTransferObject<List<NewsFile>>();
            response.IsSuccess = true;
            response.Data = await _newsFileService.GetNewsFileByFilter(newsFileFilter.CategoryId, newsFileFilter.Filters, newsFileFilter.PageOffset, newsFileFilter.PageSize, newsFileFilter.SearchText) ?? new List<NewsFile>();
            return response;
        }

        
        [HttpGet("[action]")]
        public async Task<DataTransferObject<List<string>>> SearchKeyword(string searchtext, int count = 30)
        {
            DataTransferObject<List<string>> response = new DataTransferObject<List<string>>();
            response.IsSuccess = true;
            response.Data = await _newsFileService.SearchKeyword(searchtext, count);
            return response;
        }


        

    }
}
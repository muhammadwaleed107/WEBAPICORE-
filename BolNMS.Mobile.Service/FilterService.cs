using BolNMS.Mobile.Core.IRepository;
using BolNMS.Mobile.Core.IService;
using BolNMS.Mobile.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Service
{
    public class FilterService: IFilterService
    {
        private IFilterRepository _filterRepository;
        public FilterService(IFilterRepository filterRepository)
        {
            this._filterRepository = filterRepository;
        }
        public async Task<List<Filter>> GetAllFilters()
        {
            var filters = await _filterRepository.GetAllFilters();
            return filters;
        }

        public async Task<List<Filter>> GetAllCategories()
        {
            var filters = await _filterRepository.GetAllCategories();
            return filters;
        }
    }
}

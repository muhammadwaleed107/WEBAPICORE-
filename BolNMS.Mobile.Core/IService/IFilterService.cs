using BolNMS.Mobile.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Core.IService
{
    public interface IFilterService
    {
        Task<List<Filter>> GetAllFilters();
        Task<List<Filter>> GetAllCategories();
    }
}

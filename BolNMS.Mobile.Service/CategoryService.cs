using BolNMS.Mobile.Core.IRepository;
using BolNMS.Mobile.Core.IService;
using BolNMS.Mobile.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Service
{
    public class CategoryService: ICategoryService
    {
        private ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }
        public async Task<List<Category>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategories();
            return categories;
        }
    }
}

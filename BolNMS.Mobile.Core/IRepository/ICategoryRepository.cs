using BolNMS.Mobile.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Core.IRepository
{
   public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategories();
    }
}

using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.IServices
{
    public interface ICategoryService : IBaseService<Category, CategoryModel>
    {
       Task<CategoryModel> UpdateCategory(int id, CategoryModel model); 
    }
}

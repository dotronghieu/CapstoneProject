﻿using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.IServices
{
    public interface IPhotoCategoryService
    {
        List<PhotoModel> GetPhotoByCategory(string catName, int photoId);
        List<CategoryModel> GetCategoryByPhoto(int id);
        
    }
}

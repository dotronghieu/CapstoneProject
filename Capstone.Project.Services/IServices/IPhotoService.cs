using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.IServices
{
    public interface IPhotoService : IBaseService<Photo, PhotoModel>
    {
       IEnumerable<PhotoModelGetAll> GetRandomPhoto();
        IEnumerable<PhotoModelGetAll> GetRandomPhoto2();
        PhotoModel UpdatePhoto(int id, PhotoModel model);

    }
}

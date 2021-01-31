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
        PhotoModel UpdatePhoto(int id, PhotoModel model);

        void EncryptAllPhoto();
    }
}

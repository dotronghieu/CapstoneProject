using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.IServices
{
    public interface IPhotoUploadDownloadService : IBaseService<Photo, PhotoCreateModel>
    {
        Task<Photo> CreatePhoto(PhotoCreateModel model);
        string DownloadPhoto(int id);
        Task<bool> DeletePhoto(int id);

    }
}

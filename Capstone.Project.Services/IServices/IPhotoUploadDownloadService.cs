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
        Task<string> DownloadPhoto(int id, string userId);
        Task<bool> DeletePhoto(int id);
        Task<PhotoModel> DeleteOrDisablePhoto(int photoId);
        Task<Photo> ChangeWaterMarkPhoto(int photoId);
    }
}

using Capstone.Project.Data.Helper;
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
        Task<PhotoModel> GetPhotoById(int id);
        Task<PhotoModel> GetById(int id);
        void EncryptAllPhoto();
        double percentage(string hash1, string hash2);
        IEnumerable<PhotoModelAdmin> GetPhotoNotApproved();
        (IEnumerable<PhotoModelGetAll>,int) SearchPhoto(string key, int pageSize, int pageNumber);
        bool CheckBoughtPhoto(int id, string userId);
        bool CheckMyPhoto(int photoId, string userId);
        Task<PhotoModel> GetSimilarPhoto(int photoId);
        IEnumerable<PhotoModel> GetPhotoByUser(String userId);
        double CompareHash(ulong hash1, ulong hash2);
    }

}
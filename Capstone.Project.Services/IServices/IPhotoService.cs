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
        Task<PhotoEditViewModel> UpdatePhoto(int id, PhotoEditViewModel model);
        Task<PhotoModel> GetPhotoById(int id);
        Task<PhotoModel> GetById(int id);
        void EncryptAllPhoto();
        double percentage(string hash1, string hash2);
        Task<IEnumerable<PhotoModelAdmin>> GetPhotoNotApprovedAsync();
        (IEnumerable<PhotoModelGetAll>,int) SearchPhoto(string key, int pageSize, int pageNumber);
        bool CheckBoughtPhoto(int id, string userId);
        bool CheckMyPhoto(int photoId, string userId);
        List<PhotoSimilarViewModel> GetSimilarPhoto(List<int> listPhotoId);
        IEnumerable<PhotoModel> GetPhotoByUser(String userId);
        double CompareTwoHash(ulong hash1, ulong hash2);
        IEnumerable<PhotoModelGetAll> GetAllNormalPHoto();
        IEnumerable<PhotoModelGetAll> GetAllExclusivePhoto();
        Task<bool> EnablePhoto(int id);
        Task<bool> AddToCart(int photoId);
        Task<bool> MotifyIsBought(int photoId);
        
    }

}
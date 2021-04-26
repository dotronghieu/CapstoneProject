using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Helper.HashAlgorithms;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.Repository;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.Services
{
    public class PhotoService : BaseService<Photo, PhotoModel>, IPhotoService
    {
        private readonly CapstoneProjectContext _context;
        public PhotoService(IUnitOfWork unitOfWork, IMapper mapper, CapstoneProjectContext capstoneProjectContext) : base(unitOfWork, mapper)
        {
            _context = capstoneProjectContext;
        }
        protected override IGenericRepository<Photo> _reponsitory => _unitOfWork.PhotoRepository;

        public void EncryptAllPhoto()
        {
            var encryptCode = _unitOfWork.UsersRepository.GetById(_reponsitory.GetById(2).Result.UserId).Result.EncryptCode;
            var photoList = _unitOfWork.PhotoRepository.GetAll().ToList();
            foreach (var item in photoList)
            {
                string linkAfterEncrypt = Encryption.StringCipher.Encrypt(item.Link, encryptCode);
                item.Link = linkAfterEncrypt;
                _unitOfWork.PhotoRepository.Update(item);
                _unitOfWork.SaveAsync();

            }
            
        }

        public async Task<PhotoModel> GetPhotoById(int id)
        {
            var photo = _reponsitory.GetById(id).Result;
            if (photo != null)
            {
                var user =  await  _unitOfWork.UsersRepository.GetById(photo.UserId);
                var decryptLink = Encryption.StringCipher.Decrypt(photo.Link, user.EncryptCode);
                photo.Link = decryptLink;
                return _mapper.Map<PhotoModel>(photo);
            }
            return null;
        }

     
        public  IEnumerable<PhotoModelAdmin> GetPhotoNotApproved()
        {
            PhotoCategoryService service = new PhotoCategoryService();
            var list =  _reponsitory.GetByObject(p => p.DelFlg == false && p.ApproveStatus == Constants.Const.PHOTO_STATUS_PENDING).OrderBy(c => c.PhotoId);
            if (list != null)
            {
                List<PhotoModelAdmin> resultList = new List<PhotoModelAdmin>();
                foreach (var item in list)
                {
                    var checkEdit = _unitOfWork.PhotoEditRepository.GetById(item.PhotoId).Result;
                    if(checkEdit != null)
                    {
                        item.PhotoName = checkEdit.PhotoName;
                        item.Description = checkEdit.Description;
                        item.Price = checkEdit.Price;
                    }
                    var categoryList = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.PhotoId == item.PhotoId, includeProperties: "Category").ToList();
                    List<CategoryModel> categoryResult = new List<CategoryModel>();
                    foreach (var item1 in categoryList)
                    {
                        categoryResult.Add(_mapper.Map<CategoryModel>(item1.Category));
                    }
                    PhotoModelAdmin photo = _mapper.Map<PhotoModelAdmin>(item);
                    photo.Category = categoryResult;
                    photo.SimilarPhoto = GetSimilarPhoto2(photo.PhotoId);
                    resultList.Add(photo);
                }
                return resultList;
            }
            return null;
        }
        public IEnumerable<PhotoModelGetAll> GetRandomPhoto()
        {
            List<PhotoModelGetAll> resultList = new List<PhotoModelGetAll>();
            var list = _unitOfWork.PhotoRepository.GetByObject(c => c.DelFlg == false && c.DisableFlg == false && c.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED).OrderBy(c => Guid.NewGuid()).Take(Constants.Const.NUMBER_OF_PHOTO_HOMEPAGE);
            if (list != null)
            {
                foreach (var item in list)
                {
                    var resultObject = _mapper.Map<PhotoModelGetAll>(item);
                    resultObject.UserName = _unitOfWork.UserGenRepository.GetFirst(u => u.UserId == item.UserId).Result.Username;
                    resultList.Add(resultObject);
                }
                return resultList.AsEnumerable<PhotoModelGetAll>();
            }
            return null;
        }

        public (IEnumerable<PhotoModelGetAll>,int) SearchPhoto(string key, int pageSize, int pageNumber)
        {
            List<PhotoModelGetAll> resultList = new List<PhotoModelGetAll>();
            var list1 = _unitOfWork.PhotoRepository.GetByObject(c => c.PhotoName.Contains(key) && c.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && c.DelFlg == false && c.DisableFlg == false);
            var list2 = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.Category.CategoryName.Contains(key), includeProperties: "Photo").ToList();
            if (list1 != null)
            {

                foreach (var item in list1)
                {
                    resultList.Add(_mapper.Map<PhotoModelGetAll>(item));
                }
            }
            if (list2 != null)
            {
                foreach (var item in list2)
                {
                    //var photo = _unitOfWork.PhotoRepository.GetById(item.PhotoId).Result;
                    var photo = item.Photo;
                    var flag = true;
                    foreach (var result in resultList)
                    {
                        if (result.PhotoId == photo.PhotoId)
                        {
                            flag = false;
                        }
                    }
                    if (flag && (photo.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED) && (photo.DelFlg == false) && (photo.DisableFlg == false))
                    {
                        resultList.Add(_mapper.Map<PhotoModelGetAll>(photo));
                    }

                }
            }
            int  total = resultList.Count();
            if (resultList != null)
            {
                return (resultList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), total);
            }
            return (null,0);
        }

        public PhotoEditViewModel UpdatePhoto(int id, PhotoEditViewModel model) 
        {
            var entity = new PhotoEdit
            {
                Description = model.Description,
                PhotoId = model.PhotoId,
                PhotoName = model.PhotoName,
                Price = model.Price
            };
            var photoEntity = _reponsitory.GetById(id).Result;
            photoEntity.ApproveStatus = Constants.Const.PHOTO_STATUS_PENDING;
            _unitOfWork.PhotoRepository.Update(photoEntity);
            _unitOfWork.SaveAsync();
            _unitOfWork.PhotoEditRepository.Add(entity);
            _unitOfWork.SaveAsync();
            return _mapper.Map<PhotoEditViewModel>(entity);
        }

        public IEnumerable<PhotoModel> GetPhotoByUser(string userId)
        {
            var listPhoto =  _reponsitory.GetByObject(c => c.DelFlg == false && c.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && c.UserId == userId).ToList();
            List<PhotoModel> result = new List<PhotoModel>();
            foreach (var item in listPhoto)
            {
                result.Add(_mapper.Map<PhotoModel>(item));
            }
            return result;
        }
        public async Task<PhotoModel> GetById(int id)
        {     
                var photo = await _reponsitory.GetFirst(c => c.DelFlg == false && c.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && c.PhotoId == id);
                if(photo != null)
                {
                    List<CategoryModel> categoryList = this.GetCategoryByPhoto(id);
                    var photoResult = _mapper.Map<PhotoModel>(photo);
                    photoResult.UserName = _unitOfWork.UsersRepository.GetById(photo.UserId).Result.Username;
                    photoResult.Category = categoryList;
                    return photoResult;
                }          
            return null;
        }
        public List<CategoryModel> GetCategoryByPhoto(int id)
        {
            var list = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.PhotoId == id, includeProperties: "Category").ToList();
            if (list != null)
            {
                List<CategoryModel> result = new List<CategoryModel>();
                foreach (var item in list)
                {
                    var category = item.Category;
                    result.Add(_mapper.Map<CategoryModel>(category));

                }
                return result;
            }
            return null;
        }

        public double percentage(string hash1, string hash2)
        {
            return NewPerceptualHash.CalcSimilarDegree(hash1, hash2);
        }
        private PhotoModel GetSimilarPhoto2(int photoId)
        {
            var photo = _reponsitory.GetById(photoId).Result;
            var listAllPhotoInDb = _reponsitory.GetByObject(p => p.DelFlg == false &&
             p.ApproveStatus != Constants.Const.PHOTO_STATUS_DENIED &&
             p.ApproveStatus != Constants.Const.PHOTO_STATUS_PENDING &&
             p.PhotoId != photoId).ToList();
            double maxSimilar = 0;
            var photoSimilar = new Photo();
            foreach (var item in listAllPhotoInDb)
            {
                var percentage = CompareHash.Similarity(Convert.ToUInt64(photo.Phash), Convert.ToUInt64(item2.Phash));

                if (percentage >= 80)
                {
                    maxSimilar = percentage;
                    if (percentage >= maxSimilar)
                    {
                        photoSimilar = item;
                    }
                }
            }
            if(photoSimilar.PhotoId > 0)
            {
                return _mapper.Map<PhotoModel>(photoSimilar);
            }
            return null;
        }
        public List<PhotoSimilarViewModel> GetSimilarPhoto(List<int> listPhotoId)
        {
            var listResult = new List<PhotoSimilarViewModel>();
            foreach (var item in listPhotoId)
            {
                var photo = _reponsitory.GetById(item).Result;
                var listAllPhotoInDb = _reponsitory.GetByObject(p => p.DelFlg == false &&
                p.ApproveStatus != Constants.Const.PHOTO_STATUS_DENIED && 
                p.ApproveStatus != Constants.Const.PHOTO_STATUS_PENDING && 
                p.PhotoId != item).ToList();
                double maxSimilar = 0;
                var photoSimilar = new Photo();
                foreach (var item2 in listAllPhotoInDb)
                {
                    var percentage = CompareHash.Similarity(Convert.ToUInt64(photo.Phash), Convert.ToUInt64(item2.Phash));
                    
                    if(percentage >= 80)
                    {
                        maxSimilar = percentage;
                        if(percentage >= maxSimilar)
                        {
                            photoSimilar = item2;
                        }
                    }
                }
                var photoResult = _mapper.Map<PhotoSimilarViewModel>(photo);
                if(photoSimilar.PhotoId > 0)
                {
                    photoResult.SimilarPhoto = _mapper.Map<PhotoModel>(photoSimilar);
                }
                listResult.Add(photoResult);
            }
            return listResult;
        }

        public bool CheckBoughtPhoto(int id, string userId)
        {
            var orderList =  _unitOfWork.OrdersRepository.GetByObject(c => c.UserId == userId, includeProperties: "OrderDetails").ToList();
            if (orderList != null)
            {
                List<PhotoTransactionModel> result = new List<PhotoTransactionModel>();
                foreach (var order in orderList)
                {
                    var orderDetailList = order.OrderDetails;
                    foreach (var orderDetail in orderDetailList)
                    {
                        if (orderDetail.PhotoId == id)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CheckMyPhoto(int photoId, string userId)
        {
            var photo = _unitOfWork.PhotoRepository.GetById(photoId);
            if (photo != null)
            {
                if (photo.Result.UserId == userId)
                {
                    return true;
                }
            }
            return false;
        }

        public double CompareTwoHash(ulong hash1, ulong hash2)
        {
            return CompareHash.Similarity(hash1, hash2);
        }

        public IEnumerable<PhotoModelGetAll> GetAllNormalPHoto()
        {
            var result = new List<PhotoModelGetAll>();
            var listPhotoNormal = _unitOfWork.PhotoRepository.GetByObject(c => c.DelFlg == false &&
            c.DisableFlg == false &&
            c.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED &&
            c.TypeId == 1);
            foreach (var p in listPhotoNormal)
            {
                var photoResult = _mapper.Map<PhotoModelGetAll>(p);
                photoResult.Category = GetCategoryByPhoto(p.PhotoId);
                result.Add(photoResult);
            }
            if(result.Count >= 1)
            {
                return result;
            }
            return null;
        }
        public IEnumerable<PhotoModelGetAll> GetAllExclusivePhoto()
        {
            var result = new List<PhotoModelGetAll>();
            var listPhotoNormal = _unitOfWork.PhotoRepository.GetByObject(c => c.DelFlg == false &&
            c.DisableFlg == false &&
            c.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED &&
            c.TypeId == 2);
            foreach (var p in listPhotoNormal)
            {
                var photoResult = _mapper.Map<PhotoModelGetAll>(p);
                photoResult.Category = GetCategoryByPhoto(p.PhotoId);
                result.Add(photoResult);
            }
            if (result.Count >= 1)
            {
                return result;
            }
            return null;
        }
    }
}
    
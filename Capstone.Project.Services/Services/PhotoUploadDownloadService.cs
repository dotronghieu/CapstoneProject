using AutoMapper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.Repository;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capstone.Project.Services.Services
{
    public class PhotoUploadDownloadService : BaseService<Photo, PhotoCreateModel>, IPhotoUploadDownloadService
    {
        private string ApiKey = "AIzaSyApqJz6VCYuRvXjhB4c5QS65rva1TWFDu4";
        private string Bucket = "image-cd56a.appspot.com";
        private string AuthEmail = "asdf@gmail.com";
        private string AuthPassword = "asdf123";
        private IWebHostEnvironment _environment;

        public PhotoUploadDownloadService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment environment) : base(unitOfWork, mapper)
        {
            _environment = environment;
        }

        protected override IGenericRepository<Photo> _reponsitory => _unitOfWork.PhotoRepository;

        public async Task<Photo> CreatePhoto(PhotoCreateModel model)
        {
            var file = model.File;
            FileStream stream = null;
            if (file.Length > 0)
            {
                string folder = "firebaseFiles";
                string path = Path.Combine(_environment.WebRootPath, $"images/{folder}");

                if (Directory.Exists(path))
                {
                    using (stream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    stream = new FileStream(Path.Combine(path, file.FileName), FileMode.Open);

                }
                else
                {
                    Directory.CreateDirectory(path);
                }

                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                    })
                    .Child("images")
                    .Child($"{file.FileName}")
                    .PutAsync(stream, cancellation.Token);

                try
                {
                    // error during upload will be thrown when you await the task
                    string link = await task;
                    stream.Dispose();
                    string imgdel = Path.Combine(path, file.FileName);
                    System.IO.File.Delete(imgdel);
                    Photo photo = new Photo();
                    photo.PhotoName = model.PhotoName;
                    photo.Link = link;
                    photo.Price = model.Price;
                    photo.TypeId = model.TypeId;
                    photo.UserId = model.UserId;
                    photo.DelFlg = false;
                    // add to DB
                    _reponsitory.Add(photo);
                    await _unitOfWork.SaveAsync();
                    return photo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception_UploadPhotoService: {0}", ex);
                }
            }
            return null;
        }

        public string DownloadPhoto(int id)
        {
            var photo = _reponsitory.GetById(id);
            return photo.Result.Link;
        }
    }
}

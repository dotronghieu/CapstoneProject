using AutoMapper;
using Capstone.Project.Data.Helper;
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
using System.Drawing;
using System.Drawing.Imaging;
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
                string path = Path.Combine(_environment.ContentRootPath, $"firebaseimages/{folder}");

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

                DateTime dateTime = DateTime.Now;
                string date = dateTime.ToString();
                date = date.Replace("/", "");
                date = date.Replace(":", "");

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
                    .Child(date+$"{file.FileName}")
                    .PutAsync(stream, cancellation.Token);

                try
                {
                    // error during upload will be thrown when you await the task
                    string link = await task;
                    string link1;
                    var user = await _unitOfWork.UsersRepository.GetById(model.UserId);
                    //watermark link
                    FileStream wm = new FileStream(Path.Combine(path, "WM" + file.FileName), FileMode.Create);
                    using (var img = Image.FromStream(stream))
                    {
                        using (var graphic = Graphics.FromImage(img))
                        {
                            string username = "Imago - " + user.FullName;
                            int fontSize;
                            if (img.Width < img.Height)
                            { fontSize = img.Height; }
                            else { fontSize = img.Width; }
                            var font = new Font(FontFamily.GenericSansSerif, fontSize / 35, FontStyle.Bold, GraphicsUnit.Pixel);
                            var color = Color.FromArgb(150, 0, 0, 0);
                            var brush = new SolidBrush(color);
                            double angle;
                            if (img.Width <= img.Height) { angle = 180 - Math.Atan2(img.Width, img.Height) * 180; }
                            else { angle = 180 - Math.Atan2(img.Width, img.Height) * 180; }
                            if (angle < 0) { angle = -angle; }
                            graphic.RotateTransform((float)angle);//xoay chiều watermark
                            var point = new Point((int)(img.Width * 0.1f), (int)(img.Height * -0.1f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.5f), (int)(img.Height * -0.1f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.9f), (int)(img.Height * -0.1f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.3f), (int)(img.Height * -0.3f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.7f), (int)(img.Height * -0.3f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.1f), (int)(img.Height * 0.1f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.5f), (int)(img.Height * 0.1f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.9f), (int)(img.Height * 0.1f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.3f), (int)(img.Height * 0.3f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.7f), (int)(img.Height * 0.3f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.1f), (int)(img.Height * 0.5f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.5f), (int)(img.Height * 0.5f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.9f), (int)(img.Height * 0.5f));
                            graphic.DrawString(username, font, brush, point);
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.3f), (int)(img.Height * 0.7f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.7f), (int)(img.Height * 0.7f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.1f), (int)(img.Height * -0.5f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.5f), (int)(img.Height * -0.5f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.9f), (int)(img.Height * -0.5f));
                            graphic.DrawString(username, font, brush, point);
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.3f), (int)(img.Height * -0.7f));
                            graphic.DrawString(username, font, brush, point);
                            point = new Point((int)(img.Width * 0.7f), (int)(img.Height * -0.7f));
                            graphic.DrawString(username, font, brush, point);
                            ImageFormat imageFormat = GetImageFormat(file.FileName);
                            img.Save(wm, imageFormat);
                        }
                    }
                    wm.Dispose();
                    wm = new FileStream(Path.Combine(path, "WM" + file.FileName), FileMode.Open);

                    var task1 = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                    })
                        .Child("vmimages")
                        .Child("WM" + date + file.FileName)
                        .PutAsync(wm);
                    try
                    {
                        link1 = await task1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception_UploadPhotoService_WM: {0}", ex);
                        return null;
                    }

                    stream.Dispose();
                    string imgdel = Path.Combine(path, file.FileName);
                    System.IO.File.Delete(imgdel);
                    wm.Dispose();
                    imgdel = Path.Combine(path, "WM" + file.FileName);
                    System.IO.File.Delete(imgdel);
                    Photo photo = new Photo();
                    photo.PhotoName = model.PhotoName;
                    photo.Link = Encryption.StringCipher.Encrypt(link, user.EncryptCode);
                    photo.Wmlink = link1;
                    photo.Price = model.Price;
                    photo.TypeId = model.TypeId;
                    photo.UserId = model.UserId;
                    photo.DelFlg = false;
                    photo.Description = model.Description;
                    photo.ApproveStatus = Constants.Const.PHOTO_STATUS_PENDING;
                    // add to DB
                    _reponsitory.Add(photo);
                    await _unitOfWork.SaveAsync();

                    var lastphoto = await _unitOfWork.PhotoRepository.GetFirst(c => c.Wmlink == link1);

                    PhotoCategory photoCategory = new PhotoCategory();
                    photoCategory.PhotoId = lastphoto.PhotoId;
                    photoCategory.CategoryId = model.ListCategory[0];

                    _unitOfWork.PhotoCategoryRepository.Add(photoCategory);
                    await _unitOfWork.SaveAsync();
                    if (model.ListCategory.Count == 2)
                    {
                        PhotoCategory photoCategory1 = new PhotoCategory();

                        photoCategory1.PhotoId = lastphoto.PhotoId;
                        photoCategory1.CategoryId = model.ListCategory[1];

                        _unitOfWork.PhotoCategoryRepository.Add(photoCategory1);

                        await _unitOfWork.SaveAsync();
                    }


                    return photo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception_UploadPhotoService: {0}", ex);
                }
            }
            return null;
        }

        public async Task<string> DownloadPhoto(int id)
        {
            var photo = _reponsitory.GetById(id);
            var user = await _unitOfWork.UsersRepository.GetById(photo.Result.UserId);
            string link = Encryption.StringCipher.Decrypt(photo.Result.Link, user.EncryptCode);
            return link;
        }

        public async Task<bool> DeletePhoto(int id)
        {
            var photo = _reponsitory.GetById(id);
            string link = photo.Result.Wmlink;
            int length = link.Length - 53 - 79;
            link = link.Substring(79, length);
            link = link.Replace("%20", " ");
            string link1 = photo.Result.Link;
            int length1 = link.Length - 53 - 79;
            link1 = link1.Substring(79, length1);
            link1 = link1.Replace("%20", " ");
            FirebaseStorage task = new FirebaseStorage(Bucket);
            try
            {
                await task.Child("images").Child(link).DeleteAsync();
                await task.Child("images").Child(link1).DeleteAsync();
                photo.Result.Link = "";
                photo.Result.Wmlink = "";
                photo.Result.DelFlg = true;
                _reponsitory.Update(photo.Result);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
            }
            return false;
        }

        private ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}

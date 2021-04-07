using AutoMapper;
using Capstone.Project.Data.Models;
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
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<TransactionModel> GetAllTransactionByUserId(string userId)
        {
            var transactionList = new List<TransactionModel>(); 
            var orderList = _unitOfWork.OrdersRepository.GetByObject(o => o.UserId == userId, includeProperties: "Transaction").OrderByDescending(c => c.InsDateTime).ToList();
            if(orderList.Count > 0)
            {
                foreach (var item in orderList)
                {
                    transactionList.Add(_mapper.Map<TransactionModel>(item.Transaction));
                }
                return transactionList;
            }
            return null;
        }

        public TransactionIdModel GetTransaction(string transactionId)
        {
            TransactionIdModel model = new TransactionIdModel();
            List<PhotoModel> photoList = new List<PhotoModel>();
            decimal? total = 0;
            var orderList = _unitOfWork.OrdersRepository.GetByObject(c => c.TransactionId == transactionId, includeProperties: "OrderDetails").ToList();
            if (orderList.Count > 0)
            {
                foreach (var order in orderList)
                {
                    var orderDetail = order.OrderDetails;
                    foreach (var item in orderDetail)
                    { 
                        var photo = _unitOfWork.PhotoRepository.GetById(item.PhotoId);
                        photo.Result.Price = item.Price;
                        photoList.Add(_mapper.Map<PhotoModel>(photo.Result));
                        total = total + item.Price;
                    }
                }
                model.photo = photoList;
                model.total = total;
                return model;
            }
            return null;
        }

      
    }
}

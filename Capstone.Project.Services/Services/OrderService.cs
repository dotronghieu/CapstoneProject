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
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PhotoModel>> GetUserBoughtPhoto(string id)
        {
            var orderList = _unitOfWork.OrdersRepository.GetByObject(f => f.UserId == id, includeProperties: "OrderDetails").ToList();
            if(orderList != null)
            {
                List<PhotoModel> result = new List<PhotoModel>();
                foreach (var order in orderList)
                {
                    var orderDetailList = order.OrderDetails;
                    foreach (var orderDetail in orderDetailList)
                    {
                        result.Add(_mapper.Map<PhotoModel>(await _unitOfWork.PhotoRepository.GetById(orderDetail.PhotoId)));
                    }
                }
                return result.AsEnumerable<PhotoModel>();
            }
            return null;
        }

        public async Task<Order> OrderPhoto(OrderModel orderModel)
        {
            Order order = null;
            Transaction transaction = null;
            if (orderModel != null)
            {
                transaction = new Transaction
                {
                    TransactionId = orderModel.TransactionId,
                    Amount = orderModel.Amount,
                    CreateTime = DateTime.Parse(orderModel.CreateTime),
                    PayerId = orderModel.PayerId,
                    PayerPaypalEmail = orderModel.PayerPaypalEmail
                };
                _unitOfWork.TransactionRepository.Add(transaction);
                order = new Order
                {
                    OrderId = Guid.NewGuid().ToString(),
                    UserId = orderModel.UserId,
                    InsDateTime = DateTime.Now,
                    TransactionId = orderModel.TransactionId
                };
                _unitOfWork.OrdersRepository.Add(order);
                foreach (int item in orderModel.ListPhotoId)
                {
                    var orderDetail = new OrderDetail()
                    {
                        OrderId = order.OrderId,
                        PhotoId = item,
                        Price = _unitOfWork.PhotoRepository.GetById(item).Result.Price,
                        PaymentFlag = false
                    };
                    _unitOfWork.OrderDetailRepository.Add(orderDetail);
                }
                await _unitOfWork.SaveAsync();
            }
            return _mapper.Map<Order>(order);
        }
    }
}

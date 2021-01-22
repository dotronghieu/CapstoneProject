using AutoMapper;
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

        public IEnumerable<PhotoModel> GetUserBoughtPhoto(string id)
        {
            var orderList = _unitOfWork.OrdersRepository.GetByObject(f => f.UserId == id).ToList();
            if(orderList != null)
            {
                List<PhotoModel> result = new List<PhotoModel>();
                foreach (var order in orderList)
                {
                    var orderDetailList = order.OrderDetails;
                    foreach (var orderDetail in orderDetailList)
                    {
                        result.Add(_mapper.Map<PhotoModel>(_unitOfWork.PhotoRepository.GetById(orderDetail.PhotoId)));
                    }
                }
                return result.AsEnumerable<PhotoModel>();
            }
            return null;
        }
    }
}

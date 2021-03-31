using AutoMapper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Capstone.Project.Data.Helper;

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

        public async Task<IEnumerable<PhotoTransactionModel>> GetUserBoughtPhoto(string id)
        {
            var orderList = _unitOfWork.OrdersRepository.GetByObject(f => f.UserId == id, includeProperties: "OrderDetails").ToList();
            if(orderList != null)
            {
                List<PhotoTransactionModel> result = new List<PhotoTransactionModel>();
                foreach (var order in orderList)
                {
                    var orderDetailList = order.OrderDetails;
                    foreach (var orderDetail in orderDetailList)
                    {
                        var model = _mapper.Map<PhotoTransactionModel>(await _unitOfWork.PhotoRepository.GetById(orderDetail.PhotoId));
                        model.BoughtPrice = orderDetail.Price;
                        model.BoughtTime = order.InsDateTime;
                        model.TransactionId = GetTransactionIDByOrderID(orderDetail.OrderId);
                        result.Add(model);
                    }
                }
                return result.AsEnumerable<PhotoTransactionModel>();
            }
            return null;
        }

        private string GetTransactionIDByOrderID(string orderId)
        {
            var order = _unitOfWork.OrdersRepository.GetFirst(o => o.OrderId == orderId).Result;
            return order.TransactionId;
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
            var user = await _unitOfWork.UserGenRepository.GetById(orderModel.UserId);
            if (user != null)
            {
                if (orderModel.ProofId != null)
                {
                    var verifyUrl = "http://localhost:8081/#/changeforgotpassword?userId=" + user.UserId;
                    var fromMail = new MailAddress(Constants.Const.IMAGO_EMAIL, "Imago (No Reply)");
                    var toMail = new MailAddress(user.Email);
                    var imagoPassword = Constants.Const.IMAGO_EMAIL_PASSWORD;
                    string subject = "Transaction Success";
                    string body = "<br/><br/>Hi " + user.FullName +
                      "<br/><br/>Your transaction was successful. You have purchased an exclusive photo for $" + orderModel.Amount + 
                      " at " + DateTime.Parse(orderModel.CreateTime).Kind.ToString() + "." + 
                      "<br/><br/>This is your proofid: " + orderModel.ProofId +
                      "<br/><br/>This is proof that you can look up your transaction. You must keep it carefully!" +
                      "<br/><br/>Thank you for choosing Imago, we hope to hear from you again soon!" + 
                      "<br/><br/>Sincerely" +
                      "<br/>Imago";


                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromMail.Address, imagoPassword)

                    };
                    using (var message = new MailMessage(fromMail, toMail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                    smtp.Send(message);
                } else
                {
                    var verifyUrl = "http://localhost:8081/#/changeforgotpassword?userId=" + user.UserId;
                    var fromMail = new MailAddress(Constants.Const.IMAGO_EMAIL, "Imago (No Reply)");
                    var toMail = new MailAddress(user.Email);
                    var imagoPassword = Constants.Const.IMAGO_EMAIL_PASSWORD;
                    string subject = "Transaction Success";
                    string body = "<br/><br/>Hi " + user.FullName +
                      "<br/><br/>Your transaction was successful. You have purchased " + orderModel.ListPhotoId.Count +
                      " photo for $" + orderModel.Amount + " at " + DateTime.Parse(orderModel.CreateTime).Kind.ToString() + "." + 
                      "<br/><br/>Thank you for choosing Imago, we hope to hear from you again soon!" +
                      "<br/><br/>Sincerely" +
                      "<br/>Imago";


                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromMail.Address, imagoPassword)

                    };
                    using (var message = new MailMessage(fromMail, toMail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                        smtp.Send(message);
                }
                
            }
            return _mapper.Map<Order>(order);
        }
    }
}

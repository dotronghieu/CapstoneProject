﻿using AutoMapper;
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
            var orderList = _unitOfWork.OrdersRepository.GetByObject(o => o.UserId == userId, includeProperties: "Transaction").ToList();
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

        public async Task<TransactionModel> GetTransaction(string transactionId)
        {
            return  _mapper.Map<TransactionModel>(await _unitOfWork.TransactionRepository.GetFirst(t => t.TransactionId == transactionId));
        }

      
    }
}

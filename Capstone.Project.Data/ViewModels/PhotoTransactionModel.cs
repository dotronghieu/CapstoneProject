﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class PhotoTransactionModel
    {
        public int PhotoId { get; set; }
        public string PhotoName { get; set; }
        public string Link { get; set; }
        public string Wmlink { get; set; }
        public int? TypeId { get; set; }
        public string UserId { get; set; }
        public decimal? BoughtPrice { get; set; }
        public DateTime? BoughtTime { get; set; }
        public string Description { get; set; }
        public string TransactionId { get; set; }
    }
}

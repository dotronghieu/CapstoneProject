﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class OrderDetailModel
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string PhotoId { get; set; }
        public double Price { get; set; }
    }
}
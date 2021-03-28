using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class UserSellStatisticModel
    {
        public int PhotoId { get; set; }
        public string PhotoName { get; set; }
        public string Link { get; set; }
        public string Wmlink { get; set; }
        public decimal? TotalSellAmount { get; set; }
        public double TotalNumberAmount { get; set; }
    }
}

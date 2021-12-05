using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Repository
{
    public class PurchaseHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EquitityType { get; set; }
        public string Symbol { get; set; }
        public double Cost { get; set; }
        public int Shares { get; set; }
        public DateTime BroughtDate { get; set; }
        public DateTime SellDate { get; set; }
        public DateTime ShortDate { get; set; }
        public DateTime BuytoCoverDate { get; set; }
    }
}

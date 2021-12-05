using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Models.CoinBase
{
    public class HistoricData
    {
        public DateTime time { get; set; }
        public float low { get; set; }
        public float high { get; set; }
        public float open { get; set; }
        public float close { get; set; }
        public float volume { get; set;}
    }
}

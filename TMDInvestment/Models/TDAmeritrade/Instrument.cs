using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Models
{
    public sealed class Instrument
    {
        public string assetType { get; set; }
        public string? cusip { get; set; }
        public string symbol { get; set; }
        public dynamic quote { get; set; }
        public Candles priceHistory { get; set; }
    }
}

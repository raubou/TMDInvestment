using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Models
{
    public class Accounts
    {
        public string id { get; set; }
        public string currency { get; set; }
        public string balance { get; set; }
        public string available { get; set; }
        public string hold { get; set; }
        public string profile_id { get; set; }
        public bool trading_enabled { get; set; }

        public Ticker ticker { get; set; }
        //public Conversions conversion { get; set;}
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Models
{
    public class PlaceOrder
    {   
        public string orderType { get; set; }
        public string session { get; set; }
        public string duration { get; set; }
        public string orderStrategyType { get; set; }
        public List<OrderLegCollection> orderLegCollection { get; set; }

    }
}

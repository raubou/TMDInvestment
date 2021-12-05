using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Models
{
    public class Orders 
    {
        public string session { get; set; }
        public string duration { get; set; }
        public string orderType { get; set; }
        public string complexOrderStrategyType { get; set; }
        public float quantity { get; set; }
        public float filledQuantity { get; set; }
        public float remainingQuantity { get; set; }
        public string requestedDestination { get; set; }
        public string destinationLinkName { get; set; }
        public float price { get; set; }
        public List<OrderLegCollection> orderLegCollection { get; set; }
        public string orderStrategyType { get; set; }
        public long orderId { get; set; }
        public bool cancelable { get; set; }
        public bool editable { get; set; }
        public string status { get; set; }
        public string enteredTime { get; set; }
        public string closeTime { get; set; }
        public string tag { get; set; }
        public int accountId { get; set; }
    }

}

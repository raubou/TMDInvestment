using System.Collections.Generic;

namespace TMDInvestment.Models
{
    public class Account
    {
        public AccountBalances accountBalances { get; set; }
        public List<Positions> positions { get; set; }
    }
}

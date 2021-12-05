using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Repository
{
    public class AccessKeys
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public int UserId { get; set; }
        public string Provider { get; set; }
        //public string AccountId { get; set; }
        //public int Key { get; set; }
        //public string Secret { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? DateCreated { get; set; }
    }
}

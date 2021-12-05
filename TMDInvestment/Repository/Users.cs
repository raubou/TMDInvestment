using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TMDInvestment.Repository
{
    public class Users
    {
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int? Id{ get; set; }  
		public string UserName{ get; set; }  
		public string Email{ get; set; }  
		public string Password{ get; set; }  
		public string AccountId{ get; set; }  
		//public string TDApiKey{ get; set; }
		//public string Token{ get; set; }
		//public string RefreshToken { get; set; }
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime? DateCreated{ get; set; } 
	}
}

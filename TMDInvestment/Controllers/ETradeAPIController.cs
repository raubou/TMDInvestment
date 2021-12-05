using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDInvestment.Helpers;
using TMDInvestment.Services;

namespace TMDInvestment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ETradeAPIController : Controller
    {
        private ETradeService Service;
        //private dynamic error;
        public ETradeAPIController()
        {
            Service = new ETradeService();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetRequestToken")]
        public IActionResult GetRequestToken()
        {
            //dynamic results;
            //var results = Service.GetRequestToken(ref error);
            //if (results == null || error != null || Errors.HasErrors(error))
            //{
            //    return BadRequest(error);
            //}            
            var results = Service.CallCrap();
            return Ok(results);
        }
    }
}

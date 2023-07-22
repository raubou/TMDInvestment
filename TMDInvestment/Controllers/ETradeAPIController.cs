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
    public sealed class ETradeAPIController : Controller
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
            var results = Service.GetRequestToken();
            return Ok(results);
        }

        [HttpGet("Quote/{symbol}")]
        public IActionResult Lookup(string symbol)
        {
            var results = Service.Quote(symbol);
            return Ok(results);
        }

        [HttpGet("Search/{search}")]
        public IActionResult Search(string search)
        {
            var results = Service.Search(search);
            return Ok(results);
        }
    }
}

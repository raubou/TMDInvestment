using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDInvestment.Services;

namespace TMDInvestment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralAPIController : ControllerBase
    {
        DBService service;
        CoinBaseService coinBaseService;
        dynamic error;
        public GeneralAPIController()
        {
            service = new DBService();
            coinBaseService = new CoinBaseService();
        }

        [HttpGet("GetAccountSettings/{userId}")]
        public IActionResult GetAccountSettings(int userId)
        {
            var results = service.GetAccountSetting(userId);
            return Ok(results);
        }

        [HttpGet("GetPurchaseHistory/{userId}/{symbol}")]
        public IActionResult GetPurchaseHistory(int userId, string symbol)
        {
            var results = service.GetPurchaseHistory(userId, symbol);
            return Ok(results);
        }

        [HttpGet("GetCoins")]
        public IActionResult GetCoins()
        {
            var results = service.GetCoins();
            foreach (var coin in results)
            {
                var history = coinBaseService.GetHistoricData(coin.Coin.Trim(), DateTime.Now.AddDays(-10).Date, DateTime.Now.Date, ref error);
                if (history is List<Models.HistoricData>)
                    coin.history = history;
                coin.ticker = coinBaseService.GetProductTicker(coin.Coin.Trim(), "ticker", ref error);
                var orders = coinBaseService.GetOrder(coin.Coin, ref error);
                if (orders is List<Models.Order>)
                    coin.order = orders;
                if (coin.order != null && coin.order.Count > 0)
                    foreach (var order in coin.order)
                    {
                        order.fills = coinBaseService.GetFills(order.id, ref error);
                    }
            }
            return Ok(results);
        }

        //[HttpGet("SaveCoin/{coin}/{description}")]
        //public IActionResult SaveCoin(string coin, string description)
        //{
        //    var results = service.SaveCoin(coin, description);
        //    return Ok(results);
        //}

        //[HttpGet("DeleteCoin/{Id}")]
        //public IActionResult DeleteCoin(int Id)
        //{
        //    var results = service.DeleteCoin(Id);
        //    return Ok(results);
        //}
    }
}

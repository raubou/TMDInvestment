using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDInvestment.Models;
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
        public async Task<IActionResult> GetCoins()
        {
            //List<Coins> results = null;
            List<Coins> coins = service.GetCoins();
            dynamic history = null;
            dynamic ticker = null;
            dynamic orders = null;

            foreach (var coin in coins)
            {

                var task1 = Task.Run(() => {
                    history = coinBaseService.GetHistoricData(coin.Coin.Trim().ToUpper(), DateTime.Now.AddDays(-20).Date, DateTime.Now.Date, ref error);
                    if (history is List<HistoricData> && history.Count > 0)
                        coin.history = ((List<HistoricData>)history).OrderByDescending(x => x.time).ToList();
                });

                var task2 = Task.Run(() => {
                    ticker = coinBaseService.GetProductTicker(coin.Coin.Trim().ToUpper(), "ticker", ref error);
                    if (ticker is Ticker)
                        coin.ticker = ticker;
                });

                var task3 = Task.Run(() => {
                    orders = coinBaseService.GetOrder(coin.Coin.Trim().ToUpper(), ref error);
                    if (orders is List<Order>)
                        coin.order = ((List<Order>)orders).OrderByDescending(x => x.created_at).ToList();
                    if (coin.order != null && coin.order.Count > 0)
                        foreach (var order in coin.order)
                        {
                            order.fills = coinBaseService.GetFills(order.id.Trim(), ref error);
                        }
                });
                await Task.WhenAll(task1, task2, task3);
            }

            return Ok(coins);
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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using TMDInvestment.Models;
using TMDInvestment.Services;

namespace TMDInvestment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinBaseAPIController : ControllerBase
    {
        private CoinBaseService coinBaseService;
        private dynamic error;
        public CoinBaseAPIController()
        {
            coinBaseService = new CoinBaseService();
        }

        [HttpGet("GetProduct/{productId}/{type}")]
        public IActionResult GetProduct(string productId, string type)
        {
            var results = coinBaseService.GetProductTicker(productId, type, ref error);
            return Ok(results);
        }
        [HttpGet("GetProduct/{productId}")]
        public IActionResult GetProduct(string productId)
        {
            var results = coinBaseService.GetProduct(productId, ref error);
            results.ticker = coinBaseService.GetProductTicker(results.id, "ticker", ref error);
            return Ok(results);
        }
        [HttpGet("GetProducts")]
        public IActionResult GetProducts()
        {
            var results = coinBaseService.GetProducts(ref error);
            return Ok(results);
        }
        [HttpGet("GetProductTicker/{productId}")]
        public IActionResult GetProductTicker(string productId)
        {
            
            var results = coinBaseService.GetProductTicker(productId, "ticker", ref error);
            return Ok(results);
        }
        [HttpGet("GetHistoricData/{productId}/{startTime}/{endTime}")]
        public IActionResult GetHistoricData(string productId, DateTime startTime, DateTime endTime)
        {
            
            var results = coinBaseService.GetHistoricData(productId.Trim(), startTime, endTime, ref error);
            return Ok(results);
        }
        [HttpGet("GetAccounts")]
        public IActionResult GetAccounts()
        {
            var results = (List<Accounts>)coinBaseService.GetAccounts(ref error);
            //var results = coinBaseService.GetAccounts(ref error);
            if (results != null)
                foreach (var item in results)
                {
                    if (item.currency != "USD" && item.currency != "USDC")
                    {
                        item.ticker = coinBaseService.GetProductTicker(item.currency + "-USD", "ticker", ref error);
                        item.order = coinBaseService.GetOrder(item.currency + "-USD", ref error);
                        if (item.order != null && item.order.Count > 0)
                        {
                            foreach (var order in item.order)
                            {
                                order.fills = coinBaseService.GetFills(order.id, ref error);                            
                                order.fills.ForEach(delegate (Fills fill)
                                {
                                    if (fill.side == "buy")
                                        item.broughtTicker += Convert.ToDecimal(fill.price) + Convert.ToDecimal(fill.fee);
                                });
                                item.broughtTicker = (item.broughtTicker / order.fills.Count);
                                item.broughtAmount = Convert.ToDecimal(item.available) * item.broughtTicker;
                                item.currentAmount = Convert.ToDecimal(item.available) * Convert.ToDecimal(item.ticker.price);
                            }
                        }
                    }

                }
            return Ok(results);
        }
        [HttpGet("GetOrders")]
        public IActionResult GetOrders()
        {
            var results = coinBaseService.GetOrders(ref error);
            return Ok(results);
        }
        [HttpGet("GetOrder/{productId}")]
        public IActionResult GetOrder(string productId)
        {
            var results = coinBaseService.GetOrder(productId, ref error);
            return Ok(results);
        }
        [HttpGet("GetFills/{orderId}")]
        public IActionResult GetFills(string orderId)
        {
            var results = coinBaseService.GetFills(orderId, ref error);
            return Ok(results);
        }
        [HttpGet("GetOrderandFill/{productId}")]
        public IActionResult GetOrderandFill(string productId)
        {
            var results = coinBaseService.GetOrder(productId, ref error);
            if(results != null && results.Count > 0)
                results[0].fills = coinBaseService.GetFills(results[0].id, ref error);
            return Ok(results);
        }
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] dynamic order)
        {
            var results = CoinBaseService.PlaceOrder(order, ref error);
            return Ok(results);
        }
    }
}

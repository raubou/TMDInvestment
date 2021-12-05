using Microsoft.AspNetCore.Mvc;
using TMD.Coinbase.PricePrediction.Helpers;
using TMDInvestment.Helpers;
using TMDInvestment.Models;
using TMDInvestment.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMDInvestment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TDAmeritradeAPIController : ControllerBase
    {
        private TDAmeritradeService service;
        private dynamic error;
        public TDAmeritradeAPIController()
        {
            service = new TDAmeritradeService();
        }
        
        [Route("GetWatchList")]
        public IActionResult WatchList()
        {
            var results = service.GetWatchList(ref error);
            if(Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }

        [Route("GetWatchList/{watchList}")]
        public IActionResult WatchList(string watchList)
        {
            var results = service.GetWatchList(watchList, ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }

        [Route("GetWatchListByName/{watchList}")]
        public IActionResult GetFilteredWatchList(string watchList)
        {
            var results = service.GetWatchListByName(watchList, ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
        [Route("GetAccounts")]
        public IActionResult GetAccounts()
        {
            AccountBalances results;
            results = service.GetAccountInfo(ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
        [Route("GetAccountInfo")]
        public IActionResult GetAccountInfo()
        {
            Account results = new Account();
            //results.accountBalances = service.GetAccountInfo(ref error);
            //results.accountPositions = service.GetPositions(ref error);
            var positions = service.GetPositions(ref error);
            //if (results != null && results.accountBalances != null && results.accountBalances.error != null)
            //{
            //    return NotFound(results.accountBalances.error + " " + results.accountBalances.error.description);
            //}
            //if (results != null && results.positions != null && results.positions.Count > 0 && results.positions[0].error != null)
            //{
            //    return NotFound(results.positions[0].error + " " + results.positions[0].error.description);
            //}
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            else
            {
                results.accountPositions = positions;
            }
            //if (results.positions[0] != null && results.positions[0].error != null)
            //{
            //    return NotFound(results.positions[0].error);
            //}
            return Ok(results);
        }
        [Route("GetPositions")]
        public IActionResult GetPositions()
        {
            var results = service.GetPositions(ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
        [HttpGet("GetOpenOrders/{orderStatus}")]
        public IActionResult GetOpenOrders(OrderStatus orderStatus)
        {
            var results = service.GetOpenOrders(orderStatus, ref error);
                if (Errors.HasErrors(error))
                {
                    return NotFound(error);
                }
            return Ok(results);
        }
        [HttpGet("GetOpenOrders")]
        public IActionResult GetOpenOrders()
        {
            var results = service.GetOpenOrders(OrderStatus.WORKING, ref error);
            if (results == null || results.Count <= 0)
            {                
                results = service.GetOpenOrders(OrderStatus.QUEUED, ref error);
                if (Errors.HasErrors(error))
                {
                    return NotFound(error);
                }
            }
            return Ok(results);
        }

        [HttpGet("GetMarketHours")]
        public IActionResult GetMarketHours()
        {
            var results = service.GetMarketHours(ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
        [HttpGet("GetQuote/{symbol}")]
        public IActionResult GetQuote(string symbol)
        {
            var results = service.GetQuote(symbol, ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
        [HttpGet("GetMovers/{market}/{direction}")]
        public IActionResult GetMovers(string market, Direction direction)
        {
            var results = service.GetMovers(market, direction, ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }

        [HttpGet("GetPriceHistory/{symbol}")]
        public IActionResult GetPriceHistory(string symbol)
        {

            var results = service.GetPriceHistory(symbol, PeriodType.month, 1, FrequencyType.daily, 1, ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] PlaceOrder order)
        {
            var results = service.PlaceOrder(order, ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
        [HttpPost("ReplaceOrder/{orderId}")]
        public IActionResult ReplaceOrder([FromBody] PlaceOrder order, string orderId)
        {
            var results = service.ReplaceOrder(order, orderId, ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
        [HttpPost("CancelOrder/{orderId}")]
        public IActionResult CancelOrder(string orderId)
        {
            var results = service.CancelOrder(orderId, ref error);
            if (Errors.HasErrors(error))
            {
                return NotFound(error);
            }
            return Ok(results);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TMDInvestment.Models;
using TMDInvestment.Services;
using TMDInvestments.Models;

namespace TMDInvestment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        dynamic error;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Default()
        {
            return View();
        }

        public IActionResult Movers()
        {
            string[] markets = { "$DJI", "$COMPX", "$SPX.X" };
            TDAmeritradeService client = new TDAmeritradeService();
            Stock stock = new Stock();
            List<Movers> moverUp = new List<Movers>();
            List<Movers> moverDown = new List<Movers>();
            //History history;
            for (int item = 0; item < 3; ++item)
            {
                moverUp.Add(client.GetMovers(markets[item],  Direction.up, ref error));
            }

            for (int item = 0; item < 3; ++item)
            {
                moverDown.Add(client.GetMovers(markets[item], Direction.down, ref error));
            }
            //foreach(var items in moverUp)
            //{
            //    foreach(var item in items.items)
            //    {
            //        history = new History();                    
            //        history.candles.items = client.GetPriceHistory(item.symbol, PeriodType.month, 1, FrequencyType.daily, 1);
            //        item.history = history;
            //    }
            //}
            //foreach (var items in moverDown)
            //{
            //    foreach (var item in items.items)
            //    {
            //        history = new History();
            //        history.candles.items = client.GetPriceHistory(item.symbol, PeriodType.month, 1, FrequencyType.daily, 1);
            //        item.history = history;
            //    }
            //}
            stock.moversUp = moverUp;
            stock.moversDown = moverDown;
            return View(stock);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

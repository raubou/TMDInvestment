using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using TMDInvestment.Models;
using TMDInvestment.Repository;
using TMDInvestment.Services;

namespace TMDInvestment.Controllers
{
    public class AccountController : Controller
    {
        private TDAmeritradeService service;
        private CoinBaseService coinBaseService;
        private DBService dbService;
        private dynamic error;
        public AccountController()
        {
            service = new TDAmeritradeService();
            coinBaseService = new CoinBaseService();
            dbService = new DBService();

        }
        public IActionResult Index(int id)
        {
            AccountInfo account = dbService.GetAccountSetting(id);
            if (account == null)
            {
                account = new AccountInfo();
            }
            return View(account);
            //return View();
        }

        [HttpPost()]
        public IActionResult Index([FromForm] AccountInfo setting)
        {
            bool results = false; 
            if (ModelState.IsValid)
            {
                results = dbService.SaveAccountSetting(setting);
            }

            if (results == false)
            {
                ViewBag.errors = "An error has occurred";
            }
            AccountInfo account = dbService.GetAccountSetting(setting.UserId);
            return View(account);
        }

        public async Task<IActionResult> CoinBaseAccount()
        {
            //dynamic results = new ExpandoObject();
            //results.Accounts = coinBaseService.GetAccounts(ref error);
            ////if(results.Accounts != null)
            ////    results.Accounts = ((List<Accounts>)results.Accounts).Where(x => decimal.Parse(x.balance.ToString()) > 0).ToList();
            //results.Products = coinBaseService.GetProducts(ref error);
            //results.Orders = coinBaseService.GetOrders(ref error);

            ////foreach (var item in results.Accounts)
            ////{
            ////    item.ticker = coinBaseService.GetProductTicker(item.currency + "-USD", "ticker", ref error);
            ////}
            //foreach (var item in results.Products)
            //{
            //    try
            //    {
            //        if (item.status != "Delisted" && item.trading_disabled == false)
            //            item.ticker = coinBaseService.GetProductTicker(item.id, "ticker", ref error);
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            //foreach (var item in results.Orders)
            //{
            //    item.fills = coinBaseService.GetFills(item.id, ref error);
            //}
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

            List<Coins> coins = dbService.GetCoins();
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

            ViewBag.coins = coins;
            return View(results);
        }

        [HttpGet()]
        public IActionResult Coins()
        {
            //return RedirectToAction("index", "default");
            var results = dbService.GetCoins();
            ViewBag.CryptoProducts = results;
            Coins cryptoProducts = new Coins();
            return View(cryptoProducts);
        }

        [HttpPost()]
        public IActionResult Coins(CryptoProducts cryptoProducts)
        {
            dbService.SaveCoin(cryptoProducts.Coin, cryptoProducts.Description);
            return RedirectToAction("Coins");
        }

        [HttpGet("DeleteCoin/{id}")]
        public IActionResult DeleteCoin(int id)
        {
            dbService.DeleteCoin(id);
            return RedirectToAction("Coins");
        }

        [HttpGet()]
        public IActionResult UpdateCoin(int id, string coin, string desc)
        {
            dbService.UpdateCoin(id, coin, desc);
            return RedirectToAction("Coins");
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}

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

        public IActionResult CoinBaseAccount()
        {
            dynamic results = new ExpandoObject();
            results.Accounts = coinBaseService.GetAccounts(ref error);
            //if(results.Accounts != null)
            //    results.Accounts = ((List<Accounts>)results.Accounts).Where(x => decimal.Parse(x.balance.ToString()) > 0).ToList();
            results.Products = coinBaseService.GetProducts(ref error);
            results.Orders = coinBaseService.GetOrders(ref error);
            
            //foreach (var item in results.Accounts)
            //{
            //    item.ticker = coinBaseService.GetProductTicker(item.currency + "-USD", "ticker", ref error);
            //}
            foreach (var item in results.Products)
            {
                item.ticker = coinBaseService.GetProductTicker(item.id, "ticker", ref error);
            }
            foreach (var item in results.Orders)
            {
                item.fills = coinBaseService.GetFills(item.id, ref error);
            }
            return View(results);
        }

        [HttpGet()]
        public IActionResult Coins()
        {
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using TMDInvestment.Services;
using System.Dynamic;
using TMDInvestment.Models;
using TMD.Coinbase.PricePrediction.Helpers;

namespace TMDInvestment.Controllers
{
    public class AuthController : Controller
    {
        //private readonly ILogger<AuthController> _logger;
        private TDAmeritradeService service;
        private dynamic error;
        //public AuthController(ILogger<AuthController> logger)
        public AuthController()
        {
            //_logger = logger;
            service = new TDAmeritradeService();
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet, Route("Authorize")]
        public IActionResult Authorize(string redirect)
        {
            dynamic gotoRedirect = new ExpandoObject();
            gotoRedirect.redirect = redirect;
            return View(gotoRedirect);
        }
        //[Route("Code")]
        public IActionResult Code()
        {
            var code = Request.Query["code"];
            service.GetAuthorizationToken(code, "", false, "", ref error);
            if (Errors.HasErrors(error))
            {
                return RedirectToAction("Error", "Home");
            }
            //return View();
            return RedirectToAction("Default");
        }
        //[Route("Account")]
        public IActionResult Account()
        {
            Account results = new Account();
            //results.accountBalances = service.GetAccountInfo(ref error);
            results.accountPositions = service.GetPositions(ref error);
            if (Errors.HasErrors(error))
            {
                return RedirectToAction("Authorize", "Auth", new { redirect = "Account" });
            }
            return View(results);
        }
        //[Route("WatchList")]
        public IActionResult WatchList()
        {
            List<WatchList> results;
            results = service.GetWatchList(ref error);
            if (Errors.HasErrors(error))
            {
                return RedirectToAction("Authorize", "Auth", new { redirect = "WatchList" });               
            }
            return View(results);
        }

        public IActionResult CycleAnalytics()
        {
            CycleAnalytics results = new CycleAnalytics();
            return View(results);
        }
    }
}

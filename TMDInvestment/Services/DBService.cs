using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDInvestment.DBContext;
using TMDInvestment.Models;
using TMDInvestment.Repository;

namespace TMDInvestment.Services
{
    public class DBService
    {
        private TMDInvestmentContext context;
        private string _apikey = string.Empty;
        private string _accountNo = string.Empty;
        //private string _userName = string.Empty;
        private int _userId = 0;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private IConfiguration TDAmeritradeAPI;
        //public Users user;
        public AccessKeys accessKeys;
        public DBService()
        {
            context = new TMDInvestmentContext();
            //user = GetTokens("raubou");
            accessKeys = GetTokens(1);
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            TDAmeritradeAPI = configuration.GetSection("TDAmeritradeAPI");
            if (TDAmeritradeAPI != null)
            {
                _apikey = TDAmeritradeAPI.GetSection("apikey").Value;
                //_baseUrl = TDAmeritradeAPI.GetSection("baseUrl").Value;
                _accountNo = TDAmeritradeAPI.GetSection("accountNo").Value;
                //_userName = TDAmeritradeAPI.GetSection("userName").Value;
                _userId = Convert.ToInt32(TDAmeritradeAPI.GetSection("userId").Value);
                _email = TDAmeritradeAPI.GetSection("email").Value;
                _password = TDAmeritradeAPI.GetSection("password").Value;
                //_token = TDAmeritradeAPI.GetSection("token").Value;
                //_refreshToken = TDAmeritradeAPI.GetSection("refreshToken").Value;
            }
        }
        public AccountInfo GetAccountSetting(int userId)
        {
            RulesEngine rulesEngine;
            AccountInfo account;
            try
            {
                context = new TMDInvestmentContext();
                rulesEngine = context.rulesEngine.Where(x => x.UserId == userId).FirstOrDefault();
                //Users user = GetTokens("raubou");
                if (rulesEngine == null)
                {
                    account = new AccountInfo() { UserId = accessKeys.UserId };
                }
                else
                {
                    account = new AccountInfo() { UserId = accessKeys.UserId, UseDayTrade = rulesEngine.UseDayTrade, UseSwingTrade = rulesEngine.UseSwingTrade, HistoryDayRange = rulesEngine.HistoryDayRange, HistoryIntervalRange = rulesEngine.HistoryIntervalRange, PercentBalanceUse = rulesEngine.PercentBalanceUse, PercentRangeHigh = rulesEngine.PercentRangeHigh, PercentRangeLow = rulesEngine.PercentRangeLow, PriceRangeMax = rulesEngine.PriceRangeMax, PriceRangeMin = rulesEngine.PriceRangeMin, SqueezeRangeHigh = rulesEngine.SqueezeRangeHigh, SqueezeRangeLow = rulesEngine.SqueezeRangeLow, MaxHoldforStock = rulesEngine.MaxHoldforStock, VolumeRangeLow = rulesEngine.VolumeRangLow, VolumeRangeHigh = rulesEngine.VolumeRangeHigh, MaxRoundTradesPerDay = rulesEngine.MaxRoundTradesPerDay };
                }
                //Repository.Users user = GetTokens("raubou");
                //account.UserId = (int)user.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
            return account;
        }

        //public RulesEngine GetAccountRules(int userId)
        //{
        //    RulesEngine rulesEngine;
        //    //AccountInfo account;
        //    try
        //    {
        //        context = new TMDInvestmentContext();
        //        rulesEngine = context.rulesEngine.Where(x => x.UserId == userId).FirstOrDefault();
        //        //Users user = GetTokens("raubou");
        //        //if (rulesEngine == null)
        //        //{
        //        //    account = new AccountInfo() { UserId = (int)user.Id };
        //        //}
        //        //else
        //        //{
        //        //    account = new AccountInfo() { UserId = (int)user.Id, UseDayTrade = rulesEngine.UseDayTrade, UseSwingTrade = rulesEngine.UseSwingTrade, HistoryDayRange = rulesEngine.HistoryDayRange, HistoryIntervalRange = rulesEngine.HistoryIntervalRange, PercentBalanceUse = rulesEngine.PercentBalanceUse, PercentRangeHigh = rulesEngine.PercentRangeHigh, PercentRangeLow = rulesEngine.PercentRangeLow, PriceRangeMax = rulesEngine.PriceRangeMax, PriceRangeMin = rulesEngine.PriceRangeMin, SqueezeRangeHigh = rulesEngine.SqueezeRangeHigh, SqueezeRangeLow = rulesEngine.SqueezeRangeLow, MaxHoldforStock = rulesEngine.MaxHoldforStock };
        //        //}
        //        //Repository.Users user = GetTokens("raubou");
        //        //account.UserId = (int)user.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    return rulesEngine;
        //}

        public bool SaveAccountSetting(AccountInfo account)
        {
            RulesEngine rulesEngine;
            try
            {
                context = new TMDInvestmentContext();
                rulesEngine = context.rulesEngine.Where(x => x.UserId == account.UserId).FirstOrDefault();
                if (rulesEngine == null)
                {
                    context.rulesEngine.Add(new RulesEngine() { UserId = account.UserId, UseDayTrade = account.UseDayTrade, UseSwingTrade = account.UseSwingTrade, HistoryDayRange = account.HistoryDayRange, HistoryIntervalRange = account.HistoryIntervalRange, PercentBalanceUse = account.PercentBalanceUse, PercentRangeHigh = account.PercentRangeHigh, PercentRangeLow = account.PercentRangeLow, PriceRangeMax = account.PriceRangeMax, PriceRangeMin = account.PriceRangeMin, SqueezeRangeHigh = account.SqueezeRangeHigh, SqueezeRangeLow = account.SqueezeRangeLow,VolumeRangeHigh = account.VolumeRangeHigh, VolumeRangLow = account.VolumeRangeLow, MaxHoldforStock = account.MaxHoldforStock });
                }
                else
                {
                    rulesEngine.UserId = account.UserId; rulesEngine.UseDayTrade = account.UseDayTrade; rulesEngine.UseSwingTrade = account.UseSwingTrade; rulesEngine.HistoryDayRange = account.HistoryDayRange; rulesEngine.HistoryIntervalRange = account.HistoryIntervalRange; rulesEngine.PercentBalanceUse = account.PercentBalanceUse; rulesEngine.PercentRangeHigh = account.PercentRangeHigh; rulesEngine.PercentRangeLow = account.PercentRangeLow; rulesEngine.PriceRangeMax = account.PriceRangeMax; rulesEngine.PriceRangeMin = account.PriceRangeMin; rulesEngine.SqueezeRangeHigh = account.SqueezeRangeHigh; rulesEngine.SqueezeRangeLow = account.SqueezeRangeLow; rulesEngine.DateUpdated = DateTime.Now; rulesEngine.MaxHoldforStock = account.MaxHoldforStock; rulesEngine.VolumeRangeHigh = account.VolumeRangeHigh; rulesEngine.VolumeRangLow = account.VolumeRangeLow; rulesEngine.MaxRoundTradesPerDay = account.MaxRoundTradesPerDay;
                    context.Entry(rulesEngine).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //change to use access key DB
        public bool SaveTokens(string token, string refreshToken)
        {
            try
            {
                context = new TMDInvestmentContext();
                AccessKeys accessKeys = context.accessKeys.Where(x => x.UserId == _userId).FirstOrDefault();
                if (accessKeys == null)
                {
                    context.accessKeys.Add(new AccessKeys() { UserId = _userId, Token = token, RefreshToken = refreshToken, Provider = "TDAmeritrade" });
                }
                else
                {
                    accessKeys.Token = token;
                    accessKeys.RefreshToken = refreshToken;
                    context.Entry(accessKeys).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public AccessKeys GetTokens(int userId)
        {
            AccessKeys keys;
            try
            {
                context = new TMDInvestmentContext();
                keys = context.accessKeys.Where(x => x.UserId == userId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
            return keys;
        }
        public bool SaveCoin(string coin, string description)
        {
            try
            {
                context = new TMDInvestmentContext();
                CryptoProducts cryptoProducts = context.cryptoProducts.Where(x => x.Coin == coin).FirstOrDefault();
                if (cryptoProducts == null)
                {
                    context.cryptoProducts.Add(new CryptoProducts() { Coin = coin, Description = description });
                }
                else
                {
                    cryptoProducts.Coin = coin;
                    cryptoProducts.Description = description;
                    context.Entry(cryptoProducts).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public List<Coins> GetCoins()
        {
            List<Coins> coins = new List<Coins>(); ;
            try
            {
                context = new TMDInvestmentContext();
                var crypto = context.cryptoProducts.ToList();
                foreach(var item in crypto)
                {
                    coins.Add(new Coins { Id = item.Id, Coin = item.Coin, Description = item.Description, DateCreated = item.DateCreated });
                }
            }
            catch (Exception ex)
            {
                return coins;
            }
            return coins;
        }

        public bool DeleteCoin(int Id)
        {
            CryptoProducts crypto;
            try
            {
                context = new TMDInvestmentContext();
                crypto = context.cryptoProducts.Where(x => x.Id == Id).FirstOrDefault();
                context.Entry(crypto).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool UpdateCoin(int Id, string coin, string desc)
        {
            CryptoProducts crypto;
            try
            {
                context = new TMDInvestmentContext();
                crypto = context.cryptoProducts.Where(x => x.Id == Id).FirstOrDefault();
                if(crypto != null)
                {
                    crypto.Coin = coin;
                    crypto.Description = desc;
                    context.Entry(crypto).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public PurchaseHistory GetPurchaseHistory(int UserId, string symbol)
        {
            PurchaseHistory purchase;
            try
            {
                context = new TMDInvestmentContext();
                purchase = context.purchaseHistory.Where(x => x.Symbol == symbol && x.UserId == UserId).OrderByDescending(x => x.BroughtDate).OrderByDescending(x => x.ShortDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
            return purchase;
        }

        public PurchaseHistory SavePurchaseHistory(int UserId, int equityType, string symbol, double cost, int shares, DateTime BroughtDate, DateTime SellDate, DateTime ShortDate, DateTime BuytoCoverDate)
        {
            PurchaseHistory purchase;
            try
            {
                context = new TMDInvestmentContext();
                if (SellDate != null || BuytoCoverDate != null)
                {
                    purchase = context.purchaseHistory.Where(x => x.Symbol == symbol && x.UserId == UserId).FirstOrDefault();
                    purchase.SellDate = SellDate;
                    purchase.BuytoCoverDate = BuytoCoverDate;
                    context.Entry(purchase).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }                    
                else
                {
                    purchase = new PurchaseHistory() { UserId = UserId, Symbol = symbol, Cost = cost, BroughtDate = BroughtDate, SellDate = SellDate, EquitityType = equityType, Shares = shares };
                    context.purchaseHistory.Add(purchase);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return null;
            }
            return purchase;
        }
    }
}

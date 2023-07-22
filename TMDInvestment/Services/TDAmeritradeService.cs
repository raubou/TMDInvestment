using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using TMDInvestment.APIProxy;
using TMDInvestment.Models;
using TMDInvestment.DBContext;
using TMDInvestment.Repository;
using System.Text;
using TMDInvestment.Helpers;
using System.Text.Json;
using TMD.Coinbase.PricePrediction.Helpers;
using TMDInvestments.Models;

namespace TMDInvestment.Services
{
    public class TDAmeritradeService
    {
        private string _apikey = string.Empty;
        private string _baseUrl = string.Empty;
        private string _accountNo = string.Empty;
        //private string _userName = string.Empty;
        //private string _email = string.Empty;
        //private string _password = string.Empty;
        //private string _token = string.Empty;
        //private string _refreshToken = string.Empty;
        private HttpContent _content;
        private IConfiguration TDAmeritradeAPI;
        //private TMDInvestmentContext context;
        private DBService dbService;
        //Users user;

        public TDAmeritradeService()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            TDAmeritradeAPI = configuration.GetSection("TDAmeritradeAPI");
            dbService = new DBService();
            
            if (TDAmeritradeAPI != null)
            {
                _apikey = TDAmeritradeAPI.GetSection("apikey").Value;
                _baseUrl = TDAmeritradeAPI.GetSection("baseUrl").Value;
                _accountNo = TDAmeritradeAPI.GetSection("accountNo").Value;
                //_userName = TDAmeritradeAPI.GetSection("userName").Value;
                //_email = TDAmeritradeAPI.GetSection("email").Value;
                //_password = TDAmeritradeAPI.GetSection("password").Value;
                //_token = TDAmeritradeAPI.GetSection("token").Value;
                //_refreshToken = TDAmeritradeAPI.GetSection("refreshToken").Value;
            }
        }
        public dynamic GetAuthorizationToken(string code, string refreshToken, bool isRefreshToken, string redirect, ref dynamic error)
        {
            dynamic results = new ExpandoObject();
            string refresh_token = string.Empty;
            Dictionary<string, string> content = new Dictionary<string, string>();
            if (isRefreshToken)
            {
                content.Add("grant_type", "refresh_token");
                content.Add("refresh_token", refreshToken);
                content.Add("access_type", "");
                content.Add("code", "");
            }
            else
            {
                content.Add("grant_type", "authorization_code");
                content.Add("refresh_token", "");
                content.Add("access_type", "offline");
                content.Add("code", code);
                content.Add("redirect_uri", "http://localhost:50735/auth/code");
            }
            content.Add("client_id", _apikey);
            _content = new FormUrlEncodedContent(content);
            _content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            _content.Headers.ContentType.CharSet = "UTF-8";            
            string url = _baseUrl + "oauth2/token";           
            var response = APIProxy.APIProxy<dynamic>.Post(url, _content, ref error);
            if(!Errors.HasErrors(error))
            {
                refresh_token = (isRefreshToken) ? refreshToken : ((JsonElement)response).GetProperty("refresh_token").GetString();
                dbService.SaveTokens(((JsonElement)response).GetProperty("access_token").GetString(), refresh_token, "TDAMeritrade");
                results.access_token = ((JsonElement)response).GetProperty("access_token").GetString();
                results.refresh_token = refresh_token;
            }
            return results;
        }
        public dynamic GetMovers(string market, Direction direction, ref dynamic error)
        {
            Movers results = new Movers(direction);
            //https://api.tdameritrade.com/v1/marketdata/$DJI/movers?apikey=AAYVAX3S39IAZW4VNNRP0JZVJOLTNXQI&direction=down&change=percent
            string url = _baseUrl + "marketdata/" + market + "/movers?apikey=" + _apikey + "&direction=" + direction.ToString() + "&change=percent"; 
            results.items = APIProxy<List<Items>>.Get(url, ref error);
            return results;
        }


        public dynamic GetQuote(string symbol, ref dynamic error)
        {
            dynamic proxy;
            //CandleItems results = null;
            Quote results = null;
            //https://api.tdameritrade.com/v1/marketdata/quotes?apikey=AAYVAX3S39IAZW4VNNRP0JZVJOLTNXQI&symbol=AMD
            string url = _baseUrl + "marketdata/quotes" + "?apikey=" + _apikey + "&symbol=" + symbol.ToUpper();
            //results = APIProxy<CandleItems>.Get(url, ref error);
            proxy = APIProxy<dynamic>.Get(url, ref error);
            try
            {
                var json = ((JsonElement)proxy).GetProperty(symbol.ToUpper()).GetRawText();
                results = JsonSerializer.Deserialize<Quote>(json);
            }
            catch (Exception)
            {

            }
            return results;
        }
        public dynamic GetMarketHours(ref dynamic error)
        {
            dynamic results = new ExpandoObject();
            //https://api.tdameritrade.com/v1/marketdata/equity/hours?apikey=AAYVAX3S39IAZW4VNNRP0JZVJOLTNXQI
            string url = _baseUrl + "marketdata/hours?apikey=" + _apikey + "&markets=EQUITY";
            results = APIProxy<ExpandoObject>.Get(url, ref error);
            return results;
        }
        public dynamic GetPriceHistory(string symbol,PeriodType periodType,int period,FrequencyType frequencyType, int frequency, ref dynamic error)
        {
            Candles results;
            //https://api.tdameritrade.com/v1/marketdata/BAC/pricehistory?apikey=AAYVAX3S39IAZW4VNNRP0JZVJOLTNXQI&periodType=day&period=10&frequencyType=minute&frequency=5
            string url = _baseUrl + "marketdata/" + symbol.ToUpper() + "/pricehistory" + "?apikey=" + _apikey + "&periodType=" + periodType.ToString() + "&period=" + period + "&frequencyType=" + frequencyType + "&frequency=" + frequency; 
            results = APIProxy<Candles>.Get(url, ref error);
            if(results != null)
            {
                foreach (var item in results.candles)
                {
                    //item.time = DateTimeOffset.FromUnixTimeSeconds(item.datetime).DateTime;
                    item.time = DateTimeOffset.FromUnixTimeMilliseconds(item.datetime).DateTime;
                }
                results.candles = AddChangeData(results.candles);
            }
            return results;
        }

        private List<CandleItems> AddChangeData(List<CandleItems> candleItem)
        {
            decimal previous = 0;
            decimal open = 0;
            decimal change = 0;

            foreach (var item in candleItem)
            {
                if (open > 0)
                {
                    change = (item.open - open);
                    item.change = change;
                    previous = open;
                }
                open = item.open;
                if (open > 0 && previous > 0)
                {
                    item.percent = (change / previous);
                }
            }
            return candleItem;
        }

        public dynamic GetAccountInfo(ref dynamic error)
        {
            AccountBalances results;
            
            //https://api.tdameritrade.com/v1/accounts
            string url = _baseUrl + "accounts/" + _accountNo;
            results = APIProxy<AccountBalances>.Get(url, dbService.accessKeys.Token, ref error);
            if (results == null || Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "WatchList", ref error);
                if (Errors.HasErrors(error))
                {
                    return results;
                }
                error = null;
                results = APIProxy<AccountBalances>.Get(url, token.access_token.ToString(), ref error);
                if (results == null || Errors.HasErrors(error))
                {
                    return error;
                }
            }
            return results;
        }

        public AccountPositions GetPositions(ref dynamic error)
        {
            //List<Positions> results;
            AccountPositions results;
            
            //https://api.tdameritrade.com/v1/accounts
            //string url = _baseUrl + "accounts?fields=positions,orders";
            string url = _baseUrl + "accounts/" + _accountNo + "?fields=positions,orders";
            results = APIProxy<AccountPositions>.Get(url, dbService.accessKeys.Token, ref error);
            if (results == null || Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "WatchList", ref error);
                if (Errors.HasErrors(error))
                {
                    return new AccountPositions();
                }
                error = null;
                results = APIProxy<AccountPositions>.Get(url, token.access_token.ToString(), ref error);
                if (results == null || Errors.HasErrors(error))
                {
                    return new AccountPositions();
                }
            }
            foreach(var item in ((AccountPositions)results).securitiesAccount.positions)
            {
                item.Instrument.quote = GetQuote(item.Instrument.symbol, ref error);
            }
            return results;
        }

        public dynamic GetWatchList(ref dynamic error)
        {
            List<WatchList> results;
            
            //https://api.tdameritrade.com/v1/accounts/watchlists
            string url = _baseUrl + "accounts/watchlists";
            results = APIProxy<List<WatchList>>.Get(url, dbService.accessKeys.Token, ref error);
            if (Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "", ref error);
                if(Errors.HasErrors(error))
                {
                    return results;
                }
                error = null;
                results = APIProxy<List<WatchList>>.Get(url, token.access_token.ToString(), ref error);
                if (Errors.HasErrors(error))
                {
                    return results;
                }
            }
            return results;
        }

        public dynamic GetWatchListByName(string watchList, ref dynamic error)
        {
            List<WatchList> results;
            WatchList filtered = null;
            
            //https://api.tdameritrade.com/v1/accounts/watchlists
            string url = _baseUrl + "accounts/watchlists";
            results = APIProxy<List<WatchList>>.Get(url, dbService.accessKeys.Token, ref error);
            if (Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "", ref error);
                if (Errors.HasErrors(error))
                {
                    return null;
                }
                error = null;
                results = APIProxy<List<WatchList>>.Get(url, token.access_token.ToString(), ref error);
                if (Errors.HasErrors(error))
                {
                    return null;
                }
            }
            filtered = results.Where(x => x.name.Contains(watchList)).FirstOrDefault();
            foreach (var item in filtered.watchListItems)
            {
                item.instrument.quote = GetQuote(item.instrument.symbol, ref error);
                item.instrument.priceHistory = GetPriceHistory(item.instrument.symbol, PeriodType.month, 1, FrequencyType.daily, 1, ref error);
            }
            return filtered;
        }

        public dynamic GetWatchList(string watchList, ref dynamic error)
        {
            WatchList results;
            
            //https://api.tdameritrade.com/v1/accounts/watchlists
            string url = _baseUrl + "accounts/" + _accountNo + "/watchlists/" + watchList;
            results = APIProxy<WatchList>.Get(url, dbService.accessKeys.Token, ref error);
            if (results == null || Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "WatchList", ref error);
                if (Errors.HasErrors(error))
                {
                    return results;
                }
                error = null;
                results = APIProxy<WatchList>.Get(url, token.access_token.ToString(), ref error);
                if (results == null || Errors.HasErrors(error))
                {
                    return error;
                }
            }
            foreach (var item in results.watchListItems)
            {
                item.instrument.quote = GetQuote(item.instrument.symbol, ref error);
                item.instrument.priceHistory = GetPriceHistory(item.instrument.symbol, PeriodType.month, 1, FrequencyType.daily, 1, ref error);
            }
            return results;
        }

        public dynamic GetOpenOrders(OrderStatus orderStatus, ref dynamic error)
        {
            List<Orders> results;
            
            //https://api.tdameritrade.com/v1/orders?accountId=421278406&status=WORKING
            string url = _baseUrl + "orders?accountId=" + _accountNo + "&status=" + orderStatus.ToString();
            results = APIProxy<List<Orders>>.Get(url, dbService.accessKeys.Token,ref error);
            if (results == null || Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "WatchList", ref error);
                if (Errors.HasErrors(error))
                {
                    return results;
                }
                error = null;
                results = APIProxy<List<Orders>>.Get(url, token.access_token.ToString(),ref error);
                if (error == null || Errors.HasErrors(error))
                {
                    return error;
                }
            }
            return results;
        }
        public dynamic PlaceOrder(PlaceOrder order, ref dynamic error)
        {
            Orders results;
            
            //https://api.tdameritrade.com/v1/accounts/421278406/orders
            string url = _baseUrl + "accounts/" + _accountNo  + "/orders";
            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            results = APIProxy<Orders>.Post(url, dbService.accessKeys.Token, content, ref error);
            if (results == null || Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "", ref error);
                if (Errors.HasErrors(error))
                {
                    return results;
                }
                error = null;
                results = APIProxy<Orders>.Post(url, token.access_token.ToString(), content, ref error);
                if (results == null || Errors.HasErrors(error))
                {
                    return error;
                }
            }
            return results;
        }
        public dynamic ReplaceOrder(PlaceOrder order, string orderId, ref dynamic error)
        {
            Orders results;
            
            //https://api.tdameritrade.com/v1/accounts/421278406/orders/{orderid}
            string url = _baseUrl + "v1/accounts/" + _accountNo + "/orders/" + orderId;
            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            results = APIProxy<Orders>.Put(url, dbService.accessKeys.Token, content, ref error);
            if (results == null || Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "", ref error);
                if (Errors.HasErrors(error))
                {
                    return results;
                }
                error = null;
                results = APIProxy<Orders>.Put(url, token.access_token.ToString(), content, ref error);
                if (results == null || Errors.HasErrors(error))
                {
                    return error;
                }
            }
            return results;
        }
        public dynamic CancelOrder(string orderId, ref dynamic error)
        {
            Orders results;
            
            //https://api.tdameritrade.com/v1/accounts/421278406/orders/{orderid}
            string url = _baseUrl + "v1/accounts/" + _accountNo + "/orders/" + orderId;
            results = APIProxy<Orders>.Delete(url, dbService.accessKeys.Token, ref error);
            if (results == null || Errors.HasErrors(error))
            {
                error = null;
                var token = GetAuthorizationToken("", dbService.accessKeys.RefreshToken, true, "", ref error);
                if (Errors.HasErrors(error))
                {
                    return results;
                }
                error = null;
                results = APIProxy<Orders>.Delete(url, token.access_token.ToString(), ref error);
                if (results == null || Errors.HasErrors(error))
                {
                    return error;
                }
            }
            return results;
        }

    }
}

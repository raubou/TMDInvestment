using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TMDInvestment.APIProxy;
using TMDInvestment.Helpers;
using System.Text.Json;
using TMDInvestment.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using TMD.Coinbase.PricePrediction.Helpers;
using System.Runtime.Intrinsics.X86;

namespace TMDInvestment.Services
{
    public sealed class CoinBaseService
    {
        private string _apikey = string.Empty;
        private static string _baseUrl = string.Empty;
        private static string _apiSecret = string.Empty;
        private string _passPhase = string.Empty;
        private string _url = string.Empty;
        private IConfiguration CoinBaseProAPI;

        public CoinBaseService()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            CoinBaseProAPI = configuration.GetSection("CoinBaseProAPI");
            if (CoinBaseProAPI != null)
            {
                _apikey = CoinBaseProAPI.GetSection("apikey").Value;
                _baseUrl = CoinBaseProAPI.GetSection("baseUrl").Value;
                _apiSecret = CoinBaseProAPI.GetSection("apiSecret").Value;
                _passPhase = CoinBaseProAPI.GetSection("passPhase").Value;
                CoinBaseAuthenicator.CB_ACCESS_KEY = _apikey;
                CoinBaseAuthenicator.CB_ACCESS_PASSPHRASE = _passPhase;
            }
        }

        public dynamic GetAccounts(ref dynamic error)
        {
            List<Accounts> results = null;
            string url = "/accounts";
            HttpRequestMessage request;
            try
            {
                request = GetAuthenicatedRequest(HttpMethod.Get, url, null, ref error);
                results = APIProxy<List<Accounts>>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
            }
            if (results != null)
                return results.Where(x => decimal.Parse(x.balance.ToString()) > 0).ToList();
            else
                return results;
        }

        public dynamic GetOrders(ref dynamic error)
        {
            List<Order> results = null;
            string url = "/orders?status=done&limit=5";
            HttpRequestMessage request;
            try
            {
                request = GetAuthenicatedRequest(HttpMethod.Get, url, null, ref error);
                results = APIProxy<List<Order>>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
            }
            return results;
        }

        public dynamic GetOrder(string productId, ref dynamic error)
        {
            List<Order> results = null;
            string url = "/orders?status=done&limit=1&sorting=desc&product_id=" + productId;
            HttpRequestMessage request;
            try
            {
                request = GetAuthenicatedRequest(HttpMethod.Get, url, null, ref error);
                results = APIProxy<List<Order>>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
            }
            return results;
        }

        public dynamic GetFills(string orderId, ref dynamic error)
        {
            List<Fills> results = null;
            string url = "/fills?order_id=" + orderId;
            HttpRequestMessage request;
            try
            {
                request = GetAuthenicatedRequest(HttpMethod.Get, url, null, ref error);
                results = APIProxy<List<Fills>>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
            }
            return results;
        }

        public static dynamic PlaceOrder(dynamic model, ref dynamic error)
        {
            dynamic results = new ExpandoObject();
            string url = "/orders";
            HttpRequestMessage request;
            try
            {
                request = GetAuthenicatedRequest(HttpMethod.Post, url, model, ref error);
                request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                results = APIProxy<dynamic>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
            }

            return results;
        }

        public dynamic GetProduct(string productId, ref dynamic error)
        {
            Product results = null;
            string url = "/products/" + productId;
            HttpRequestMessage request;
            try
            {
                request = GetRequest(url, HttpMethod.Get, null);
                results = APIProxy<Product>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
            }
            return results;
        }

        public List<Product> GetProducts(ref dynamic error)
        {
            List<Product> results = null;
            string url = "/products";
            HttpRequestMessage request;
            try 
            {
                request = GetRequest(url, HttpMethod.Get, null);
                results = APIProxy<List<Product>>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
            }
            return results;
        }

        public dynamic GetProductTicker(string productId, string type, ref dynamic error)
        {
            Ticker results = null;
            string url = "/products";
            HttpRequestMessage request;
            try
            {
                if (productId.Trim() != string.Empty)
                {
                    url += "/" + productId;
                }
                if (type.Trim() != string.Empty)
                {
                    url += "/" + type;
                }
                request = GetRequest(url, HttpMethod.Get, null);
                results = APIProxy<Ticker>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    //try again
                    request = GetRequest(url, HttpMethod.Get, null);
                    results = APIProxy<dynamic>.Send(request, ref error);
                    if (Errors.HasErrors(error))
                    {
                        return error;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return results;
        }

        //format [timestamp, price_low, price_high, price_open, price_close]
        public dynamic GetHistoricData(string productId, DateTime startTime, DateTime endTime, ref dynamic error)
        {
            dynamic results = new ExpandoObject();
            List<HistoricData> HistoricData = new List<HistoricData>();
            HistoricData candleItem;
            string url = "/products/" + productId + "/candles?start=" + startTime.ToString("yyyy-MM-ddTHH:mm:ssZ") + "&end=" + endTime.ToString("yyyy-MM-ddTHH:mm:ssZ") + "&granularity=21600";
            HttpRequestMessage request;
            try
            {
                request = GetRequest(url, HttpMethod.Get, null);
                results = APIProxy<dynamic>.Send(request, ref error);
                if (Errors.HasErrors(error))
                {
                    //try again
                    request = GetRequest(url, HttpMethod.Get, null);
                    results = APIProxy<dynamic>.Send(request, ref error);
                    if (Errors.HasErrors(error))
                    {
                        return error;
                    }
                }
                var first = ((JsonElement)results).EnumerateArray();
                Console.ForegroundColor = ConsoleColor.Red;                 
                foreach (var his in first)
                {
                    int index = 0;
                    candleItem = new HistoricData() { symbol = productId };
                    var second = ((JsonElement)his).EnumerateArray();
                    foreach (var col in second)
                    {
                        index += 1;
                        switch (index)
                        {
                            case 1:
                                candleItem.epoch = Convert.ToInt64(col.ToString());
                                candleItem.time = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(col.ToString())).DateTime;
                                break;
                            case 2:
                                candleItem.low = Convert.ToDecimal(col.ToString());
                                break;
                            case 3:
                                candleItem.high = Convert.ToDecimal(col.ToString());
                                break;
                            case 4:
                                candleItem.open = Convert.ToDecimal(col.ToString());
                                break;
                            case 5:
                                candleItem.close = Convert.ToDecimal(col.ToString());
                                break;
                            case 6:
                                candleItem.volume = Convert.ToDecimal(col.ToString());
                                break;
                        }
                        Console.WriteLine(col.ToString());
                    }
                    HistoricData.Add(candleItem);
                }
            }
            catch (Exception ex)
            {
            }
            HistoricData = AddChangeData(HistoricData);
            return HistoricData;
        }

        private List<HistoricData> AddChangeData(List<HistoricData> candleItem)
        {
            decimal previous = 0;
            decimal open = 0;
            decimal change = 0;
            List<HistoricData> results = candleItem.OrderBy(x => x.time).ToList();
            foreach(var item in results)
            {
                if(open > 0)
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
            return results;
        }

        private static string GetTimeStamp(ref dynamic error)
        {
            string url = "/time";
            dynamic results;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + url);
            request.Headers.Add("User-Agent", "TMDInvestment/1.0");
            results = APIProxy<dynamic>.Send(request, ref error);
            return results.GetProperty("epoch").ToString();
        }

        private HttpRequestMessage GetRequest(string url, HttpMethod method, dynamic body)
        {
            string sBody = (body != null) ? System.Text.Json.JsonSerializer.Serialize(body) : string.Empty;
            HttpRequestMessage request = new HttpRequestMessage(method, _baseUrl + url);
            request.Headers.Add("User-Agent", "TMDInvestment/1.0");
            request.Headers.Add("Accept", "application/json");
            request.Content = new StringContent(sBody, Encoding.UTF8, "application/json");
            return request;
        }

        private static HttpRequestMessage GetAuthenicatedRequest(HttpMethod method, string url, dynamic model, ref dynamic error)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
                MaxDepth = 128,
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Disallow
            };

            string jsonString = string.Empty;
            //if (model != null && ((JsonElement)model).GetType() != null)
            try
            {
                if (((JsonElement)model).GetType() != null)
                    jsonString = JsonSerializer.Serialize(model, options);
            }
            catch (Exception)
            {

            }

            CoinBaseAuthenicator.CB_ACCESS_TIMESTAMP = GetTimeStamp(ref error);
            CoinBaseAuthenicator.CB_ACCESS_SIGN = CoinBaseAuthenicator.GenerateSignature(CoinBaseAuthenicator.CB_ACCESS_TIMESTAMP, method, url, jsonString, _apiSecret);
            HttpRequestMessage request = new HttpRequestMessage(method, _baseUrl + url);
            request.Headers.Add("CB-ACCESS-KEY", CoinBaseAuthenicator.CB_ACCESS_KEY);
            request.Headers.Add("CB-ACCESS-PASSPHRASE", CoinBaseAuthenicator.CB_ACCESS_PASSPHRASE);
            request.Headers.Add("CB-ACCESS-TIMESTAMP", CoinBaseAuthenicator.CB_ACCESS_TIMESTAMP);
            request.Headers.Add("CB-ACCESS-SIGN", CoinBaseAuthenicator.CB_ACCESS_SIGN);
            request.Headers.Add("User-Agent", "TMDInvestment/1.0");
            request.Headers.Add("Accept", "application/json");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return request;
        }

        public static bool AnalyzeCoin(Coins coin,decimal avgGainChange,decimal avgLossChange,decimal avgGainPercent,decimal avgLossPercent,decimal currentPrice,decimal ema,decimal BUYAMOUNT,int RSIstepone, ref decimal SellTicker)
        {
            bool buy = false;
            decimal priceatLastFill = 0m;
            DateTime dateofLastOrder = DateTime.MinValue;
            DateTime lastOrderDate = DateTime.MinValue;

            SellTicker = (coin.order != null && coin.order.Count > 0 && coin.order[0].fills != null) ? coin.order[0].fills.Where(x => x.side == "sell").Sum(x => Convert.ToDecimal(x.price)) : 0m;
            lastOrderDate = (coin.order != null && coin.order.Count > 0) ? DateTime.Parse(coin.order[0].done_at) : DateTime.MinValue;

            //if ((avgGainChange > avgLossChange) && (avgGainPercent > avgLossPercent) && (RSIstepone > 50) && (ema > currentPrice) && (avgGainPercent >= ((SELLMARGINDECIMAL * 100) + (SELLFEE * 100))))
            if ((avgGainChange >= BUYAMOUNT) && (avgGainChange > avgLossChange) && (avgGainPercent > avgLossPercent) && (RSIstepone >= 50) && (ema >= currentPrice))
            {
                buy = true;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Avg for Symbol " + coin.Coin);
            Console.WriteLine($"Current Price ${currentPrice}");
            Console.WriteLine("Last order date " + lastOrderDate);

            if (coin.order != null && coin.order.Count > 0 && coin.order[0].fills != null)
            {
                priceatLastFill = decimal.Round(coin.order.Average(x => x.fills.Average(y => Convert.ToDecimal(y.price))), 2);
                dateofLastOrder = DateTime.Parse(coin.order[0].done_at);
                Console.WriteLine("Avg Price at last Order " + priceatLastFill + " at Date Time " + dateofLastOrder);
            }
            else
            {
                Console.WriteLine("Previous order not found for " + coin.Coin);
            }
            Console.WriteLine(Environment.NewLine);
            return buy;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
using TMDInvestment.Helpers;
using System.Net.Http;
using System.Text.Json;
using System.Text.Encodings.Web;
using TMDInvestment.APIProxy;
using System.Net;
using TinyOAuth1;

namespace TMDInvestment.Services
{
    public class ETradeService
    {
        private string _apikey = string.Empty;
        private string _baseUrl = string.Empty;
        private string _apiSecret = string.Empty;
        private IConfiguration EtradeAPI;
        public ETradeService()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            EtradeAPI = configuration.GetSection("ETradeAPI");
            if (EtradeAPI != null)
            {
                _apikey = EtradeAPI.GetSection("sandBoxKey").Value;
                _baseUrl = EtradeAPI.GetSection("sandBoxURL").Value;
                _apiSecret = EtradeAPI.GetSection("sandBoxSecret").Value;

                //_apikey = EtradeAPI.GetSection("productionKey").Value;
                //_baseUrl = EtradeAPI.GetSection("baseURL").Value;
                //_apiSecret = EtradeAPI.GetSection("productionSecret").Value;
                //_passPhase = EtradeAPI.GetSection("passPhase").Value;
                //WebUtility.UrlEncode
                //HttpUtility.UrlEncode
                ETradeAuthenicator.oauth_consumer_key = _apikey;
                ETradeAuthenicator.appSecret = _apiSecret;
                ETradeAuthenicator.oauth_nonce = Guid.NewGuid().ToString().Replace("-","");
                ETradeAuthenicator.oauth_timestamp = ETradeAuthenicator.GetTimeStamp().ToString();
            }
        }

        public async Task<AccessTokenInfo> CallCrap()
        {
            // *** Check if we have saved tokens already, if not do the following: ***

            // Set up the basic config parameters
            var config = new TinyOAuthConfig
            {
                //AccessTokenUrl = "https://api.provider.com/oauth/accessToken",
                //AuthorizeTokenUrl = "https://api.provider.com/oauth/authorize",
                //RequestTokenUrl = "https://api.provider.com/oauth/requestToken",
                //ConsumerKey = "CONSUMER_KEY",
                //ConsumerSecret = "CONSUMER_SECRET"

                AccessTokenUrl = _baseUrl + "oauth/accessToken",
                AuthorizeTokenUrl = _baseUrl + "oauth/authorize",
                RequestTokenUrl = _baseUrl + "oauth/requestToken",
                ConsumerKey = _apikey,
                ConsumerSecret = _apiSecret
            };

            // Use the library
            var tinyOAuth = new TinyOAuth(config);

            // Get the request token and request token secret
            var requestTokenInfo = await tinyOAuth.GetRequestTokenAsync();

            // Construct the authorization url
            var authorizationUrl = tinyOAuth.GetAuthorizationUrl(requestTokenInfo.RequestToken);

            // *** You will need to implement these methods yourself ***
            //await LaunchWebBrowserAsync(authorizationUrl); // Use Process.Start(authorizationUrl), LaunchUriAsync(new Uri(authorizationUrl)) etc...
            //var verificationCode = await InputVerificationCodeAsync(authorizationUrl);

            // *** Important: Do not run this code before visiting and completing the authorization url ***
            //var accessTokenInfo = await tinyOAuth.GetAccessTokenAsync(requestTokenInfo.RequestToken, requestTokenInfo.RequestTokenSecret, verificationCode);
            var accessTokenInfo = await tinyOAuth.GetAccessTokenAsync(requestTokenInfo.RequestToken, requestTokenInfo.RequestTokenSecret);
            return accessTokenInfo;
        }

        public dynamic GetRequestToken(ref dynamic error)
        {
            dynamic results = null;
            string url = "oauth/request_token";
            HttpRequestMessage request;
            try
            {
                request = GetAuthenicatedRequest(HttpMethod.Get, _baseUrl + url, null, ref error);
                results = APIProxy<dynamic>.Send(request, ref error);
                //if (results == null || Errors.HasErrors(error))
                //{
                //    return error;
                //}
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return results;
        }

        private HttpRequestMessage GetAuthenicatedRequest(HttpMethod method, string url, dynamic model, ref dynamic error)
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
            if (model != null && ((JsonElement)model).GetType() != null)
                jsonString = JsonSerializer.Serialize(model, options);

            //ETradeAuthenicator.oauth_signature = ETradeAuthenicator.GenerateSignature(ETradeAuthenicator.oauth_consumer_key, ETradeAuthenicator.oauth_timestamp.ToString(), ETradeAuthenicator.oauth_nonce, ETradeAuthenicator.oauth_signature_method, method, url, jsonString, _apiSecret);
            ETradeAuthenicator.oauth_signature = ETradeAuthenicator.GenerateSignature(ETradeAuthenicator.oauth_consumer_key, ETradeAuthenicator.appSecret, "", "", ETradeAuthenicator.oauth_timestamp.ToString(), ETradeAuthenicator.oauth_nonce, method, url);
            //ETradeAuthenicator.oauth_signature = ETradeAuthenicator.GenerateSignature("c5bb4dcb7bd6826c7c4340df3f791188", "7d30246211192cda43ede3abd9b393b9", "VbiNYl63EejjlKdQM6FeENzcnrLACrZ2JYD6NQROfVI=", "XCF9RzyQr4UEPloA+WlC06BnTfYC1P0Fwr3GUw/B0Es=", "1344885636", "0bba225a40d1bbac2430aa0c6163ce44", method, "https://etws.etrade.com/accounts/rest/accountlist");
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            //request.Headers.Add("Authorization", "OAuth realm = \"\"");
            //request.Headers.Add("oauth_consumer_key", ETradeAuthenicator.oauth_consumer_key);
            //request.Headers.Add("oauth_token", "");
            //request.Headers.Add("oauth_signature_method", ETradeAuthenicator.oauth_signature_method);
            //request.Headers.Add("oauth_signature", ETradeAuthenicator.oauth_signature);
            //request.Headers.Add("oauth_timestamp", ETradeAuthenicator.oauth_timestamp.ToString());
            //request.Headers.Add("oauth_nonce", ETradeAuthenicator.oauth_nonce);
            //request.Headers.Add("oauth_version", "1.0");
            //request.Headers.Add("oauth_callback", ETradeAuthenicator.oauth_callback);
            //request.Headers.Add("Authorization", "OAuth realm=\"\",oauth_callback=\"oob\"," +
            //"oauth_signature=\"" + ETradeAuthenicator.oauth_signature + "\",oauth_nonce=\"" + ETradeAuthenicator.oauth_nonce + "\"," +
            //"oauth_signature_method=\"HMAC-SHA1\",oauth_consumer_key=\"" + ETradeAuthenicator.oauth_consumer_key + "\"," +
            //"oauth_timestamp=\"" + ETradeAuthenicator.oauth_timestamp.ToString() +"\"");
            //request.Headers.Add("User-Agent", "TMDInvestment/1.0");
            //request.Headers.Add("Accept", "application/json");

            //"oauth_signature=\"FjoSQaFDKEDK1FJazlY3xArNflk%3D\",oauth_nonce=\"LTg2ODUzOTQ5MTEzMTY3MzQwMzE%3D\"," +
            //"oauth_signature_method=\"HMAC-SHA1\", oauth_consumer_key=\"282683cc9e4b8fc81dea6bc687d46758\"," +
            //"oauth_timestamp=\"" + WebUtility.UrlEncode(ETradeAuthenicator.oauth_timestamp.ToString()) + "\",oauth_token=\"\"," +
            //"oauth_timestamp=\"1273254425\"");
            //request.Headers.Add("Authorization", "OAuth realm=\"" + _baseUrl + "\"," +
            //request.Headers.Add("Authorization", "OAuth ," +
            request.Headers.Add("Authorization", "OAuth realm=\"\"," +
                //request.Headers.Add("Authorization", "OAuth," +
                "oauth_consumer_key=\"" + (ETradeAuthenicator.oauth_consumer_key) + "\"," +
                "oauth_timestamp=\"" + (ETradeAuthenicator.oauth_timestamp.ToString()) + "\"," +
                "oauth_nonce=\"" + (ETradeAuthenicator.oauth_nonce) + "\"," +
                "oauth_signature_method=\"HMAC-SHA1\"," +
                "oauth_callback=\"oob\"," +
                "oauth_signature=\"" + (ETradeAuthenicator.oauth_signature) + "\"," +
                "oauth_version=\"1.0\"");




            //"oauth_version=\"1.0\"");

            //if (!string.IsNullOrEmpty(jsonString))
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/x-www-form-urlencoded");
            return request;
        }
    }
}

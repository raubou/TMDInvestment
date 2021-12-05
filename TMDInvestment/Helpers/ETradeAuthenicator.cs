using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TMDInvestment.Helpers
{
    //Authorization: OAuth realm = "", oauth_callback = "oob",
    //oauth_signature = "FjoSQaFDKEDK1FJazlY3xArNflk%3D", oauth_nonce = "LTg2ODUzOTQ5MTEzMTY3MzQwMzE%3D",
    //oauth_signature_method = "HMAC-SHA1", oauth_consumer_key = "282683cc9e4b8fc81dea6bc687d46758",
    //oauth_timestamp = "1273254425"
    public static class ETradeAuthenicator
    {
        public static string oauth_consumer_key = string.Empty; //string Required    The value used by the consumer to identify itself to the service provider.
        public static string oauth_timestamp = string.Empty; //integer Required The date and time of the request, in epoch time.Must be accurate within five minutes.
        public static string oauth_nonce = string.Empty; //string Required  guid  A nonce, as described in OAuth 1.0a documentation - roughly, an arbitrary or random value that cannot be used again with the same timestamp.
        public static string oauth_signature_method = "HMAC-SHA1"; //string Required    The signature method used by the consumer to sign the request.The only supported option is HMAC-SHA1.
        public static string oauth_signature = string.Empty; //string Required    Signature generated with the shared secret and token secret using the specified oauth_signature_method, as described in OAuth documentation.
        public static string oauth_token = string.Empty; //string Required    The consumer’s access token issued by the service provider.
        public static string oauth_callback = "oob";
        public static string oauth_version = "1.0";
        public static string appSecret = string.Empty;
        //public static string GenerateSignature(string consumerKey, string timeStamp, string atNonce, string signatureMethod, HttpMethod method, string url, string body, string appSecret)
        public static string GenerateSignature(string consumerKey, string appSecret, string accessToken, string accessSecret, string timeStamp, string atNonce, HttpMethod method, string url)
        {
            //()
            var sb = new StringBuilder();            
            sb.Append(url);
            sb.Append("&" + method.ToString().ToUpper());
            sb.Append("&" + "oauth_consumer_key=" + consumerKey);
            sb.Append("&" + ("oauth_timestamp=" + timeStamp));
            sb.Append("&" + ("oauth_nonce=" + atNonce));
            sb.Append("&" + ("oauth_signature_method=" + oauth_signature_method));
            sb.Append("&" + ("oauth_callback=" + oauth_callback));
            sb.Append("&" + ("oauth_version=" + oauth_version));


            //if (!string.IsNullOrEmpty(atNonce))


            //if (!string.IsNullOrEmpty(consumerKey))

            //if (!string.IsNullOrEmpty(accessToken))
            //    sb.Append("&" + ("oauth_token=" + accessToken));
            //if (!string.IsNullOrEmpty(accessSecret))
            //    sb.Append("&" + ("oauth_token_secret=" + accessSecret));
            //if (!string.IsNullOrEmpty(authMethod))

            //if (!string.IsNullOrEmpty(timeStamp))


            //sb.Append("&" + ("oauth_token="));

            //if (!string.IsNullOrEmpty(accessSecret))
            //    sb.Append("&" + (accessSecret));



            //if (!string.IsNullOrEmpty(method.ToString()))
            //sb.Append("&" + (method.ToString().ToUpper()));
            //if (!string.IsNullOrEmpty(url))

            //sb.Append("&" + signatureMethod);
            //{ "oauth_token", token },
            //{ "oauth_verifier", oauthVerifier }
            //return (GetHMACInHex(appSecret, sb.ToString()));
            //return GetHMACInHex(Uri.EscapeDataString(appSecret), Uri.EscapeDataString(sb.ToString()));
            return GetHMACInHex(HttpUtility.UrlEncode(appSecret), HttpUtility.UrlEncode(sb.ToString()));
        }

        private static string GetHMACInHex(string key, string data)
        {
            //var hmacKey = Encoding.UTF8.GetBytes(key);
            var hmacKey = Convert.FromBase64String(key);
            var dataBytes = Encoding.ASCII.GetBytes(data);

            using (var hmac = new HMACSHA256(hmacKey))
            {
                hmac.Initialize();
                var sig = hmac.ComputeHash(dataBytes);
                //return sig.ByteArrayToHexString();
                //return ByteToHexString(sig);
                return Uri.EscapeDataString(Convert.ToBase64String(sig));
            }
        }

        public static int GetTimeStamp()
        {
            int secondsSinceEpoch = 0;
            try
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                secondsSinceEpoch = (int)t.TotalSeconds;
                return secondsSinceEpoch;
            }
            catch (Exception ex)
            {
                //error = ex;
            }
            return secondsSinceEpoch;
        }
    }
}

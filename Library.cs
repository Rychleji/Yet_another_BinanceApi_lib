using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace BinanceApiLib
{
    public static class BApiLib
    {
        private static string CreateSignature(string queryString, string secret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(queryStringBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private static long GetTimestamp()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        public static HttpClient CreateHttpClient(String apiKey, String url = @"https://testnet.binancefuture.com/")
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            client.BaseAddress = new Uri(url);

            return client;
        }

        public static string FuturesGetBalance(HttpClient httpClient, String secretKey, int recvWindow = 5000)
        {
            string arguments = $"recvWindow={recvWindow}&timestamp={GetTimestamp()}";
            string endpoint = "fapi/v2/balance?";
            String signature = CreateSignature(arguments, secretKey);
            string query = arguments + "&signature=" + signature;

            //httpClient.BaseAddress = new Uri(query);

            var response = httpClient.GetAsync(endpoint + query);
            return response.Result.Content.ReadAsStringAsync().Result;
        }

        public static string FuturesGetBalance(String apiKey, String secretKey, String url = @"https://testnet.binancefuture.com/", int recvWindow = 5000)
        {
            return FuturesGetBalance(CreateHttpClient(apiKey, url), secretKey, recvWindow);
        }
    }
}
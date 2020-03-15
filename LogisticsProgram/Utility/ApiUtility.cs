using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LogisticsProgram
{
    public class ApiUtility
    {
        private static ApiUtility apiUtility;

        private static HttpClient client;
        private static readonly String BASE_URL = "https://api.tomtom.com/";
        private static readonly String APP_KEY = "rOwvGPEEBP70iDS2ohHSlqzBF1sp8d7z";

        private ApiUtility()
        {
            client = new HttpClient();
        }

        public static ApiUtility GetInstance()
        {
            return apiUtility ?? (apiUtility = new ApiUtility());
        }

        public async Task<HttpResponseMessage> GetSearchedAddresses(String value)
        {
            return await client.GetAsync($"{BASE_URL}search/2/search/{value}.json?typeahead=true&idxSet=PAD&countrySet=RU&key={APP_KEY}");
        }

        public async Task<HttpResponseMessage> GetRoute(String valueFrom, String valueTo)
        {
            return await client.GetAsync($"{BASE_URL}routing/1/calculateRoute/{valueFrom}:{valueTo}/json?maxAlternatives=1&key={APP_KEY}");
        }
    }
}

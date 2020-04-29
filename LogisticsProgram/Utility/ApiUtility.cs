using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using SimpleJSON;

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

        public async Task<Period[,]> GetPeriodMatrix(List<Position> positions)
        {
            var requestBodyRaw = "{\"origins\": [";
            foreach (Position position in positions)
            {
                String[] latlong = position.Address.AddressValue.Split(',');
                String latitude = latlong[0];
                String longitude = latlong[1];
                requestBodyRaw += "{\"point\": {\"latitude\": " + latitude + ", \"longitude\": " + longitude + "}},";
            }

            requestBodyRaw = requestBodyRaw.Remove(requestBodyRaw.Length - 1); //Delete last comma
            requestBodyRaw += "],\"destinations\": [";
            foreach (Position position in positions)
            {
                String[] latlong = position.Address.AddressValue.Split(',');
                String latitude = latlong[0];
                String longitude = latlong[1];
                requestBodyRaw += "{\"point\": {\"latitude\": " + latitude + ", \"longitude\": " + longitude + "}},";
            }

            requestBodyRaw = requestBodyRaw.Remove(requestBodyRaw.Length - 1); //Delete last comma
            requestBodyRaw += "]}";
            var requestBody = new StringContent(requestBodyRaw, Encoding.UTF8, "application/json");
            var response =
                await client.PostAsync(
                    $"{BASE_URL}routing/1/matrix/sync/json?routeType=shortest&computeTravelTimeFor=all&key={APP_KEY}", requestBody);
            var strResponse = response.Content.ReadAsStringAsync().Result;
            var N = JSON.Parse(strResponse);
            Period[,] periodMatrix = new Period[positions.Count,positions.Count];
            for (int i = 0; i < positions.Count; i++)
            {
                for (int j = 0; j < positions.Count; j++)
                {
                    //TODO status code check
                    var timeInSeconds = N["matrix"][i][j]["response"]["routeSummary"]["travelTimeInSeconds"];
                    //var builder = new PeriodBuilder();
                    var timeBetween = Period.FromSeconds(timeInSeconds);
                    periodMatrix[i, j] = timeBetween;
                }
            }

            return periodMatrix;
        }
        /*
        public async Task<HttpResponseMessage> GetRoute(String valueFrom, String valueTo)
        {
            return await client.GetAsync($"{BASE_URL}routing/1/calculateRoute/{valueFrom}:{valueTo}/json?maxAlternatives=1&key={APP_KEY}");
        }*/
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
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
        private static readonly string BASE_URL = "https://api.tomtom.com/";
        private static readonly string APP_KEY = "rOwvGPEEBP70iDS2ohHSlqzBF1sp8d7z";

        private ApiUtility()
        {
            client = new HttpClient();
        }

        public static ApiUtility GetInstance()
        {
            return apiUtility ?? (apiUtility = new ApiUtility());
        }

        public async Task< /*Async*/ObservableCollection<AddressModel.AddressVariant>> GetSearchedAddresses(
            string address)
        {
            var encValue = WebUtility.UrlEncode(address);
            var response =
                await client.GetAsync(
                    $"{BASE_URL}search/2/search/{encValue}.json?idxSet=PAD&countrySet=RU&key={APP_KEY}");
            if (response.IsSuccessStatusCode)
            {
                /*Async*/
                var result = new /*Async*/ObservableCollection<AddressModel.AddressVariant>();
                var strResponse = await response.Content.ReadAsStringAsync();
                var N = JSON.Parse(strResponse);
                for (var i = 0; i < N["results"].Count; i++)
                {
                    string freeFormAddress = N["results"][i]["address"]["freeformAddress"];
                    var lat = N["results"][i]["position"]["lat"].Value;
                    var lon = N["results"][i]["position"]["lon"].Value;
                    var addr = $"{lat},{lon}";
                    result.Add(new AddressModel.AddressVariant(freeFormAddress, addr));
                }

                return result;
            }

            throw new Exception("Request wasn't successful");
        }

        public async Task<Period[,]> GetPeriodMatrix(List<Position> positions)
        {
            var requestBodyRaw = "{\"origins\": [";
            foreach (var position in positions)
            {
                var latlong = position.Address.AddressValue.Split(',');
                var latitude = latlong[0];
                var longitude = latlong[1];
                requestBodyRaw += "{\"point\": {\"latitude\": " + latitude + ", \"longitude\": " + longitude + "}},";
            }

            requestBodyRaw = requestBodyRaw.Remove(requestBodyRaw.Length - 1); //Delete last comma
            requestBodyRaw += "],\"destinations\": [";
            foreach (var position in positions)
            {
                var latlong = position.Address.AddressValue.Split(',');
                var latitude = latlong[0];
                var longitude = latlong[1];
                requestBodyRaw += "{\"point\": {\"latitude\": " + latitude + ", \"longitude\": " + longitude + "}},";
            }

            requestBodyRaw = requestBodyRaw.Remove(requestBodyRaw.Length - 1); //Delete last comma
            requestBodyRaw += "]}";
            var requestBody = new StringContent(requestBodyRaw, Encoding.UTF8, "application/json");
            var response =
                await client.PostAsync(
                    $"{BASE_URL}routing/1/matrix/sync/json?routeType=shortest&computeTravelTimeFor=all&key={APP_KEY}",
                    requestBody);
            var strResponse = response.Content.ReadAsStringAsync().Result;
            var N = JSON.Parse(strResponse);
            var periodMatrix = new Period[positions.Count, positions.Count];
            for (var i = 0; i < positions.Count; i++)
            for (var j = 0; j < positions.Count; j++)
            {
                //TODO status code check
                var timeInSeconds = N["matrix"][i][j]["response"]["routeSummary"]["travelTimeInSeconds"];
                //var builder = new PeriodBuilder();
                var timeBetween = Period.FromSeconds(timeInSeconds);
                periodMatrix[i, j] = timeBetween;
            }

            return periodMatrix;
        }
    }
}
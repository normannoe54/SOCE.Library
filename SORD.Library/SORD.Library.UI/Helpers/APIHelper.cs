using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace SORD.Library.UI.Helpers
{
    public static class APIHelper
    {
        //yes this is where the api route lives for now LOL
        public static string baseapiroute = "https://localhost:44369";

        /// <summary>
        /// Default APICall with json body casted to an object output
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uripath"></param>
        /// <param name="methodtype"></param>
        /// <param name="jsonbody"></param>
        /// <param name="authtoken"></param>
        /// <returns></returns>
        public static async Task<T> ApiCall<T>(string uripath, HttpMethod methodtype, string jsonbody, string authtoken = "")
        {
            //setup client for api request
            HttpClient client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = methodtype,
                RequestUri = new Uri($"{baseapiroute }/{uripath}"),
                Content = new StringContent(jsonbody, Encoding.UTF8, "application/json"),
                //add authentication
            };

            var response = await client.SendAsync(request).ConfigureAwait(false);

            T responsefinal = default(T);

            //If successfully login - save token 
            if (response.IsSuccessStatusCode)
            {
                responsefinal = response.Content.ReadAsAsync<T>().Result;
            }

            return responsefinal;
        }
    }
}

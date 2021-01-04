using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SORD.Library.Access.Models;

namespace SORD.Library.Access.Helpers
{
    public class APIHelper
    {
        private HttpClient _apiClient { get; set; }
        private LoggedInUser _userModel { get; set; }

        public APIHelper(LoggedInUser userModel)
        {
            _userModel = userModel;

            string api = "";
            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task Authenticate(string username, string password)
        {
            KeyValuePair<string,string> k1 = new KeyValuePair<string, string>("granttype", "password");
            KeyValuePair<string, string> k2 = new KeyValuePair<string, string>("username", username);
            KeyValuePair<string, string> k3 = new KeyValuePair<string, string>("password", password);

            KeyValuePair<string,string>[] keys = new KeyValuePair<string, string>[] { k1, k2, k3 };

            var data = new FormUrlEncodedContent(keys);

            HttpResponseMessage response = await _apiClient.PostAsync("/Token", data);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage response = await _apiClient.GetAsync("/api/User");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoggedInUser>();
                _userModel.FirsName = result.FirsName;
                _userModel.LastName = result.LastName;
                _userModel.EmailAdress = result.EmailAdress;
                _userModel.CreatedDate = result.CreatedDate;
                _userModel.Id = result.Id;
                _userModel.Token = result.Token;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}

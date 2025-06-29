using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace accgame
{
    public class GoogleApiService
    {
        private readonly string _clientId = "590585545691-ut62tli8feukqj9ns2u7c4l0t7khdft7.apps.googleusercontent.com";
        private readonly string _clientSecret = "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";
        private readonly string _redirectUri = "https://wibugame.net/Account/OAuth2Callback"; // Đảm bảo URL trùng với URL trong Google API Console

        public string GetAuthorizationUrl()
        {
            var authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
            var scope = "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";
            var url = $"{authorizationEndpoint}?response_type=code&client_id={_clientId}&redirect_uri={_redirectUri}&scope={scope}&access_type=offline";
            return url;
        }

        public async Task<string> ExchangeCodeForTokenAsync(string code)
        {
            var tokenRequestUri = "https://oauth2.googleapis.com/token";
            var tokenRequestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(tokenRequestUri, tokenRequestBody);
                var responseString = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

                if (tokenResponse.TryGetValue("access_token", out var accessToken))
                {
                    return accessToken;
                }
                throw new Exception("Unable to retrieve access token.");
            }
        }

        public async Task<Userinfo> GetUserInfoAsync(string accessToken)
        {
            var userInfoRequestUri = "https://www.googleapis.com/oauth2/v2/userinfo";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(userInfoRequestUri);
                var responseString = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<Userinfo>(responseString);

                return userInfo;
            }
        }
    }

    public class Userinfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HXRd.Google.AccessTokenAuthorization.Models;

namespace HXRd.Google.AccessTokenAuthorization
{
    public class GoogleHttpClient
    {
        private HttpClient _client;
        private ILogger<GoogleHttpClient> _logger;

        public GoogleHttpClient(HttpClient client, ILogger<GoogleHttpClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GoogleTokenInfo> GetTokenInfo(string token)
        {
            var url = $"oauth2/v3/tokeninfo";
            var accessTokenDict = new Dictionary<string, string>
            {
                { "access_token", token}
            };
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(accessTokenDict) };
            var result = await _client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                var errorMessage = await result.Content.ReadAsStringAsync();
                throw new TokenInfoException(errorMessage);
            }
            var json = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleTokenInfo>(json);
        }

        public async Task<GooglePeople> GetMe(string token)
        {
            var url = $"plus/v1/people/me";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");
            var result = await _client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                var errorMessage = await result.Content.ReadAsStringAsync();
                throw new UserInfoException(errorMessage);
            }
            var json = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GooglePeople>(json);
        }
    }
}

using LionwoodSoftware.BaseApiClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RouterApi.ApiClients.Identity
{
    public partial class IdentityApiClient : ApiBaseClient
    {
        public IdentityApiClient(IHttpClientFactory httpClientFactory, string baseUrl) : base(httpClientFactory, baseUrl) { }

        private Task<string> GetAccessTokenAsync()
        {
            string apiClient = "identity";
            string authorityUrl = Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY");
            string clientId = Environment.GetEnvironmentVariable("ROUTER_CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("ROUTER_CLIENT_SECRET");
            string clientScopes = Environment.GetEnvironmentVariable("IDENTITY_API_SCOPES");

            return AuthorityToken.GetAccessTokenAsync(apiClient, authorityUrl, clientId, clientSecret, clientScopes);
        }
    }
}

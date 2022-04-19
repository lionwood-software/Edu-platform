using LionwoodSoftware.BaseApiClient;
using RouterApi.ApiClients.Identity.Models.Users;
using System.Threading.Tasks;

namespace RouterApi.ApiClients.Identity
{
    public partial class IdentityApiClient : ApiBaseClient
    {
        public async Task<UserVM> GetUserAsync(string id)
        {
            string token = await GetAccessTokenAsync();

            return await GetAsync<UserVM>($"api/v1/users/{id}", token);
        }

        public async Task<UserVM> GetUserByCodeAsync(string code)
        {
            string token = await GetAccessTokenAsync();

            try
            {
                return await GetAsync<UserVM>($"api/v1/users/code/{code}", token);
            }
            catch (NotFoundException)
            {
                return null;
            }
        }
    }
}

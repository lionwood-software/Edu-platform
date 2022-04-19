using IdentityServer.Models.System;
using LionwoodSoftware.DataFilter;
using System.Threading.Tasks;

namespace IdentityServer.Interfaces
{
    public interface ISystemService
    {
        Task<PrivateUserVM> GetPrivateUserByIdAsync(string id, bool withRoles = false, bool withImage = false);

        Task<KendoResponse<PrivateUserVM>> GetPrivateUsersAsync(KendoDataFilter filter, bool withRoles = false, bool withImage = false);
    }
}

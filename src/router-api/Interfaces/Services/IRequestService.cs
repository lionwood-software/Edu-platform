using LionwoodSoftware.DataFilter;
using RouterApi.Models.Attachment;
using RouterApi.Models.Request;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RouterApi.Interfaces.Services
{
    public interface IRequestService
    {
        Task<string> CreateChildRequestAsync(string userId, CreateChildRequestVM model);

        Task<string> CreateSchoolRequestAsync(string userId, CreateSchoolRequestVM model);

        Task<string> CreateTeacherRequestAsync(string userId, CreateTeacherRequestVM model);

        Task<string> CreateMarkRequestAsync(string userId, CreateMarkRequestVM model);

        Task<RequestVM> GetRequestByIdAndUserIdAsync(ClaimsPrincipal user, string requestId);

        Task RejectRequestAsync(string userId, string requestId, string token, RejectRequestVM model);

        Task ApproveRequestAsync(string userId, string id, string token);

        Task CancelRequestAsync(CancelRequestVM model);

        Task<KendoResponse<GridRequestVM>> GetByUserIdAsync(ClaimsPrincipal user, KendoDataFilter dataFilter);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models.Attachment;

namespace IdentityServer.Interfaces
{
    public interface IAttachmentService
    {
        Task<string> CreateAsync(CreateAttachmentVM model);

        Task DeleteAsync(string id);

        Task<AttachmentVM> GetByOwnerIdAndAttachmentTypeAsync(string ownerId, string type);

        Task<AttachmentVM> GetByIdAsync(string id);

        Task<List<AttachmentVM>> GetByOwnerIdAndAttachmentTypeAsync(IEnumerable<string> ownerIds, string type);

        Task<bool> UpdateAsync(CreateAttachmentVM model);

        Task<string> GetImgURLByOwnerIdAndAttachmentTypeAsync(string ownerId, string type);

        Task<string> GetImgURLByIdAsync(string id);
    }
}

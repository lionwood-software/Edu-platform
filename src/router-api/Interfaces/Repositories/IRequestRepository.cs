using RouterApi.Domain.Entities.Request;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RouterApi.Interfaces.Repositories
{
    public interface IRequestRepository
    {
        Task<Request> GetByIdAsync(string id);

        Task<Request> GetWhereAsync(Expression<Func<Request, bool>> expression);

        Task<IEnumerable<Request>> GetAllWhereAsync(Expression<Func<Request, bool>> expression);

        Task<string> AddAsync(Request entity);

        Task ReplaceAsync(Request entity);

        Task DeleteAsync(string id);
    }
}

using LionwoodSoftware.Repository.Interfaces;
using MongoDB.Driver;
using RouterApi.Domain.Entities.Request;
using RouterApi.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RouterApi.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly IMongoCollection<Request> _collection;

        public RequestRepository(IRepository repository)
        {
            _collection = repository.GetCollection<Request>();
        }

        public async Task<string> AddAsync(Request entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity.Id;
        }

        public Task<Request> GetWhereAsync(Expression<Func<Request, bool>> expression)
        {
            return _collection.Find(expression).FirstOrDefaultAsync();
        }

        public Task<Request> GetByIdAsync(string id)
        {
            return _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public Task DeleteAsync(string id)
        {
            return _collection.DeleteOneAsync(x => x.Id == id);
        }

        public Task ReplaceAsync(Request entity)
        {
            return _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
        }

        public async Task<IEnumerable<Request>> GetAllWhereAsync(Expression<Func<Request, bool>> expression)
        {
            return (await (await _collection.FindAsync(expression)).ToListAsync()).AsEnumerable();
        }
    }
}

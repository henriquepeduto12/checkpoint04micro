
using MongoDB.Driver;
using UserSessionApi.Infrastructure;
using UserSessionApi.Models;
using Microsoft.Extensions.Options;

namespace UserSessionApi.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
        Task UpdateLastAccessAsync(string id, DateTime lastAccessUtc, CancellationToken ct = default);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoContext context, IOptions<MongoSettings> settings)
        {
            _users = context.Database.GetCollection<User>(settings.Value.UsersCollection);
        }

        public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync(ct);
        }

        public async Task UpdateLastAccessAsync(string id, DateTime lastAccessUtc, CancellationToken ct = default)
        {
            var update = Builders<User>.Update.Set(u => u.UltimoAcesso, lastAccessUtc);
            await _users.UpdateOneAsync(u => u.Id == id, update, cancellationToken: ct);
        }
    }
}

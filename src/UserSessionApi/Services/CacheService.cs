
using System.Text.Json;
using Microsoft.Extensions.Options;
using UserSessionApi.Infrastructure;
using UserSessionApi.Models;
using StackExchange.Redis;

namespace UserSessionApi.Services
{
    public interface ICacheService
    {
        Task<User?> GetUserAsync(string id, CancellationToken ct = default);
        Task SetUserAsync(User user, TimeSpan? ttl = null, CancellationToken ct = default);
        string MakeUserKey(string id);
    }

    public class CacheService : ICacheService
    {
        private readonly RedisConnection _redis;
        private readonly int _defaultTtlMinutes;

        public CacheService(RedisConnection redis, IOptions<RedisSettings> settings)
        {
            _redis = redis;
            _defaultTtlMinutes = settings.Value.DefaultTtlMinutes;
        }

        public string MakeUserKey(string id) => $"user:{id}";

        public async Task<User?> GetUserAsync(string id, CancellationToken ct = default)
        {
            // StackExchange.Redis is sync-over-async; emulate cancellation with timeout in calling layer if needed.
            var key = MakeUserKey(id);
            var value = await _redis.Database.StringGetAsync(key);
            if (value.IsNullOrEmpty) return null;
            try
            {
                return JsonSerializer.Deserialize<User>(value!);
            }
            catch
            {
                // If cache is corrupted, delete and return null to force DB fetch
                await _redis.Database.KeyDeleteAsync(key);
                return null;
            }
        }

        public async Task SetUserAsync(User user, TimeSpan? ttl = null, CancellationToken ct = default)
        {
            var key = MakeUserKey(user.Id);
            var payload = JsonSerializer.Serialize(user);
            var expiry = ttl ?? TimeSpan.FromMinutes(_defaultTtlMinutes);
            await _redis.Database.StringSetAsync(key, payload, expiry);
        }
    }
}

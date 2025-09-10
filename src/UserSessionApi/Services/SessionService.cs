
using Microsoft.Extensions.Logging;
using UserSessionApi.Models;
using UserSessionApi.Repositories;

namespace UserSessionApi.Services
{
    public interface ISessionService
    {
        Task<User?> GetUserProfileAsync(string userId, CancellationToken ct = default);
    }

    public class SessionService : ISessionService
    {
        private readonly ILogger<SessionService> _logger;
        private readonly IUserRepository _repo;
        private readonly ICacheService _cache;

        public SessionService(ILogger<SessionService> logger, IUserRepository repo, ICacheService cache)
        {
            _logger = logger;
            _repo = repo;
            _cache = cache;
        }

        public async Task<User?> GetUserProfileAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("userId is required", nameof(userId));

            try
            {
                // 1) Try cache
                var cached = await _cache.GetUserAsync(userId, ct);
                if (cached is not null)
                {
                    _logger.LogInformation("Cache hit for user {UserId}", userId);
                    return cached;
                }

                _logger.LogInformation("Cache miss for user {UserId}. Fetching from MongoDB.", userId);

                // 2) Fallback to MongoDB
                var user = await _repo.GetByIdAsync(userId, ct);
                if (user is null)
                {
                    _logger.LogWarning("User {UserId} not found in MongoDB.", userId);
                    return null;
                }

                // 3) Update last access and persist
                user.UltimoAcesso = DateTime.UtcNow;
                await _repo.UpdateLastAccessAsync(userId, user.UltimoAcesso, ct);

                // 4) Store in cache with TTL=15m (default)
                await _cache.SetUserAsync(user, ttl: null, ct: ct);

                return user;
            }
            catch (Exception ex)
            {
                // Good practice: log and rethrow or return a safe error to controller
                _logger.LogError(ex, "Error retrieving user profile for {UserId}", userId);
                throw;
            }
        }
    }
}

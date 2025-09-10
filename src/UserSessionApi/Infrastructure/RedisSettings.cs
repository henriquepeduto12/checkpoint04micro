
namespace UserSessionApi.Infrastructure
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = default!;
        public int DefaultTtlMinutes { get; set; } = 15;
    }
}

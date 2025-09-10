
using StackExchange.Redis;
using Microsoft.Extensions.Options;

namespace UserSessionApi.Infrastructure
{
    public class RedisConnection : IDisposable
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        public IDatabase Database => _lazyConnection.Value.GetDatabase();

        public RedisConnection(IOptions<RedisSettings> settings)
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var options = ConfigurationOptions.Parse(settings.Value.ConnectionString);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            });
        }

        public void Dispose()
        {
            if (_lazyConnection.IsValueCreated)
            {
                _lazyConnection.Value?.Dispose();
            }
        }
    }
}

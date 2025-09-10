
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace UserSessionApi.Infrastructure
{
    public class MongoContext
    {
        public IMongoDatabase Database { get; }
        public MongoContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.Database);
        }
    }
}

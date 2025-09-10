
namespace UserSessionApi.Infrastructure
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string Database { get; set; } = default!;
        public string UsersCollection { get; set; } = "users";
    }
}

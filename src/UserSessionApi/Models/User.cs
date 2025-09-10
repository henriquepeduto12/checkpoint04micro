
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserSessionApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonElement("name")]
        public string Nome { get; set; } = default!;

        [BsonElement("email")]
        public string Email { get; set; } = default!;

        [BsonElement("lastAccess")]
        public DateTime UltimoAcesso { get; set; }
    }
}

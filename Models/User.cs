using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SmartWeather.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        // ⭐ THÊM PHÂN QUYỀN
        // Mặc định là User, Admin sẽ được set thủ công
        public string Role { get; set; } = "User"; // User | Admin

        public DateTime CreatedAt { get; set; }

        public List<string> FavoriteCities { get; set; } = new();
    }
}

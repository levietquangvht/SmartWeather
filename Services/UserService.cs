using MongoDB.Driver;
using SmartWeather.Models;
using BCrypt.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWeather.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(MongoDbService mongo)
        {
            _users = mongo.GetCollection<User>("users");
        }

        // ========================
        // USER AUTH
        // ========================

        public async Task<User?> GetByUsername(string username)
        {
            return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task Create(User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.Now;
            user.FavoriteCities = new List<string>();
            user.Role = "User"; // ⭐ mặc định

            await _users.InsertOneAsync(user);
        }

        public bool VerifyPassword(string inputPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        }

        // ========================
        // FAVORITE CITIES
        // ========================

        public async Task AddFavoriteCity(string username, string city)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var update = Builders<User>.Update.AddToSet(u => u.FavoriteCities, city);

            await _users.UpdateOneAsync(filter, update);
        }

        public async Task<List<string>> GetFavoriteCities(string username)
        {
            var user = await GetByUsername(username);
            return user?.FavoriteCities ?? new List<string>();
        }

        public async Task RemoveFavoriteCity(string username, string city)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var update = Builders<User>.Update.Pull(u => u.FavoriteCities, city);

            await _users.UpdateOneAsync(filter, update);
        }

        // ========================
        // ⭐ ADMIN FUNCTIONS
        // ========================

        // Lấy tất cả user (trừ password)
        public async Task<List<User>> GetAllUsers()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        // Set quyền Admin/User
        public async Task SetRole(string username, string role)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var update = Builders<User>.Update.Set(u => u.Role, role);

            await _users.UpdateOneAsync(filter, update);
        }

        // Kiểm tra có phải Admin không
        public async Task<bool> IsAdmin(string username)
        {
            var user = await GetByUsername(username);
            return user != null && user.Role == "Admin";
        }
    }
}

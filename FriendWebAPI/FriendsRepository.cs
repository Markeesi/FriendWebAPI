using System.IO;
using System.Text.Json;

namespace FriendWebAPI
{
    internal class FriendsRepository : IFriendsRepository
    {
        private static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "friends.json");

        public IEnumerable<Friend> GetAllFriends()
        {
            if (File.Exists(FilePath))
            {
                string fileContent = File.ReadAllText(FilePath);
                List<Friend> friends = JsonSerializer.Deserialize<List<Friend>>(fileContent);
                return friends;
            }

            return Enumerable.Empty<Friend>(); // Return an empty collection if file doesn't exist
        }

        // Implement other methods...
    }
}

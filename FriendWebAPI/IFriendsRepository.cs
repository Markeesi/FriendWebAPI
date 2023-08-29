

namespace FriendWebAPI
{
    public interface IFriendsRepository
    {
        IEnumerable<Friend> GetAllFriends();
        // Other method declarations...
    }
}

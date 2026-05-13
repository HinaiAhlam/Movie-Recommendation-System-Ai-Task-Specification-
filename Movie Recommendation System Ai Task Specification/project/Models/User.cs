namespace project.Models
{
    public class User : Person
    {
        public List<string> FavoriteGenres { get; set; } = new();
        public List<int> WatchHistory { get; set; } = new();
        public List<Rating> Ratings { get; set; } = new();

        public User(int id, string username, string password)
            : base(id, username, password)
        {
        }
    }
}
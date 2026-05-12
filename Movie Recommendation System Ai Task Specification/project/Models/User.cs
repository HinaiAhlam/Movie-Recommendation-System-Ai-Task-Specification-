using System.Collections.Generic;

namespace project.Models
{
    public class User : Person
    {
        public List<string> FavoriteGenres { get; set; } = new List<string>();
        public List<int> WatchHistory { get; set; } = new List<int>();

        public User(int id, string username, string password) : base(id, username, password) { }
    }
}
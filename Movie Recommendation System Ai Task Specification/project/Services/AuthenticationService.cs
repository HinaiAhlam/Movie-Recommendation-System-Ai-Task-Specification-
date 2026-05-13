using project.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace project.Services
{
    public class AuthenticationService
    {
        private readonly string filePath = "Data/users.json";
        private List<User> users = new();

        public User? CurrentUser { get; private set; }

        public AuthenticationService()
        {
            Load();
        }

        private void Load()
        {
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory("Data");
                File.WriteAllText(filePath, "[]");
            }

            var json = File.ReadAllText(filePath);
            users = JsonConvert.DeserializeObject<List<User>>(json) ?? new();
        }

        public void SaveUsers()
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public bool Register(string username, string password)
        {
            if (users.Any(u => u.Username == username))
                return false;

            users.Add(new User(users.Count + 1, username, password));
            SaveUsers();
            return true;
        }

        public bool Login(string username, string password)
        {
            CurrentUser = users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            return CurrentUser != null;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        // 🔥 مهم للـ AI
        public List<User> GetUsers() => users;
    }
}
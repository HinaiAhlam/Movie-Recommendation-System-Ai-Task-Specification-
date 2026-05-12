using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using project.Models; // تأكد من مطابقة اسم مشروعك

namespace project.Services
{
    public class AuthenticationService
    {
        private readonly string _filePath = "Data/users.json"; // مسار قاعدة البيانات
        private List<User> _users;
        public User? CurrentUser { get; private set; } // إدارة المستخدم الحالي

        public AuthenticationService()
        {
            _users = new List<User>();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");
                    File.WriteAllText(_filePath, "[]");
                    return;
                }
                var json = File.ReadAllText(_filePath);
                _users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            catch { _users = new List<User>(); }
        }

        private void SaveUsers()
        {
            var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public bool Register(string username, string password)
        {
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return false;

            int newId = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
            var newUser = new User(newId, username, password);
            _users.Add(newUser);
            SaveUsers();
            return true;
        }

        public bool Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        // هذه هي الدالة التي كانت تنقصك وتسببت في الخطأ
        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
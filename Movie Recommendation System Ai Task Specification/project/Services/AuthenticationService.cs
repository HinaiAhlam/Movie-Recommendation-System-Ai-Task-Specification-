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
        // تم تعديل الامتداد ليتوافق مع الصورة المرفقة image_57d218.png
        private readonly string _filePath = "Data/users.josn";
        private List<User> _users;
        public User? CurrentUser { get; private set; }

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
            catch
            {
                _users = new List<User>();
            }
        }

        // تم تغييرها إلى public لكي تستطيع استدعاءها من Program.cs أو أي Service أخرى
        public void SaveUsers()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                // طباعة الخطأ في حال فشل الحفظ (مثلاً الملف مفتوح في برنامج آخر)
                Console.WriteLine($"Error saving to users.josn: {ex.Message}");
            }
        }

        public bool Register(string username, string password)
        {
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return false;

            int newId = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;

            // عند إنشاء مستخدم جديد، يتم تهيئة القوائم الفارغة تلقائياً
            var newUser = new User(newId, username, password);
            _users.Add(newUser);

            SaveUsers(); // حفظ المستخدم الجديد في الملف
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

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
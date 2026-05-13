using project.Models;
using System.Collections.Generic;
using System.Linq;

namespace project.Services
{
    public class SearchService
    {
        private List<Movie> _movies;

        public SearchService(List<Movie> movies)
        {
            // التأكد من تهيئة القائمة حتى لو كانت فارغة لتجنب الـ NullReferenceException
            _movies = movies ?? new List<Movie>();
        }

        public List<Movie> SearchByTitle(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return _movies; // إرجاع كل الأفلام إذا كان مربع البحث فارغاً
            }

            // تحويل نص البحث إلى حروف صغيرة لضمان مرونة البحث
            string lowerQuery = query.Trim().ToLower();

            return _movies
                .Where(m =>
                    // 1. البحث في العنوان
                    (m.Title != null && m.Title.ToLower().Contains(lowerQuery)) ||

                    // 2. البحث في النوع (هذا سيحل مشكلة كلمة Action)
                    (m.Genre != null && m.Genre.ToLower().Contains(lowerQuery)) ||

                    // 3. البحث في المخرج
                    (m.Director != null && m.Director.ToLower().Contains(lowerQuery)) ||

                    // 4. البحث في سنة الإصدار (تحويل الرقم لنص)
                    m.ReleaseYear.ToString().Contains(lowerQuery)
                )
                .ToList();
        }
    }
}
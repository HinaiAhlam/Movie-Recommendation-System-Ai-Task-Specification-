using project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace project.Services
{
    public class MovieService
    {
        private List<Movie> movies = new List<Movie>();

        private string filePath = "Data/movies.json";

        // تحميل الأفلام من الملف
        public void LoadMovies()
        {
            if (!File.Exists(filePath))
            {
                movies = new List<Movie>();
                return;
            }

            string json = File.ReadAllText(filePath);
            movies = JsonSerializer.Deserialize<List<Movie>>(json);
        }

        // حفظ الأفلام
        public void SaveMovies()
        {
            string json = JsonSerializer.Serialize(movies, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        // عرض كل الأفلام
        public List<Movie> GetAllMovies()
        {
            return movies;
        }

        // إضافة فيلم جديد
        public void AddMovie(Movie movie)
        {
            movie.Id = movies.Count + 1;
            movies.Add(movie);
            SaveMovies();
        }

        // البحث بالاسم
        public Movie SearchByTitle(string title)
        {
            return movies.FirstOrDefault(m => m.Title.ToLower().Contains(title.ToLower()));
        }

        // فلترة حسب النوع
        public List<Movie> GetByGenre(string genre)
        {
            return movies
                .Where(m => m.Genre.ToLower() == genre.ToLower())
                .ToList();
        }

        // فلترة حسب السنة
        public List<Movie> GetByYear(int year)
        {
            return movies
                .Where(m => m.ReleaseYear == year)
                .ToList();
        }
    }
}

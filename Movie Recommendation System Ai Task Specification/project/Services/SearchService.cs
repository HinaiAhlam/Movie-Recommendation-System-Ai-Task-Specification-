
using project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using YourProject.Interfaces;
using project.Models;

namespace YourProject.Services
{
    public class SearchService : ISearch
    {
        private List<Movie> _movies;

        // Constructor
        public SearchService(List<Movie> movies)
        {
            _movies = movies;
        }

        // البحث بالاسم
        public List<Movie> SearchByTitle(string title)
        {
            return _movies
                .Where(m => m.Title.ToLower().Contains(title.ToLower()))
                .ToList();
        }

        //  البحث بالتصنيف
        public List<Movie> SearchByGenre(string genre)
        {
            return _movies
                .Where(m => m.Genre.ToLower().Contains(genre.ToLower()))
                .ToList();
        }

        // البحث بالسنة
        public List<Movie> SearchByYear(int year)
        {
            return _movies
                .Where(m => m.ReleaseYear == year)
                .ToList();
        }

        //  البحث بالمخرج
        public List<Movie> SearchByDirector(string director)
        {
            return _movies
                .Where(m => m.Director.ToLower().Contains(director.ToLower()))
                .ToList();
        }

        //  البحث بالتقييم
        public List<Movie> SearchByRating(double rating)
        {
            return _movies
                .Where(m => m.Rating >= rating)
                .OrderByDescending(m => m.Rating)
                .ToList();
        }

        //  بحث ذكي (Smart Search)
        public List<Movie> SmartSearch(string keyword)
        {
            return _movies
                .Where(m =>
                    m.Title.ToLower().Contains(keyword.ToLower()) ||
                    m.Genre.ToLower().Contains(keyword.ToLower()) ||
                    m.Director.ToLower().Contains(keyword.ToLower()) ||
                    m.ReleaseYear.ToString().Contains(keyword)
                )
                .OrderByDescending(m => m.Rating)
                .ToList();
        }

        //  عرض النتائج + معالجة عدم وجود نتائج
        public void DisplayResults(List<Movie> movies)
        {
            if (movies == null || movies.Count == 0)
            {
                Console.WriteLine("❌ No movies found!");
                return;
            }

            Console.WriteLine("\n Search Results:");
            Console.WriteLine("-------------");

            foreach (var movie in movies)
            {
                Console.WriteLine($" {movie.Title} | {movie.Genre} | {movie.ReleaseYear} | ⭐ {movie.Rating}");
            }

            Console.WriteLine("-----------\n");
        }
    }
}
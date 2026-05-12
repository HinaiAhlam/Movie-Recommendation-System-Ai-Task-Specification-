using project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace project.Services
{
    public class MovieService
    {
        private List<Movie> movies = new List<Movie>();
        private readonly string filePath;

        public MovieService()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(baseDirectory, "Data", "movies.json");

            LoadMovies();
        }

        public void LoadMovies()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    movies = new List<Movie>();
                    return;
                }

                string json = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    movies = new List<Movie>();
                    return;
                }

                movies = JsonSerializer.Deserialize<List<Movie>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Loading Movies: " + ex.Message);
                movies = new List<Movie>();
            }
        }

        public void SaveMovies()
        {
            try
            {
                string json = JsonSerializer.Serialize(movies, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Saving Movies: " + ex.Message);
            }
        }

        public List<Movie> GetAllMovies() => movies;

        public void AddMovie(Movie movie)
        {
            movie.Id = movies.Any() ? movies.Max(m => m.Id) + 1 : 1;
            movies.Add(movie);
            SaveMovies();
        }

        // ✅ مهم: البحث يرجّع قائمة
        public List<Movie> SearchByTitle(string title)
        {
            return movies
                .Where(m => m.Title != null &&
                            m.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Movie> GetByGenre(string genre)
        {
            return movies
                .Where(m => m.Genre != null &&
                            m.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Movie> GetByYear(int year)
        {
            return movies.Where(m => m.ReleaseYear == year).ToList();
        }
    }
}
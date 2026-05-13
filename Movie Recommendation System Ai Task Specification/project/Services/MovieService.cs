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
        private List<Movie> movies = new();
        private readonly string filePath;

        public MovieService()
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "movies.json");
            LoadMovies();
        }

        public void LoadMovies()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    File.WriteAllText(filePath, "[]");
                    movies = new List<Movie>();
                    return;
                }

                string json = File.ReadAllText(filePath);

                movies = string.IsNullOrWhiteSpace(json)
                    ? new List<Movie>()
                    : JsonSerializer.Deserialize<List<Movie>>(json) ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading movies: " + ex.Message);
                movies = new List<Movie>();
            }
        }

        public List<Movie> GetAllMovies()
        {
            return movies;
        }

        public Movie? GetById(int id)
        {
            return movies.FirstOrDefault(m => m.Id == id);
        }
    }
}
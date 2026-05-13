using project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace project.Services
{
    public class RatingService
    {
        private List<Rating> ratings = new();
        private readonly string filePath;

        public RatingService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(baseDir, "Data", "ratings.json");
            LoadRatings();
        }

        public List<Rating> GetAllRatings() => ratings;

        // ⭐ NEW: Get ratings for specific movie
        public List<Rating> GetRatingsByMovie(int movieId)
        {
            return ratings.Where(r => r.MovieId == movieId).ToList();
        }

        // ⭐ NEW: Get average rating for movie (مهم جداً للـ AI)
        public double GetAverageRating(int movieId)
        {
            var movieRatings = GetRatingsByMovie(movieId);

            return movieRatings.Any()
                ? movieRatings.Average(r => r.Score)
                : 0;
        }

        // ⭐ NEW: Get user ratings
        public List<Rating> GetUserRatings(string username)
        {
            return ratings.Where(r => r.Username == username).ToList();
        }

        public void LoadRatings()
        {
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                ratings = new List<Rating>();
                return;
            }

            string json = File.ReadAllText(filePath);

            ratings = string.IsNullOrWhiteSpace(json)
                ? new List<Rating>()
                : JsonSerializer.Deserialize<List<Rating>>(json) ?? new List<Rating>();
        }

        public void SaveRatings()
        {
            string json = JsonSerializer.Serialize(ratings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        public void AddRating(string username, int movieId, int score)
        {
            if (score < 1 || score > 5) return;

            var existing = ratings.FirstOrDefault(r =>
                r.Username == username && r.MovieId == movieId);

            if (existing != null)
            {
                existing.Score = score;
                existing.RatedAt = DateTime.Now;
            }
            else
            {
                ratings.Add(new Rating
                {
                    Username = username,
                    MovieId = movieId,
                    Score = score,
                    RatedAt = DateTime.Now
                });
            }

            SaveRatings();
        }
    }
}
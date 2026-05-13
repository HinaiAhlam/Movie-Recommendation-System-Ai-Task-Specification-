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
        private List<Rating> ratings = new List<Rating>();
        private readonly string filePath;

        public RatingService()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(baseDirectory, "Data", "ratings.json");
            LoadRatings();
        }

        public void LoadRatings()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    ratings = new List<Rating>();
                    return;
                }

                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ratings = new List<Rating>();
                    return;
                }

                ratings = JsonSerializer.Deserialize<List<Rating>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Rating>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Loading Ratings: " + ex.Message);
                ratings = new List<Rating>();
            }
        }

        public void SaveRatings()
        {
            try
            {
                string json = JsonSerializer.Serialize(ratings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Saving Ratings: " + ex.Message);
            }
        }

        public void AddRating(string username, int movieId, int score)
        {
            if (score < 1 || score > 5) return;

            var existingRating = ratings.FirstOrDefault(r => r.Username == username && r.MovieId == movieId);
            if (existingRating != null)
            {
                existingRating.Score = score; // إذا كان موجوداً، نحدث التقييم بدلاً من إضافة مكرر
            }
            else
            {
                ratings.Add(new Rating()
                {
                    Username = username,
                    MovieId = movieId,
                    Score = score,
                    RatedAt = DateTime.Now
                });
            }
            SaveRatings();
        }

        // ============================================================
        // التعديل هنا: تغيير النوع إلى bool ليناسب استدعاء Program.cs
        // وتغيير الاسم ليتطابق مع ما هو موجود في الصورة (image_5662b2.png)
        // ============================================================

        public bool UpdateRating(string username, int movieId, int newScore)
        {
            if (newScore < 1 || newScore > 5) return false;

            var rating = ratings.FirstOrDefault(r => r.Username == username && r.MovieId == movieId);
            if (rating == null) return false;

            rating.Score = newScore;
            rating.RatedAt = DateTime.Now; // تحديث التاريخ أيضاً
            SaveRatings();
            return true;
        }

        public bool RemoveRating(string username, int movieId)
        {
            var rating = ratings.FirstOrDefault(r => r.Username == username && r.MovieId == movieId);
            if (rating == null) return false;

            ratings.Remove(rating);
            SaveRatings();
            return true;
        }

        public double GetAverageRating(int movieId)
        {
            var movieRatings = ratings.Where(r => r.MovieId == movieId);
            return movieRatings.Any() ? movieRatings.Average(r => r.Score) : 0;
        }
    }
}
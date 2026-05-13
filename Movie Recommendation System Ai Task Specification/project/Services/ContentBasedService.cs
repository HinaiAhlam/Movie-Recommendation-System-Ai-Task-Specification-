using project.Models;
using project.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace project.Services
{
    public class ContentBasedService : IRecommendation
    {
        private readonly List<Movie> _allMovies;
        private readonly List<Rating> _allRatings;

        public ContentBasedService(List<Movie> movies, List<Rating> ratings)
        {
            _allMovies = movies;
            _allRatings = ratings;
        }

        public List<Movie> GetRecommendations(User user)
        {
            // تحويل ذوق المستخدم إلى Vector بناءً على الأنواع المفضلة
            var userVector = GetUserGenreVector(user);

            var recommendations = _allMovies
                .Where(m => !user.WatchHistory.Contains(m.Id))
                .Select(m => new {
                    Movie = m,
                    Score = CalculateCosineSimilarity(userVector, GetMovieGenreVector(m))
                })
                .OrderByDescending(x => x.Score)
                .Take(5)
                .Select(x => x.Movie)
                .ToList();

            return recommendations;
        }

        // دالة حساب Cosine Similarity الرياضية [cite: 258]
        private double CalculateCosineSimilarity(double[] vectorA, double[] vectorB)
        {
            double dotProduct = 0, magnitudeA = 0, magnitudeB = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += Math.Pow(vectorA[i], 2);
                magnitudeB += Math.Pow(vectorB[i], 2);
            }
            if (magnitudeA == 0 || magnitudeB == 0) return 0;
            return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }

        private double[] GetUserGenreVector(User user)
        {
            // مثال بسيط: تحويل التفضيلات لمصفوفة أرقام (يمكن تطويرها لتشمل كل الأنواع)
            string[] genres = { "Action", "Drama", "Comedy", "Sci-Fi", "Horror" };
            return genres.Select(g => user.FavoriteGenres.Contains(g) ? 1.0 : 0.0).ToArray();
        }

        private double[] GetMovieGenreVector(Movie movie)
        {
            string[] genres = { "Action", "Drama", "Comedy", "Sci-Fi", "Horror" };
            return genres.Select(g => movie.Genre.Contains(g) ? 1.0 : 0.0).ToArray();
        }
    }
}
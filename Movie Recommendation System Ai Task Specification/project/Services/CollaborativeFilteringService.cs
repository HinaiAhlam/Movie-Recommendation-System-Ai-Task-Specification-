using project.Models;
using project.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace project.Services
{
    public class CollaborativeFilteringService : IRecommendation
    {
        private readonly List<User> _allUsers;
        private readonly List<Movie> _allMovies;
        private readonly List<Rating> _allRatings;

        public CollaborativeFilteringService(List<User> users, List<Movie> movies, List<Rating> ratings)
        {
            _allUsers = users;
            _allMovies = movies;
            _allRatings = ratings;
        }

        public List<Movie> GetRecommendations(User user)
        {
            // 1. العثور على مستخدمين آخرين قيموا نفس الأفلام التي قيمها هذا المستخدم
            var userRatings = _allRatings.Where(r => r.Username == user.Username).Select(r => r.MovieId).ToList();

            var similarUsers = _allRatings
                .Where(r => r.Username != user.Username && userRatings.Contains(r.MovieId))
                .Select(r => r.Username)
                .Distinct()
                .ToList();

            // 2. جلب الأفلام التي أعجبت هؤلاء المستخدمين المشابهين ولم يشاهدها مستخدمنا
            var recommendedMovieIds = _allRatings
                .Where(r => similarUsers.Contains(r.Username) && r.Score >= 4 && !userRatings.Contains(r.MovieId))
                .Select(r => r.MovieId)
                .Distinct()
                .ToList();

            return _allMovies.Where(m => recommendedMovieIds.Contains(m.Id)).Take(5).ToList();
        }
    }
}
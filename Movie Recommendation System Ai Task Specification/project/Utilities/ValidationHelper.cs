using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommendationSystem.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username)
                   && username.Length >= 3;
        }

        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password)
                   && password.Length >= 6;
        }

        public static bool IsValidRating(int rating)
        {
            return rating >= 1 && rating <= 5;
        }

        public static bool IsValidMovieId(int id)
        {
            return id > 0;
        }
    }
}
using project.Models;
using System.Collections.Generic;

namespace project.Interfaces
{
    public interface IRecommendation
    {
        // دالة أساسية تعيد قائمة أفلام مقترحة لمستخدم معين
        List<Movie> GetRecommendations(User user);
    }
}
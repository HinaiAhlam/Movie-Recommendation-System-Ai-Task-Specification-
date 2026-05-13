using project.Models;
using System.Collections.Generic;
using System.Linq;

namespace project.Services
{
    public class RecommendationService
    {
        private readonly ContentBasedService _contentService;
        private readonly CollaborativeFilteringService _collabService;

        public RecommendationService(ContentBasedService content, CollaborativeFilteringService collab)
        {
            _contentService = content;
            _collabService = collab;
        }

        public List<Movie> GetFinalRecommendations(User user)
        {
            var contentRecs = _contentService.GetRecommendations(user);
            var collabRecs = _collabService.GetRecommendations(user);

            // دمج النتائج مع إعطاء أولوية للأفلام التي ظهرت في الطريقتين (Weighted Scoring)
            var finalResults = contentRecs.Union(collabRecs)
                .OrderByDescending(m => (contentRecs.Contains(m) ? 2 : 0) + (collabRecs.Contains(m) ? 3 : 0))
                .ToList();

            return finalResults;
        }
    }
}
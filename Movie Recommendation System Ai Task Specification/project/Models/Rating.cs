namespace project.Models
{
    public class Rating
    {
        public string Username { get; set; }

        public int MovieId { get; set; }

        public int Score { get; set; }

        public DateTime RatedAt { get; set; }
    }
}
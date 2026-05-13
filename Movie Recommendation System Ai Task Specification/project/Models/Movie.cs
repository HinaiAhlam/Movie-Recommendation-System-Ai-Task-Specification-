using project.Models;
using System.Collections.Generic;

namespace project.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public string Director { get; set; }

        public List<string> Cast { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}

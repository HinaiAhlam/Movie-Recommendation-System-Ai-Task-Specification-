using project.Models;
using System.Collections.Generic;
using project.Models;

namespace YourProject.Interfaces
{
    public interface ISearch
    {
        List<Movie> SearchByTitle(string title);
        List<Movie> SearchByGenre(string genre);
        List<Movie> SearchByYear(int year);
        List<Movie> SearchByDirector(string director);
        List<Movie> SearchByRating(double rating);

        List<Movie> SmartSearch(string keyword);
    }
}
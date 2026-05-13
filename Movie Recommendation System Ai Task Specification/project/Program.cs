using project.Models;
using project.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using YourProject.Services;

namespace project
{
    class Program
    {
        static void Main(string[] args)
        {
            // تهيئة الخدمات مع معالجة تنبيهات الـ null
            AuthenticationService authService = new AuthenticationService();
            MovieService movieService = new MovieService();
            RatingService ratingService = new RatingService();

            movieService.LoadMovies();

            var allMovies = movieService.GetAllMovies() ?? new List<Movie>();
            SearchService searchService = new SearchService(allMovies);

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("======================================");
                Console.WriteLine("    MOVIE RECOMMENDATION SYSTEM (v1)   ");
                Console.WriteLine("======================================");
                Console.ResetColor();

                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");

                Console.Write("\nSelect an option: ");
                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        HandleRegistration(authService);
                        break;
                    case "2":
                        HandleLogin(authService, movieService, searchService, ratingService);
                        break;
                    case "3":
                        running = false;
                        break;
                    default:
                        ShowError("Invalid selection.");
                        break;
                }
            }
        }

        // --- نظام التقييم المطور (بناءً على image_566a14.png) ---
        static void HandleRatingMenu(AuthenticationService authService, RatingService ratingService, MovieService movieService)
        {
            if (authService.CurrentUser == null) return;

            bool inRatingMenu = true;
            while (inRatingMenu)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("========== RATING SYSTEM ==========");
                Console.ResetColor();
                Console.WriteLine("1. Add/Rate Movie (1-5)");
                Console.WriteLine("2. Update Existing Rating");
                Console.WriteLine("3. Remove Rating");
                Console.WriteLine("4. Back to Dashboard");
                Console.Write("\nSelect action: ");

                string choice = Console.ReadLine()?.Trim() ?? "";
                switch (choice)
                {
                    case "1":
                        ProcessAddRating(authService, ratingService, movieService);
                        break;
                    case "2":
                        ProcessUpdateRating(authService, ratingService);
                        break;
                    case "3":
                        ProcessRemoveRating(authService, ratingService);
                        break;
                    case "4":
                        inRatingMenu = false;
                        break;
                    default:
                        ShowError("Invalid selection.");
                        break;
                }
            }
        }

        static void ProcessAddRating(AuthenticationService authService, RatingService ratingService, MovieService movieService)
        {
            Console.Write("Enter Movie ID to rate: ");
            if (int.TryParse(Console.ReadLine(), out int mId))
            {
                var movie = movieService.GetAllMovies()?.FirstOrDefault(m => m.Id == mId);
                if (movie != null)
                {
                    Console.Write("Enter Rating (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int score) && score >= 1 && score <= 5)
                    {
                        ratingService.AddRating(authService.CurrentUser.Username, mId, score);

                        // تحديث بيانات المستخدم وسجل المشاهدة
                        if (!authService.CurrentUser.WatchHistory.Contains(mId))
                            authService.CurrentUser.WatchHistory.Add(mId);
                        if (!authService.CurrentUser.FavoriteGenres.Contains(movie.Genre))
                            authService.CurrentUser.FavoriteGenres.Add(movie.Genre);

                        authService.SaveUsers();
                        Console.WriteLine("\n[✔] Rating added and user profile updated!");
                    }
                    else { ShowError("Rating must be between 1 and 5."); }
                }
                else { ShowError("Movie not found."); }
            }
            Console.ReadKey();
        }

        static void ProcessUpdateRating(AuthenticationService authService, RatingService ratingService)
        {
            Console.Write("Enter Movie ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int mId))
            {
                Console.Write("Enter New Rating (1-5): ");
                if (int.TryParse(Console.ReadLine(), out int newScore) && newScore >= 1 && newScore <= 5)
                {
                    bool updated = ratingService.UpdateRating(authService.CurrentUser.Username, mId, newScore);
                    if (updated) Console.WriteLine("\n[✔] Rating updated successfully!");
                    else ShowError("No existing rating found for this movie.");
                }
            }
            Console.ReadKey();
        }

        static void ProcessRemoveRating(AuthenticationService authService, RatingService ratingService)
        {
            Console.Write("Enter Movie ID to remove: ");
            if (int.TryParse(Console.ReadLine(), out int mId))
            {
                bool removed = ratingService.RemoveRating(authService.CurrentUser.Username, mId);
                if (removed) Console.WriteLine("\n[✔] Rating removed successfully!");
                else ShowError("No rating found to remove.");
            }
            Console.ReadKey();
        }

        // --- الخدمات الأخرى (Dashboard, Login, Search) ---
        static void ShowDashboard(AuthenticationService authService, MovieService movieService, SearchService searchService, RatingService ratingService)
        {
            bool inDashboard = true;
            while (inDashboard)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Phase 3 — User Dashboard");
                Console.ResetColor();

                string currentUserName = authService.CurrentUser?.Username ?? "User";
                Console.WriteLine($"Welcome, {currentUserName}\n");

                Console.WriteLine("1. Browse Movies");
                Console.WriteLine("2. Search Movies");
                Console.WriteLine("3. Rating System (Add/Update/Remove)");
                Console.WriteLine("4. View Recommendations");
                Console.WriteLine("5. Watch History");
                Console.WriteLine("6. Logout");

                Console.Write("\nSelect action: ");
                string dashChoice = Console.ReadLine()?.Trim() ?? "";

                switch (dashChoice)
                {
                    case "1": DisplayAllMovies(movieService); break;
                    case "2": HandleSearch(searchService); break;
                    case "3": HandleRatingMenu(authService, ratingService, movieService); break;
                    case "4":
                        Console.WriteLine("\n[Recommendations feature coming soon...]");
                        Console.ReadKey();
                        break;
                    case "5": ShowWatchHistory(authService, movieService); break;
                    case "6":
                        authService.Logout();
                        inDashboard = false;
                        break;
                    default: ShowError("Invalid selection."); break;
                }
            }
        }

        static void HandleRegistration(AuthenticationService authService)
        {
            Console.Clear();
            Console.Write("Create Username: ");
            string regUser = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Create Password: ");
            string regPass = Console.ReadLine() ?? "";

            if (authService.Register(regUser, regPass))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[✔] Registration Successful!");
                Console.ResetColor();
            }
            else ShowError("Registration failed.");
            Console.ReadKey();
        }

        static void HandleLogin(AuthenticationService authService, MovieService movieService, SearchService searchService, RatingService ratingService)
        {
            Console.Clear();
            Console.Write("Username: ");
            string logUser = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Password: ");
            string logPass = Console.ReadLine() ?? "";

            if (authService.Login(logUser, logPass))
                ShowDashboard(authService, movieService, searchService, ratingService);
            else
            {
                ShowError("Invalid credentials.");
                Console.ReadKey();
            }
        }

        static void HandleSearch(SearchService searchService)
        {
            Console.Clear();
            Console.WriteLine("========= SEARCH SYSTEM =========");
            Console.WriteLine("1. Name | 2. Genre | 3. Year | 4. Director | 5. Rating");
            Console.Write("\nChoose (1-5): ");

            string searchChoice = Console.ReadLine()?.Trim() ?? "";
            List<Movie> results = new List<Movie>();

            switch (searchChoice)
            {
                case "1":
                    Console.Write("Enter name: ");
                    results = searchService.SearchByTitle(Console.ReadLine() ?? "");
                    break;
                case "2":
                    Console.Write("Enter genre: ");
                    results = searchService.SearchByGenre(Console.ReadLine() ?? "");
                    break;
                case "3":
                    Console.Write("Enter year: ");
                    if (int.TryParse(Console.ReadLine(), out int yr)) results = searchService.SearchByYear(yr);
                    break;
                case "4":
                    Console.Write("Enter director: ");
                    results = searchService.SearchByDirector(Console.ReadLine() ?? "");
                    break;
                case "5":
                    Console.Write("Enter min rating: ");
                    if (double.TryParse(Console.ReadLine(), out double rt)) results = searchService.SearchByRating(rt);
                    break;
            }

            searchService.DisplayResults(results);
            Console.ReadKey();
        }

        static void ShowWatchHistory(AuthenticationService authService, MovieService movieService)
        {
            Console.Clear();
            Console.WriteLine("========== WATCH HISTORY ==========\n");
            var history = authService.CurrentUser?.WatchHistory;
            var movies = movieService.GetAllMovies() ?? new List<Movie>();

            if (history == null || history.Count == 0)
                Console.WriteLine("Your history is empty.");
            else
            {
                foreach (var id in history)
                {
                    var movie = movies.Find(m => m.Id == id);
                    Console.WriteLine($"- {(movie != null ? movie.Title : "Unknown Movie")}");
                }
            }
            Console.ReadKey();
        }

        static void DisplayAllMovies(MovieService movieService)
        {
            Console.Clear();
            Console.WriteLine("========== BROWSE MOVIES ==========\n");
            var movies = movieService.GetAllMovies() ?? new List<Movie>();
            foreach (var movie in movies)
            {
                Console.WriteLine($"[{movie.Id}] {movie.Title} ({movie.ReleaseYear})");
            }
            Console.ReadKey();
        }

        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[✘] {message}");
            Console.ResetColor();
            System.Threading.Thread.Sleep(1000);
        }
    }
}
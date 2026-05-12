using project.Models;
using project.Services;
using System;
using System.Collections.Generic;
using YourProject.Services;

namespace project
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthenticationService authService =
                new AuthenticationService();

            // Movie Service
            MovieService movieService =
                new MovieService();

            movieService.LoadMovies();

            // Search Service
            SearchService searchService =
                new SearchService(movieService.GetAllMovies());

            bool running = true;

            while (running)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine("======================================");
                Console.WriteLine("   MOVIE RECOMMENDATION SYSTEM (v1)   ");
                Console.WriteLine("======================================");

                Console.ResetColor();

                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");

                Console.Write("\nSelect an option: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    // ================= REGISTER =================

                    case "1":

                        Console.Write("Create Username: ");
                        string regUser = Console.ReadLine() ?? "";

                        Console.Write("Create Password: ");
                        string regPass = Console.ReadLine() ?? "";

                        if (authService.Register(regUser, regPass))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;

                            Console.WriteLine("\n[✔] Registration Successful!");

                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;

                            Console.WriteLine("\n[✘] Username already exists.");

                            Console.ResetColor();
                        }

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();

                        break;

                    // ================= LOGIN =================

                    case "2":

                        Console.Write("Username: ");
                        string logUser = Console.ReadLine() ?? "";

                        Console.Write("Password: ");
                        string logPass = Console.ReadLine() ?? "";

                        if (authService.Login(logUser, logPass))
                        {
                            ShowDashboard(
                                authService,
                                movieService,
                                searchService);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;

                            Console.WriteLine("\n[✘] Invalid Username or Password.");

                            Console.ResetColor();

                            Console.ReadKey();
                        }

                        break;

                    // ================= EXIT =================

                    case "3":

                        running = false;

                        Console.ForegroundColor = ConsoleColor.Yellow;

                        Console.WriteLine("\nGoodbye!");

                        Console.ResetColor();

                        break;

                    default:

                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine("\nInvalid selection.");

                        Console.ResetColor();

                        System.Threading.Thread.Sleep(1000);

                        break;
                }
            }
        }

        // ================= DASHBOARD =================

        static void ShowDashboard(
            AuthenticationService authService,
            MovieService movieService,
            SearchService searchService)
        {
            bool inDashboard = true;

            while (inDashboard)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(
                    $"===== Welcome, {authService.CurrentUser?.Username} =====");

                Console.ResetColor();

                Console.WriteLine("1. Browse Movies");
                Console.WriteLine("2. Search Movie");
                Console.WriteLine("3. View Recommendations");
                Console.WriteLine("4. Logout");

                Console.Write("\nSelect action: ");

                string dashChoice = Console.ReadLine() ?? "";

                switch (dashChoice)
                {
                    // ================= BROWSE MOVIES =================

                    case "1":

                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Cyan;

                        Console.WriteLine("========== MOVIES LIST ==========\n");

                        Console.ResetColor();

                        var movies = movieService.GetAllMovies();

                        if (movies.Count == 0)
                        {
                            Console.WriteLine("No movies found.");
                        }
                        else
                        {
                            foreach (var movie in movies)
                            {
                                Console.WriteLine($"ID: {movie.Id}");
                                Console.WriteLine($"Title: {movie.Title}");
                                Console.WriteLine($"Genre: {movie.Genre}");
                                Console.WriteLine($"Year: {movie.ReleaseYear}");
                                Console.WriteLine($"Director: {movie.Director}");
                                Console.WriteLine($"Rating: {movie.Rating}");

                                Console.WriteLine("--------------------------------");
                            }
                        }

                        Console.WriteLine("\nPress any key to return...");
                        Console.ReadKey();

                        break;

                    // ================= SEARCH MOVIE =================

                    case "2":

                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Cyan;

                        Console.WriteLine("========= SEARCH MOVIES =========");

                        Console.ResetColor();

                        Console.WriteLine("1. Search By Title");
                        Console.WriteLine("2. Search By Genre");
                        Console.WriteLine("3. Search By Year");
                        Console.WriteLine("4. Search By Director");
                        Console.WriteLine("5. Search By Rating");
                        Console.WriteLine("6. Smart Search");

                        Console.Write("\nChoose Search Type: ");

                        string searchChoice =
                            Console.ReadLine() ?? "";

                        List<Movie> results =
                            new List<Movie>();

                        switch (searchChoice)
                        {
                            // Search By Title
                            case "1":

                                Console.Write("Enter movie title: ");

                                string title =
                                    Console.ReadLine() ?? "";

                                results =
                                    searchService.SearchByTitle(title);

                                break;

                            // Search By Genre
                            case "2":

                                Console.Write("Enter genre: ");

                                string genre =
                                    Console.ReadLine() ?? "";

                                results =
                                    searchService.SearchByGenre(genre);

                                break;

                            // Search By Year
                            case "3":

                                Console.Write("Enter release year: ");

                                int year =
                                    Convert.ToInt32(Console.ReadLine());

                                results =
                                    searchService.SearchByYear(year);

                                break;

                            // Search By Director
                            case "4":

                                Console.Write("Enter director name: ");

                                string director =
                                    Console.ReadLine() ?? "";

                                results =
                                    searchService.SearchByDirector(director);

                                break;

                            // Search By Rating
                            case "5":

                                Console.Write("Enter minimum rating: ");

                                double rating =
                                    Convert.ToDouble(Console.ReadLine());

                                results =
                                    searchService.SearchByRating(rating);

                                break;

                            // Smart Search
                            case "6":

                                Console.Write("Enter keyword: ");

                                string keyword =
                                    Console.ReadLine() ?? "";

                                results =
                                    searchService.SmartSearch(keyword);

                                break;

                            default:

                                Console.ForegroundColor = ConsoleColor.Red;

                                Console.WriteLine("\nInvalid search option.");

                                Console.ResetColor();

                                Console.ReadKey();

                                break;
                        }

                        // Display Results
                        searchService.DisplayResults(results);

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();

                        break;

                    // ================= RECOMMENDATIONS =================

                    case "3":

                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Magenta;

                        Console.WriteLine(
                            "[Recommendation System Coming Soon]");

                        Console.ResetColor();

                        Console.WriteLine(
                            "\n(Waiting for Member 5 AI Recommendation Engine)");

                        Console.WriteLine(
                            "\nPress any key to continue...");

                        Console.ReadKey();

                        break;

                    // ================= LOGOUT =================

                    case "4":

                        authService.Logout();

                        inDashboard = false;

                        Console.ForegroundColor = ConsoleColor.Yellow;

                        Console.WriteLine("\nLogging out...");

                        Console.ResetColor();

                        System.Threading.Thread.Sleep(1000);

                        break;

                    default:

                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine("\nInvalid selection.");

                        Console.ResetColor();

                        System.Threading.Thread.Sleep(1000);

                        break;
                }
            }
        }
    }
}
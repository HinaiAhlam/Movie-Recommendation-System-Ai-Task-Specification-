using project.Services;
using System;


namespace project
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthenticationService authService = new AuthenticationService();

            // Movie Service
            MovieService movieService = new MovieService();
            movieService.LoadMovies();

            bool running = true;

            while (running)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("======================================");
                Console.WriteLine("   MOVIE RECOMMENDATION SYSTEM (v1)   ");
                Console.WriteLine("======================================");
                Console.ResetColor();

                Console.WriteLine("1. Register (New User)");
                Console.WriteLine("2. Login (Existing User)");
                Console.WriteLine("3. Exit");

                Console.Write("\nSelect an option: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    // REGISTER
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

                    // LOGIN
                    case "2":

                        Console.Write("Username: ");
                        string logUser = Console.ReadLine() ?? "";

                        Console.Write("Password: ");
                        string logPass = Console.ReadLine() ?? "";

                        if (authService.Login(logUser, logPass))
                        {
                            ShowDashboard(authService, movieService);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[✘] Invalid Username or Password.");
                            Console.ResetColor();

                            Console.ReadKey();
                        }

                        break;

                    // EXIT
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

        static void ShowDashboard(AuthenticationService authService, MovieService movieService)
        {
            bool inDashboard = true;

            while (inDashboard)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"===== Welcome, {authService.CurrentUser?.Username} =====");
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

                        Console.Write("Enter movie title: ");

                        string title = Console.ReadLine() ?? "";

                        var foundMovie = movieService.SearchByTitle(title);

                        Console.WriteLine();

                        if (foundMovie != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Movie Found!\n");
                            Console.ResetColor();

                            Console.WriteLine($"Title: {foundMovie.Title}");
                            Console.WriteLine($"Genre: {foundMovie.Genre}");
                            Console.WriteLine($"Year: {foundMovie.ReleaseYear}");
                            Console.WriteLine($"Director: {foundMovie.Director}");
                            Console.WriteLine($"Rating: {foundMovie.Rating}");
                            Console.WriteLine($"Description: {foundMovie.Description}");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Movie not found.");
                            Console.ResetColor();
                        }

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();

                        break;

                    // ================= RECOMMENDATIONS =================

                    case "3":

                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("[Recommendation System Coming Soon]");
                        Console.ResetColor();

                        Console.WriteLine("\n(Waiting for Member 5 AI Recommendation Engine)");

                        Console.WriteLine("\nPress any key to continue...");
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
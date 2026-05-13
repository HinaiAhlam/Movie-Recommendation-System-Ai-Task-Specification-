using project.Models;
using project.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // --- 1. تهيئة الخدمات (Services Initialization) ---
            AuthenticationService authService = new AuthenticationService();
            MovieService movieService = new MovieService();
            RatingService ratingService = new RatingService();

            // تحميل بيانات الأفلام من ملف JSON
            movieService.LoadMovies();

            // تهيئة خدمة البحث بناءً على الأفلام المحملة
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

                Console.WriteLine("1. Register (New User)");
                Console.WriteLine("2. Login (Existing User)");
                Console.WriteLine("3. Exit");

                Console.Write("\nSelect an option: ");
                string choice = Console.ReadLine() ?? "";

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
                        Console.WriteLine("\nGoodbye!");
                        break;
                    default:
                        ShowError("Invalid selection.");
                        break;
                }
            }
        }

        // ================= لوحة التحكم (DASHBOARD) =================
        static void ShowDashboard(
            AuthenticationService auth,
            MovieService movieService,
            SearchService searchService,
            RatingService ratingService)
        {
            bool logged = true;

            while (logged)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"===== Welcome, {auth.CurrentUser?.Username} =====");
                Console.ResetColor();

                Console.WriteLine("1. Browse Movies");
                Console.WriteLine("2. Search Movies");
                Console.WriteLine("3. Rate Movie (AI Input)");
                Console.WriteLine("4. AI Recommendations 🤖");
                Console.WriteLine("5. Watch History");
                Console.WriteLine("6. Logout");

                Console.Write("\nChoose action: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": DisplayBrowse(movieService); break;
                    case "2": HandleSearch(searchService); break;
                    case "3": HandleRate(auth, ratingService, movieService); break;
                    case "4": HandleAI(auth, movieService, ratingService); break;
                    case "5": DisplayHistory(auth, movieService); break;
                    case "6":
                        logged = false;
                        auth.Logout();
                        break;
                    default: ShowError("Invalid choice."); break;
                }
            }
        }

        // ================= نظام الـ AI (Member 5 Engine) =================
        static void HandleAI(
            AuthenticationService auth,
            MovieService movieService,
            RatingService ratingService)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("===== AI RECOMMENDATIONS =====\n");
            Console.ResetColor();

            var user = auth.CurrentUser;
            if (user == null) return;

            var movies = movieService.GetAllMovies() ?? new List<Movie>();
            var allRatings = ratingService.GetAllRatings();

            // تجهيز محركات الذكاء الاصطناعي (أكواد الشخص الخامس)
            var content = new ContentBasedService(movies, allRatings);
            var collab = new CollaborativeFilteringService(auth.GetUsers(), movies, allRatings);

            // محرك التوصية النهائي (Hybrid System)
            var recommender = new RecommendationService(content, collab);
            var result = recommender.GetFinalRecommendations(user);

            if (result == null || result.Count == 0)
            {
                Console.WriteLine("No recommendations yet.");
                Console.WriteLine("👉 Tip: Rate more movies so the AI can understand your taste!");
            }
            else
            {
                Console.WriteLine("Based on your preferences, you might like:\n");
                foreach (var m in result)
                    Console.WriteLine($"🎬 {m.Title} | Genre: {m.Genre} | Score: {m.Rating}/5");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // ================= نظام التقييم (الحل لمشكلة الحفظ في JSON) =================
        static void HandleRate(AuthenticationService auth, RatingService ratingService, MovieService movieService)
        {
            Console.Clear();
            Console.WriteLine("========== RATE A MOVIE ==========\n");

            Console.Write("Enter Movie ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var movie = movieService.GetAllMovies()?.FirstOrDefault(m => m.Id == id);
                if (movie != null)
                {
                    Console.Write("Enter Rating (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int score) && score >= 1 && score <= 5)
                    {
                        // 1. إضافة التقييم لخدمة التقييمات
                        ratingService.AddRating(auth.CurrentUser.Username, id, score);

                        // 2. تحديث سجل المشاهدة (Watch History)
                        if (!auth.CurrentUser.WatchHistory.Contains(id))
                            auth.CurrentUser.WatchHistory.Add(id);

                        // 3. تحديث الأنواع المفضلة (Favorite Genres)
                        if (!auth.CurrentUser.FavoriteGenres.Contains(movie.Genre))
                            auth.CurrentUser.FavoriteGenres.Add(movie.Genre);

                        // 4. *** الحل الجوهري ***: حفظ التعديلات فوراً في ملف Users.json
                        auth.SaveUsers();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n[✔] Rating Saved! JSON file and AI profile updated.");
                        Console.ResetColor();
                    }
                    else ShowError("Rating must be 1 to 5.");
                }
                else ShowError("Movie ID not found.");
            }
            else ShowError("Invalid Input.");

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // ================= وظائف مساعدة (Helpers) =================

        static void HandleRegistration(AuthenticationService auth)
        {
            Console.Write("Create Username: ");
            string u = Console.ReadLine() ?? "";
            Console.Write("Create Password: ");
            string p = Console.ReadLine() ?? "";

            if (auth.Register(u, p)) Console.WriteLine("\n[✔] Registered Successfully!");
            else ShowError("User already exists.");
            Console.ReadKey();
        }

        static void HandleLogin(AuthenticationService auth, MovieService m, SearchService s, RatingService r)
        {
            Console.Write("Username: ");
            string u = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            string p = Console.ReadLine() ?? "";

            if (auth.Login(u, p)) ShowDashboard(auth, m, s, r);
            else { ShowError("Invalid login."); Console.ReadKey(); }
        }

        static void DisplayBrowse(MovieService movieService)
        {
            Console.Clear();
            Console.WriteLine("========= ALL MOVIES =========\n");
            var movies = movieService.GetAllMovies();

            if (movies == null || movies.Count == 0)
            {
                ShowError("No movies found in the system.");
            }
            else
            {
                foreach (var m in movies)
                {
                    Console.WriteLine($"[{m.Id}] {m.Title}");
                    Console.WriteLine($"   Genre: {m.Genre} | Year: {m.ReleaseYear} | Rating: {m.Rating}/5");
                    Console.WriteLine("------------------------------");
                }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        static void HandleSearch(SearchService searchService)
        {
            Console.Clear();
            Console.WriteLine("========== MOVIE SEARCH ==========\n");
            Console.Write("Search by Title, Genre, or Year: ");
            var text = Console.ReadLine() ?? "";

            var results = searchService.SearchByTitle(text);

            if (results == null || results.Count == 0)
            {
                ShowError("No matches found.");
            }
            else
            {
                foreach (var m in results)
                    Console.WriteLine($"🎬 {m.Title} | {m.Genre} ({m.ReleaseYear})");
                Console.WriteLine($"\nFound {results.Count} result(s).");
            }
            Console.ReadKey();
        }

        static void DisplayHistory(AuthenticationService auth, MovieService movieService)
        {
            Console.Clear();
            Console.WriteLine("========== WATCH HISTORY ==========\n");
            var history = auth.CurrentUser?.WatchHistory;
            if (history == null || history.Count == 0)
            {
                Console.WriteLine("Your history is empty. Start rating movies!");
            }
            else
            {
                foreach (var id in history)
                {
                    var movie = movieService.GetAllMovies()?.FirstOrDefault(m => m.Id == id);
                    Console.WriteLine($"- {(movie != null ? movie.Title : "Unknown Movie")}");
                }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        static void ShowError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[✘] {msg}");
            Console.ResetColor();
            System.Threading.Thread.Sleep(1000);
        }
    }
}
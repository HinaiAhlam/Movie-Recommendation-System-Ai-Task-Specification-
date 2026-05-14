using project.Models;
using project.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace project
{
    class Program
    {
        static string dbFile = "cinema_empire_vault.json";

        static void Main(string[] args)
        {
            Visuals.Setup();

            AuthenticationService authService = new AuthenticationService();
            MovieService movieService = new MovieService();
            RatingService ratingService = new RatingService();

            movieService.LoadMovies();
            var allMovies = movieService.GetAllMovies() ?? new List<Movie>();
            if (allMovies.Count < 10) allMovies = Generate50Movies();

            Visuals.PlayIntroSound();

            while (true)
            {
                if (authService.CurrentUser == null)
                {
                    // المرحلة 1: شاشة الدخول (اللوجو والخيارات ممركزة تماماً)
                    int startChoice = Visuals.ShowFullCenteredGateway(new[] { "LOGIN SYSTEM", "REGISTER ACCOUNT", "EXIT" });
                    if (startChoice == 2) break;

                    Visuals.Header();
                    string u = Visuals.GetInputBox("USERNAME IDENTIFICATION");
                    string p = Visuals.GetInputBox("SECURITY PASSWORD", true);

                    if (startChoice == 1)
                    {
                        if (authService.Register(u, p)) Visuals.ShowMessage("ACCOUNT CREATED!", ConsoleColor.Green);
                        else Visuals.ShowMessage("USER EXISTS!", ConsoleColor.Red);
                    }
                    else
                    {
                        if (authService.Login(u, p)) Visuals.ShowMessage("ACCESS GRANTED!", ConsoleColor.Cyan);
                        else Visuals.ShowMessage("INVALID LOGIN!", ConsoleColor.Red);
                    }
                }
                else
                {
                    string personality = Visuals.AnalyzeViewerPersonality(authService.CurrentUser, allMovies);
                    string[] hubOpts = { "🎥 EXPLORE 50 MOVIES", "🔎 ADVANCED SEARCH", "⭐ RATE A MOVIE", "🤖 AI TOP PICKS", "🎰 SURPRISE ME", "📜 WATCH HISTORY", "📤 EXPORT DATA", "🚪 LOGOUT" };
                    int c = Visuals.ShowMenu("COMMAND HUB", hubOpts, authService.CurrentUser.Username, personality);

                    var allRatings = ratingService.GetAllRatings();
                    SearchService searchService = new SearchService(allMovies);
                    ContentBasedService contentService = new ContentBasedService(allMovies, allRatings);
                    CollaborativeFilteringService collab = new CollaborativeFilteringService(authService.GetUsers(), allMovies, allRatings);
                    RecommendationService recService = new RecommendationService(contentService, collab);

                    switch (c)
                    {
                        case 0:
                            var selBrowse = Visuals.ShowMovieGrid(allMovies, authService.CurrentUser.Username);
                            if (selBrowse != null) HandleMovieFlow(selBrowse, authService, ratingService);
                            break;
                        case 1:
                            Visuals.Header(authService.CurrentUser.Username);
                            string q = Visuals.GetInputBox("NAME/GENRE/YEAR").ToLower();
                            var res = allMovies.Where(m => m.Title.ToLower().Contains(q) || m.Genre.ToLower().Contains(q) || m.ReleaseYear.ToString() == q).ToList();
                            if (res.Any())
                            {
                                var s = Visuals.ShowMovieGrid(res, authService.CurrentUser.Username, "SEARCH RESULTS");
                                if (s != null) HandleMovieFlow(s, authService, ratingService);
                            }
                            else Visuals.ShowMessage("NOT FOUND!", ConsoleColor.Red);
                            break;
                        case 2:
                            Visuals.Header(authService.CurrentUser.Username);
                            string idInput = Visuals.GetInputBox("ENTER MOVIE ID");
                            if (int.TryParse(idInput, out int mid))
                            {
                                var movie = allMovies.FirstOrDefault(m => m.Id == mid);
                                if (movie != null) HandleMovieFlow(movie, authService, ratingService);
                                else Visuals.ShowMessage("NOT FOUND!", ConsoleColor.Red);
                            }
                            break;
                        case 3:
                            Visuals.Header(authService.CurrentUser.Username);
                            Visuals.ShowProgressBar("AI ANALYZING");
                            var recs = recService.GetFinalRecommendations(authService.CurrentUser);
                            var aiSel = Visuals.ShowMovieGrid(recs, authService.CurrentUser.Username, "AI SMART PICKS");
                            if (aiSel != null) HandleMovieFlow(aiSel, authService, ratingService);
                            break;
                        case 4:
                            var randomMovie = Visuals.ShowSurpriseWheel(allMovies, authService.CurrentUser.Username);
                            if (randomMovie != null) HandleMovieFlow(randomMovie, authService, ratingService);
                            break;
                        case 5:
                            var hMovies = allMovies.Where(m => authService.CurrentUser.WatchHistory.Contains(m.Id)).ToList();
                            Visuals.ShowMovieGrid(hMovies, authService.CurrentUser.Username, "WATCH LOG");
                            break;
                        case 6:
                            Visuals.ExportUserData(authService.CurrentUser, allMovies);
                            break;
                        case 7:
                            authService.Logout();
                            break;
                    }
                }
            }
        }

        static void HandleMovieFlow(Movie m, AuthenticationService auth, RatingService rs)
        {
            Visuals.BatteryLoading($"CHARGING {m.Title}");
            Visuals.Header(auth.CurrentUser.Username);
            string r = Visuals.GetInputBox($"Rate {m.Title} (1-5)");
            if (int.TryParse(r, out int s) && s >= 1 && s <= 5)
            {
                rs.AddRating(auth.CurrentUser.Username, m.Id, s);
                if (!auth.CurrentUser.WatchHistory.Contains(m.Id)) auth.CurrentUser.WatchHistory.Add(m.Id);
                auth.SaveUsers();
                Visuals.TheaterAnimation();
                Visuals.DisplayDigitalTicket(m, s, auth.CurrentUser.Username);
            }
        }

        static List<Movie> Generate50Movies()
        {
            var list = new List<Movie>();
            string[] genres = { "Action", "Sci-Fi", "Drama", "Animation", "Crime", "Comedy" };
            for (int i = 1; i <= 50; i++) list.Add(new Movie { Id = i, Title = "Cinema Hero " + i, Genre = genres[i % 6], ReleaseYear = 2000 + (i % 25), Rating = 4.0 });
            return list;
        }
    }

    public static class Visuals
    {
        public static void Setup()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.SetBufferSize(140, 1000);
                Console.SetWindowSize(Math.Min(140, Console.LargestWindowWidth), Math.Min(45, Console.LargestWindowHeight));
            }
            catch { }
        }

        public static void PlayIntroSound() { try { Console.Beep(200, 300); Thread.Sleep(50); Console.Beep(400, 500); } catch { } }

        public static void DrawSystemBorder()
        {
            int w = Console.WindowWidth; int h = Console.WindowHeight;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(0, 0); Console.Write("┏" + new string('━', w - 3) + "┓");
            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(i % 2 == 0 ? "┃ █" : "┃  ");
                Console.SetCursorPosition(w - 3, i);
                Console.Write(i % 2 == 0 ? "█ ┃" : "  ┃");
            }
            Console.SetCursorPosition(0, h - 1); Console.Write("┗" + new string('━', w - 3) + "┛");
            Console.ResetColor();
        }

        // --- دالة التوسيط الكلية المصلحة (تعيد إظهار العنوان) ---
        public static int ShowFullCenteredGateway(string[] opts)
        {
            int sel = 0; ConsoleKey key;
            string[] logo = {
                @"███╗   ███╗ ██████╗ ██╗   ██╗██╗███████╗     █████╗ ██╗",
                @"████╗ ████║██╔═══██╗██║   ██║██║██╔════╝    ██╔══██╗██║",
                @"██╔████╔██║██║   ██║██║   ██║██║█████╗      ███████║██║",
                @"██║╚██╔╝██║██║   ██║╚██╗ ██╔╝██║██╔══╝      ██╔══██║██║",
                @"██║ ╚═╝ ██║╚██████╔╝ ╚████╔╝ ██║███████╗    ██║  ██║██║",
                @"╚═╝     ╚═╝ ╚═════╝   ╚═══╝  ╚═╝╚══════╝    ╚═╝  ╚═╝╚═╝"
            };
            do
            {
                Console.Clear(); DrawSystemBorder();
                int w = Console.WindowWidth; int h = Console.WindowHeight;

                // ..حساب الطول الإجمالي للمحتوى (اللوجو + الخيارات + فراغات)
                int totalHeight = logo.Length + opts.Length + 4;
                int startY = (h / 2) - (totalHeight / 2);

                //  1.. رسم اللوجو الممركز
                Console.ForegroundColor = ConsoleColor.Red;
                for (int i = 0; i < logo.Length; i++)
                {
                    Console.SetCursorPosition((w / 2) - (logo[i].Length / 2), startY + i);
                    Console.WriteLine(logo[i]);
                }

                // 2. رسم الخيارات الممركزة تحت اللوجو
                for (int i = 0; i < opts.Length; i++)
                {
                    int x = (w / 2) - (opts[i].Length / 2) - 5;
                    Console.SetCursorPosition(x, startY + logo.Length + 2 + i);
                    if (i == sel)
                    {
                        Console.BackgroundColor = ConsoleColor.Red; Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" ► {opts[i]} ");
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"   {opts[i]}   ");
                    }
                    Console.ResetColor();
                }
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow) sel = (sel == 0) ? opts.Length - 1 : sel - 1;
                else if (key == ConsoleKey.DownArrow) sel = (sel == opts.Length - 1) ? 0 : sel + 1;
            } while (key != ConsoleKey.Enter);
            return sel;
        }

        public static void Header(string user = null, string personality = "")
        {
            Console.Clear(); DrawSystemBorder();
            Console.ForegroundColor = ConsoleColor.Red;
            string t = "--- MOVIE AI SYSTEM ---";
            Console.SetCursorPosition((Console.WindowWidth / 2) - (t.Length / 2), 2);
            Console.WriteLine(t);
            if (user != null)
            {
                string st = $" [ VIEWER: {user.ToUpper()} | RANK: {personality} ] ";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (st.Length / 2), 4);
                Console.ForegroundColor = ConsoleColor.Yellow; Console.Write(st);
            }
        }

        public static Movie ShowMovieGrid(List<Movie> list, string user, string title = "EXPLORE")
        {
            int sel = 0, cols = 3; ConsoleKey key;
            if (list == null || list.Count == 0) return null;
            do
            {
                Header(user);
                int start = (sel / 9) * 9;
                for (int i = start; i < Math.Min(start + 9, list.Count); i++)
                {
                    int r = (i - start) / cols, c = i % cols;
                    int x = 12 + (c * 40), y = 7 + (r * 7);
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = (i == sel) ? ConsoleColor.Yellow : ConsoleColor.DarkGray;
                    Console.Write("┏" + new string('━', 30) + "┓");
                    Console.SetCursorPosition(x, y + 1); Console.Write($"┃ {list[i].Title.PadRight(28).Substring(0, 28)} ┃");
                    Console.SetCursorPosition(x, y + 2); Console.Write($"┃ Rating: {list[i].Rating}/5".PadRight(29) + "┃");
                    Console.SetCursorPosition(x, y + 3); Console.Write($"┃ {list[i].Genre.PadRight(28).Substring(0, 28)} ┃");
                    Console.SetCursorPosition(x, y + 4); Console.Write("┗" + new string('━', 30) + "┛");
                }
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.RightArrow && sel < list.Count - 1) sel++;
                else if (key == ConsoleKey.LeftArrow && sel > 0) sel--;
                else if (key == ConsoleKey.DownArrow && sel + cols < list.Count) sel += cols;
                else if (key == ConsoleKey.UpArrow && sel - cols >= 0) sel -= cols;
                else if (key == ConsoleKey.Escape) return null;
            } while (key != ConsoleKey.Enter);
            return list[sel];
        }

        public static void BatteryLoading(string msg)
        {
            Console.Clear(); DrawSystemBorder();
            int x = (Console.WindowWidth / 2) - 15; int y = (Console.WindowHeight / 2) - 2;
            for (int i = 0; i <= 100; i += 20)
            {
                Console.SetCursorPosition(x, y); Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{msg}... {i}%");
                Console.SetCursorPosition(x, y + 2); Console.ForegroundColor = ConsoleColor.White;
                Console.Write("┏━━━━━━━━━━━━━━━━━━━━┓ █");
                Console.SetCursorPosition(x + 1, y + 2);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(new string('█', i / 5)); Thread.Sleep(150);
            }
        }

        public static int ShowMenu(string title, string[] opts, string user = null, string personality = "")
        {
            int sel = 0; ConsoleKey key;
            do
            {
                Header(user, personality);
                Console.SetCursorPosition((Console.WindowWidth / 2) - (title.Length / 2), 7);
                Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine($"--- {title} ---");
                for (int i = 0; i < opts.Length; i++)
                {
                    int p = (Console.WindowWidth / 2) - (opts[i].Length / 2) - 5;
                    Console.SetCursorPosition(Math.Max(0, p), 9 + i);
                    if (i == sel) { Console.BackgroundColor = ConsoleColor.Red; Console.ForegroundColor = ConsoleColor.White; Console.WriteLine($" ► {opts[i].ToUpper()} "); }
                    else { Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine($"   {opts[i]}   "); }
                    Console.ResetColor();
                }
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow) sel = (sel == 0) ? opts.Length - 1 : sel - 1;
                else if (key == ConsoleKey.DownArrow) sel = (sel == opts.Length - 1) ? 0 : sel + 1;
            } while (key != ConsoleKey.Enter);
            return sel;
        }

        public static string GetInputBox(string prompt, bool isP = false)
        {
            int w = 44; int x = (Console.WindowWidth / 2) - (w / 2);
            Console.SetCursorPosition(x, 15); Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔" + new string('═', w - 2) + "╗");
            Console.SetCursorPosition(x, 16); Console.WriteLine($"║ {prompt.PadRight(w - 4)} ║");
            Console.SetCursorPosition(x, 17); Console.Write("║ > ");
            Console.ForegroundColor = ConsoleColor.White;
            string input = isP ? ReadPass() : Console.ReadLine();
            Console.SetCursorPosition(x, 18); Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╚" + new string('═', w - 2) + "╝");
            return input;
        }
        private static string ReadPass() { string p = ""; while (true) { var k = Console.ReadKey(true); if (k.Key == ConsoleKey.Enter) break; if (k.Key == ConsoleKey.Backspace && p.Length > 0) { p = p.Substring(0, p.Length - 1); Console.Write("\b \b"); } else if (!char.IsControl(k.KeyChar)) { p += k.KeyChar; Console.Write("*"); } } Console.WriteLine(); return p; }

        public static void TheaterAnimation()
        {
            int w = Console.WindowWidth;
            for (int i = 0; i < w / 2; i += 8)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                for (int j = 0; j < Console.WindowHeight; j++) { Console.SetCursorPosition(i, j); Console.Write("████"); Console.SetCursorPosition(Math.Max(0, w - i - 4), j); Console.Write("████"); }
                Thread.Sleep(30);
            }
            Console.Clear();
        }

        public static void DisplayDigitalTicket(Movie m, int r, string u)
        {
            Header(u);
            int x = (Console.WindowWidth / 2) - 25;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x, 10); Console.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            Console.SetCursorPosition(x, 11); Console.WriteLine($"┃ MOVIE : {m.Title.PadRight(40)} ┃");
            Console.SetCursorPosition(x, 12); Console.WriteLine($"┃ RATING: {new string('★', r).PadRight(40)} ┃");
            Console.SetCursorPosition(x, 13); Console.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            Console.ReadKey();
        }

        public static Movie ShowSurpriseWheel(List<Movie> m, string u)
        {
            Random r = new Random(); Movie pick = m[r.Next(m.Count)];
            Header(u);
            Console.SetCursorPosition((Console.WindowWidth / 2) - 10, 15);
            Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine(">> SPINNING... <<");
            Thread.Sleep(1000); return pick;
        }

        public static string AnalyzeViewerPersonality(User u, List<Movie> m) { return "Cinema Critic"; }
        public static void ExportUserData(User u, List<Movie> m) { ShowMessage("Report Exported!", ConsoleColor.Green); }
        public static void ShowMessage(string m, ConsoleColor c) { Console.ForegroundColor = c; Console.WriteLine("\n >> " + m); Thread.Sleep(1200); }
        public static void ShowProgressBar(string msg)
        {
            Console.WriteLine(); int x = (Console.WindowWidth / 2) - 15;
            Console.SetCursorPosition(x, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Red; Console.Write(msg + " [");
            for (int i = 0; i < 10; i++) { Console.Write("■"); Thread.Sleep(80); }
            Console.WriteLine("] DONE");
        }
    }
}
using project.Services;
using System;

namespace project
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthenticationService authService = new AuthenticationService();
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
                    case "1":
                        Console.Write("Create Username: ");
                        string regUser = Console.ReadLine() ?? "";
                        Console.Write("Create Password: ");
                        string regPass = Console.ReadLine() ?? "";

                        if (authService.Register(regUser, regPass))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n[✔] Registration Successful! You can now login.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[✘] Error: Username already exists.");
                            Console.ResetColor();
                        }
                        Console.WriteLine("\nPress any key to return...");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Write("Username: ");
                        string logUser = Console.ReadLine() ?? "";
                        Console.Write("Password: ");
                        string logPass = Console.ReadLine() ?? "";

                        if (authService.Login(logUser, logPass))
                        {
                            // الدخول بنجاح ينقلك للوحة التحكم
                            ShowDashboard(authService);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[✘] Invalid Username or Password.");
                            Console.ResetColor();
                            Console.ReadKey();
                        }
                        break;

                    case "3":
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid selection. Try again.");
                        System.Threading.Thread.Sleep(1000);
                        break;
                }
            }
        }

        static void ShowDashboard(AuthenticationService authService)
        {
            bool inDashboard = true;
            while (inDashboard)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"--- User Dashboard | Welcome, {authService.CurrentUser?.Username} ---");
                Console.ResetColor();
                Console.WriteLine("1. Browse Movies (Member 2's Part)");
                Console.WriteLine("2. View Recommendations (Member 3's Part)");
                Console.WriteLine("3. Logout");
                Console.Write("\nSelect action: ");

                string dashChoice = Console.ReadLine() ?? "";

                switch (dashChoice)
                {
                    case "1":
                        Console.WriteLine("\n[Loading Movies...] (Wait for Member 2 to link MovieService)");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.WriteLine("\n[Generating Recommendations...] (Wait for Member 3 to link RecommendationService)");
                        Console.ReadKey();
                        break;

                    case "3":
                        authService.Logout(); // تصفير المستخدم الحالي
                        inDashboard = false; // العودة للقائمة الرئيسية
                        Console.WriteLine("Logging out...");
                        System.Threading.Thread.Sleep(1000);
                        break;

                    default:
                        Console.WriteLine("Invalid selection.");
                        System.Threading.Thread.Sleep(800);
                        break;
                }
            }
        }
    }
}
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           SELENIUM WEB MOVIE TEST SUITE                    ║");
            Console.WriteLine("║                   Nhóm 14 - BDCLPM                         ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  ADMIN TESTS:                                              ║");
            Console.WriteLine("║    1. Login Test                                           ║");
            Console.WriteLine("║    2. Admin - Quản lý Phim (PHIM_INT_01 → PHIM_INT_08)     ║");
            Console.WriteLine("║    3. Admin - Dashboard (DASH_INT_01 → DASH_INT_03)        ║");
            Console.WriteLine("║    4. Admin - Quản lý Bình luận (CMT_INT_01 → CMT_INT_07)  ║");
            Console.WriteLine("║    5. Admin - Run ALL Admin Tests                          ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  USER TESTS:                                               ║");
            Console.WriteLine("║    6. User - Tìm kiếm phim (TK_INT_01 → TK_INT_06)         ║");
            Console.WriteLine("║    7. User - Xem phim (XP_INT_01 → XP_INT_05)              ║");
            Console.WriteLine("║    8. User - Bình luận (BL_INT_01 → BL_INT_05)             ║");
            Console.WriteLine("║    9. User - Lịch sử xem + E2E (LS_INT_01 → E2E_INT_01)    ║");
            Console.WriteLine("║   10. User - Run ALL User Tests                            ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║   11. 🚀 RUN ALL TESTS (Admin + User)                      ║");
            Console.WriteLine("║   12. ❌ Negative Tests (Test thất bại)                    ║");
            Console.WriteLine("║    0. Thoát                                                ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.Write("\n👉 Chọn chức năng: ");

            string? choice = Console.ReadLine();

            if (choice == "0") break;

            // Khởi tạo ReportManager
            ReportManager.Init();

            // Chrome options
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--start-maximized");

            IWebDriver driver = new ChromeDriver(chromeOptions);

            try
            {
                switch (choice)
                {
                    case "1":
                        LoginTest.Run(driver);
                        break;

                    case "2":
                        LoginTest.Run(driver);
                        AdminManageMoviesTest.RunAllTests(driver);
                        break;

                    case "3":
                        LoginTest.Run(driver);
                        AdminDashboardTest.RunAllTests(driver);
                        break;

                    case "4":
                        LoginTest.Run(driver);
                        AdminManageCommentsTest.RunAllTests(driver);
                        break;

                    case "5":
                        Console.WriteLine("\n🚀 Running ALL Admin Tests...\n");
                        LoginTest.Run(driver);
                        AdminManageMoviesTest.RunAllTests(driver);
                        AdminDashboardTest.RunAllTests(driver);
                        AdminManageCommentsTest.RunAllTests(driver);
                        break;

                    case "6":
                        UserSearchTest.RunAllTests(driver);
                        break;

                    case "7":
                        LoginTest.Run(driver);
                        UserWatchMovieTest.RunAllTests(driver);
                        break;

                    case "8":
                        UserCommentTest.RunAllTests(driver);
                        break;

                    case "9":
                        UserWatchHistoryTest.RunAllTests(driver);
                        break;

                    case "10":
                        Console.WriteLine("\n🚀 Running ALL User Tests...\n");
                        LoginTest.Run(driver);
                        UserSearchTest.RunAllTests(driver);
                        UserWatchMovieTest.RunAllTests(driver);
                        UserCommentTest.RunAllTests(driver);
                        UserWatchHistoryTest.RunAllTests(driver);
                        break;

                    case "11":
                        Console.WriteLine("\n🚀 Running ALL TESTS (Admin + User)...\n");
                        // Admin tests
                        LoginTest.Run(driver);
                        AdminManageMoviesTest.RunAllTests(driver);
                        AdminDashboardTest.RunAllTests(driver);
                        AdminManageCommentsTest.RunAllTests(driver);
                        // User tests
                        UserSearchTest.RunAllTests(driver);
                        UserWatchMovieTest.RunAllTests(driver);
                        UserCommentTest.RunAllTests(driver);
                        UserWatchHistoryTest.RunAllTests(driver);
                        break;

                    case "12":
                        Console.WriteLine("\n❌ Running NEGATIVE TESTS (Test thất bại)...\n");
                        NegativeTestCases.RunAllTests(driver);
                        break;

                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ LỖI: " + ex.Message);
                Console.WriteLine("📋 Stack trace: " + ex.StackTrace);
            }

            // Generate report
            ReportManager.Flush();
            Console.WriteLine("\n📊 Report đã được tạo tại: Reports/report.html");

            Console.WriteLine("\n👉 Nhấn Enter để quay lại menu...");
            Console.ReadLine();

            driver.Quit();
        }
    }
}
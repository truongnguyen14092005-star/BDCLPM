using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        while (true)
        {
            try
            {
                if (!Console.IsInputRedirected && !Console.IsOutputRedirected && !Console.IsErrorRedirected)
                {
                    Console.Clear();
                }
            }
            catch
            {
                // Ignore Console.Clear errors in non-interactive hosts
            }
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           SELENIUM WEB MOVIE TEST SUITE                    ║");
            Console.WriteLine("║                   Nhóm 2 - BDCLPM                          ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  ADMIN TESTS:                                              ║");
            Console.WriteLine("║    1. Login Test                                           ║");
            Console.WriteLine("║    2. Admin - Quản lý Phim                                 ║");
            Console.WriteLine("║    3. Admin - Dashboard                                    ║");
            Console.WriteLine("║    4. Admin - Quản lý Bình luận                            ║");
            Console.WriteLine("║    5. Admin - Run ALL Admin Tests                          ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  USER TESTS:                                               ║");
            Console.WriteLine("║    6. User - Tìm kiếm phim                                 ║");
            Console.WriteLine("║    7. User - Xem phim                                      ║");
            Console.WriteLine("║    8. User - Bình luận                                     ║");
            Console.WriteLine("║    9. User - Lịch sử xem                                   ║");
            Console.WriteLine("║   10. User - Run ALL User Tests                            ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║   11. 🚀 RUN ALL TESTS (Admin + User)                      ║");
            Console.WriteLine("║   12. ❌ Negative Tests                                    ║");
            Console.WriteLine("║   13. 📊 Data-Driven Testing (Excel)                      ║");
            Console.WriteLine("║    0. Thoát                                                ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.Write("\n👉 Chọn chức năng: ");

            string? choice = Console.ReadLine();
            if (choice == "0") break;

            IWebDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

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
                        // ✅ Kết quả đã được ghi vào Excel tự động
                        break;

                    case "3":
                        LoginTest.Run(driver);
                        AdminDashboardTest.RunAllTests(driver);
                        // ✅ Kết quả đã được ghi vào Excel tự động
                        break;

                    case "4":
                        LoginTest.Run(driver);
                        AdminManageCommentsTest.RunAllTests(driver);
                        // TODO: Thêm SaveResultsToExcel khi AdminManageCommentsTest có LastRunResults
                        break;

                    case "5":
                        LoginTest.Run(driver);
                        AdminManageMoviesTest.RunAllTests(driver);
                        AdminDashboardTest.RunAllTests(driver);
                        AdminManageCommentsTest.RunAllTests(driver);
                        break;

                    case "6":
                        UserSearchTest.RunAllTests(driver);
                        // ✅ Kết quả đã được ghi vào Excel tự động
                        break;

                    case "7":
                        LoginTest.Run(driver);
                        UserWatchMovieTest.RunAllTests(driver);
                        // ✅ Kết quả đã được ghi vào Excel tự động
                        break;

                    case "8":
                        UserCommentTest.RunAllTests(driver);
                        // ✅ Kết quả đã được ghi vào Excel tự động
                        break;

                    case "9":
                        UserWatchHistoryTest.RunAllTests(driver);
                        // ✅ Kết quả đã được ghi vào Excel tự động
                        break;

                    case "10":
                        LoginTest.Run(driver);

                        try
                        {
                            UserSearchTest.RunAllTests(driver);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ UserSearchTest LỖI: {ex.Message}");
                        }

                        try
                        {
                            UserWatchMovieTest.RunAllTests(driver);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ UserWatchMovieTest LỖI: {ex.Message}");
                        }

                        try
                        {
                            UserCommentTest.RunAllTests(driver);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ UserCommentTest LỖI: {ex.Message}");
                        }

                        try
                        {
                            UserWatchHistoryTest.RunAllTests(driver);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ UserWatchHistoryTest LỖI: {ex.Message}");
                        }

                        break;

                    case "11":
                        LoginTest.Run(driver);

                        try { AdminManageMoviesTest.RunAllTests(driver); }
                        catch (Exception ex) { Console.WriteLine($"❌ AdminManageMoviesTest LỖI: {ex.Message}"); }

                        try { AdminDashboardTest.RunAllTests(driver); }
                        catch (Exception ex) { Console.WriteLine($"❌ AdminDashboardTest LỖI: {ex.Message}"); }

                        try { AdminManageCommentsTest.RunAllTests(driver); }
                        catch (Exception ex) { Console.WriteLine($"❌ AdminManageCommentsTest LỖI: {ex.Message}"); }

                        try { UserSearchTest.RunAllTests(driver); }
                        catch (Exception ex) { Console.WriteLine($"❌ UserSearchTest LỖI: {ex.Message}"); }

                        try { UserWatchMovieTest.RunAllTests(driver); }
                        catch (Exception ex) { Console.WriteLine($"❌ UserWatchMovieTest LỖI: {ex.Message}"); }

                        try { UserCommentTest.RunAllTests(driver); }
                        catch (Exception ex) { Console.WriteLine($"❌ UserCommentTest LỖI: {ex.Message}"); }

                        try { UserWatchHistoryTest.RunAllTests(driver); }
                        catch (Exception ex) { Console.WriteLine($"❌ UserWatchHistoryTest LỖI: {ex.Message}"); }

                        break;

                    case "12":
                        NegativeTestCases.RunAllTests(driver);
                        break;

                    // 🔥 CASE QUAN TRỌNG NHẤT (ĐÃ SỬA CHUẨN)
                    case "13":
                        {
                            Console.WriteLine("\n📊 Running Auto Test Suite (Data-Driven from Excel)...\n");

                            string excelPath = @"C:\Users\truongnguyen\Downloads\IntegrationTestCase_Nhom2_WebMovie.xlsx";

                            if (!File.Exists(excelPath))
                            {
                                Console.WriteLine("❌ Không tìm thấy file Excel: " + excelPath);
                                break;
                            }

                            try
                            {
                                // 👉 Auto Test đọc cột 3 (Test Case ID), thực thi các step, ghi kết quả vào cột 11
                                var autoRunner = new AutoTestRunner(driver, excelPath);
                                autoRunner.RunAllTests();

                                Console.WriteLine("\n🎉 Hoàn thành tất cả test cases!");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ Lỗi: {ex.Message}");
                            }
                            break;
                        }

                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ LỖI: " + ex.Message);
            }

            Console.WriteLine("\n👉 Nhấn Enter để tiếp tục...");
            Console.ReadLine();
            driver.Quit();
        }
    }

    // Helper method: Ghi kết quả test vào Excel
    // sheetName: Tên sheet trong Excel
    // results: Dictionary<string, string> chứa {testCaseId: "Passed"/"Failed"}
    static void SaveResultsToExcel(string sheetName, Dictionary<string, string> results)
    {
        if (results == null || results.Count == 0)
        {
            return;
        }

        foreach (var kvp in results)
        {
            ExcelHelper.SaveTestResultToExcel(sheetName, kvp.Key, kvp.Value);
        }
    }
}
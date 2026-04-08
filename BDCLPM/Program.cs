using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.Linq;

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
            Console.WriteLine("╠═══════════════════════════════════════════════════════════ ╣");
            Console.WriteLine("║  USER TESTS:                                               ║");
            Console.WriteLine("║    6. User - Tìm kiếm phim (TK_INT_01 → TK_INT_06)         ║");
            Console.WriteLine("║    7. User - Xem phim (XP_INT_01 → XP_INT_05)              ║");
            Console.WriteLine("║    8. User - Bình luận (BL_INT_01 → BL_INT_05)             ║");
            Console.WriteLine("║    9. User - Lịch sử xem + E2E (LS_INT_01 → E2E_INT_01)    ║");
            Console.WriteLine("║   10. User - Run ALL User Tests                            ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║   11. 🚀 RUN ALL TESTS (Admin + User)                      ║");
            Console.WriteLine("║   12. ❌ Negative Tests (Test thất bại)                    ║");
            Console.WriteLine("║   13. 📊 Data-Driven Testing (Excel)                      ║");
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
                        UpdateIntegrationExcelResults();
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
                        UpdateIntegrationExcelResults();
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
                        UpdateIntegrationExcelResults();
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

                    case "13":
                        Console.WriteLine("\n📊 Running Data-Driven Testing from Excel...\n");
                        LoginTest.Run(driver);
                        AdminManageMoviesTest.Initialize(driver);
                        AdminDashboardTest.Initialize(driver);

                        var excelPath = ExcelHelper.GetIntegrationExcelPath();
                        if (!File.Exists(excelPath))
                        {
                            Console.WriteLine("⚠️ Không tìm thấy file Excel: " + excelPath);
                            break;
                        }

                        ExcelPackage.License.SetNonCommercialPersonal("Truong Nguyen");
                        using (var package = new ExcelPackage(new FileInfo(excelPath)))
                        {
                            var worksheet = package.Workbook.Worksheets["Integrated TC QL Phim"]
                                ?? package.Workbook.Worksheets["Integrated TC Dashboard"]
                                ?? package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet == null)
                            {
                                Console.WriteLine("Không tìm thấy worksheet để chạy Excel tests.");
                                break;
                            }

                            int rowCount = worksheet.Dimension?.Rows ?? 0;
                            if (rowCount < 3)
                            {
                                Console.WriteLine("Excel sheet không có dữ liệu test.");
                                break;
                            }

                            for (int row = 3; row <= rowCount; row++)
                            {
                                string testId = worksheet.Cells[row, 3].Text.Trim();
                                if (string.IsNullOrEmpty(testId)) continue;
                                Console.WriteLine($"Running test: {testId}");
                                try
                                {
                                    bool passed;
                                    switch (testId)
                                    {
                                        case "PHIM_INT_01":
                                            passed = AdminManageMoviesTest.Test_PHIM_INT_01_SearchAndPagination(driver);
                                            break;
                                        case "PHIM_INT_02":
                                            passed = AdminManageMoviesTest.Test_PHIM_INT_02_FullPageSearch(driver);
                                            break;
                                        case "PHIM_INT_03":
                                            passed = AdminManageMoviesTest.Test_PHIM_INT_03_HideShowMovie(driver);
                                            break;
                                        case "PHIM_INT_04":
                                            passed = AdminManageMoviesTest.Test_PHIM_INT_04_AddNewMovie(driver);
                                            break;
                                        case "PHIM_INT_05":
                                            passed = AdminManageMoviesTest.Test_PHIM_INT_05_CustomTitle(driver);
                                            break;
                                        case "DASH_INT_01":
                                            passed = AdminDashboardTest.Test_DASH_INT_01_DashboardStatistics(driver);
                                            break;
                                        case "DASH_INT_02":
                                            passed = AdminDashboardTest.Test_DASH_INT_02_DashboardSync(driver);
                                            break;
                                        case "DASH_INT_03":
                                            passed = AdminDashboardTest.Test_DASH_INT_03_DashboardAfterDeleteComments(driver);
                                            break;
                                        default:
                                            Console.WriteLine($"Test {testId} not implemented.");
                                            worksheet.Cells[row, 11].Value = "Not Implemented";
                                            worksheet.Cells[row, 11].Style.Font.Color.SetColor(Color.Orange);
                                            continue;
                                    }

                                    worksheet.Cells[row, 11].Value = passed ? "Passed" : "Failed";
                                    worksheet.Cells[row, 11].Style.Font.Color.SetColor(passed ? Color.Green : Color.Red);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Test {testId} failed: {ex.Message}");
                                    worksheet.Cells[row, 11].Value = "Failed";
                                    worksheet.Cells[row, 11].Style.Font.Color.SetColor(Color.Red);
                                }
                            }

                            package.Save();
                            Console.WriteLine("Excel file updated and saved: " + excelPath);
                        }
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

    private static void UpdateIntegrationExcelResults()
    {
        var excelPath = ExcelHelper.GetIntegrationExcelPath();
        if (!File.Exists(excelPath))
        {
            Console.WriteLine("⚠️ Không tìm thấy file Excel để cập nhật: " + excelPath);
            return;
        }

        if (AdminManageMoviesTest.LastRunResults.Count == 0 && AdminDashboardTest.LastRunResults.Count == 0)
        {
            Console.WriteLine("⚠️ Chưa có kết quả test để ghi vào Excel.");
            return;
        }

        try
        {
            var combinedResults = new Dictionary<string, string>(AdminManageMoviesTest.LastRunResults);
            foreach (var kv in AdminDashboardTest.LastRunResults)
            {
                combinedResults[kv.Key] = kv.Value;
            }

            var saved = ExcelHelper.SaveIntegrationResults(excelPath, combinedResults);
            if (!saved)
            {
                Console.WriteLine("⚠️ Không thể cập nhật Excel. Kiểm tra file đang mở hoặc quyền truy cập.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi ghi Excel: {ex.Message}");
        }
    }
}
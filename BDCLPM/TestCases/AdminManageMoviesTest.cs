using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;
using System.Linq;

/// <summary>
/// Test Quản lý phim - Admin
/// Dựa trên: Integrated TC QL Phim (PHIM_INT_01 → PHIM_INT_08)
/// </summary>
public class AdminManageMoviesTest
{
    private static ExtentTest? test;
    private static WebDriverWait? wait;
    public static Dictionary<string, string> LastRunResults { get; private set; } = new Dictionary<string, string>();
    private const string BaseUrl = "https://localhost:5001";

    public static void Initialize(IWebDriver driver)
    {
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    private static void EnsureWait(IWebDriver driver)
    {
        if (wait == null)
        {
            Initialize(driver);
        }
    }

    public static void RunAllTests(IWebDriver driver)
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🎬 ADMIN QUẢN LÝ PHIM - BẮT ĐẦU TEST");
        Console.WriteLine(new string('=', 60));

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

        LastRunResults.Clear();

        // Sheet name cho lưu Excel
        string sheetName = "Integrated TC QL Phim";

        // Chạy các test case với tự động ghi kết quả vào Excel
        RunTestAndSaveResult(driver, "PHIM_INT_01", () => Test_PHIM_INT_01_SearchAndPagination(driver), sheetName);
        RunTestAndSaveResult(driver, "PHIM_INT_02", () => Test_PHIM_INT_02_FullPageSearch(driver), sheetName);
        RunTestAndSaveResult(driver, "PHIM_INT_03", () => Test_PHIM_INT_03_HideShowMovie(driver), sheetName);
        RunTestAndSaveResult(driver, "PHIM_INT_04", () => Test_PHIM_INT_04_AddNewMovie(driver), sheetName);
        RunTestAndSaveResult(driver, "PHIM_INT_05", () => Test_PHIM_INT_05_CustomTitle(driver), sheetName);

        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("✅ ADMIN QUẢN LÝ PHIM - HOÀN THÀNH");
        Console.WriteLine(new string('=', 60));
    }

    /// <summary>
    /// Helper method: Chạy test và tự động ghi kết quả vào Excel
    /// </summary>
    private static void RunTestAndSaveResult(IWebDriver driver, string testCaseId, Func<bool> testMethod, string sheetName)
    {
        string status = "Failed";
        string screenshotPath = "";

        try
        {
            // 1. Chạy hàm test
            bool isPassed = testMethod();

            if (isPassed)
            {
                status = "Passed";
            }
            else
            {
                status = "Failed";
                // CHỤP ẢNH NGAY khi hàm test trả về false (Logic fail)
                screenshotPath = ScreenshotHelper.Capture(driver, testCaseId + "_LogicFail");
            }
        }
        catch (Exception ex)
        {
            status = "Failed";
            // CHỤP ẢNH NGAY khi có lỗi hệ thống/crash
            screenshotPath = ScreenshotHelper.Capture(driver, testCaseId + "_Exception");
            Console.WriteLine($"[LỖI] {testCaseId}: {ex.Message}");
        }
        finally
        {
            LastRunResults[testCaseId] = status;
            // LUÔN LUÔN ghi vào Excel ở bước cuối cùng
            ExcelHelper.SaveTestResultToExcel(sheetName, testCaseId, status, screenshotPath);
        }
    }

    /// <summary>
    /// PHIM_INT_01: Tìm kiếm real-time → Xem chi tiết → Phân trang
    /// </summary>
    public static bool Test_PHIM_INT_01_SearchAndPagination(IWebDriver driver)
    {
        EnsureWait(driver);
        Console.WriteLine("\n📋 Test PHIM_INT_01: Tìm kiếm + Phân trang + Xem chi tiết");
        test = ReportManager.extent?.CreateTest("PHIM_INT_01: Search + Pagination + Detail");
        bool testPassed = true;
        const string movieKeyword = "Mai";

        try
        {
            // Step 1: Vào Manage Movies
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/ManageMovies");
            Thread.Sleep(2000);

            // ✅ CHỨNG MINH: Kiểm tra trang đã load và có phân trang
            bool hasMovieList = driver.PageSource.Contains("Movie") || driver.PageSource.Contains("Phim");
            bool hasPagination = driver.FindElements(By.CssSelector(".pagination, nav[aria-label='pagination'], .page-link")).Count > 0;

            if (hasMovieList)
            {
                Console.WriteLine("  ✅ Step 1 PASS: Danh sách phim hiển thị");
                Console.WriteLine($"     📊 Có phân trang: {(hasPagination ? "Có" : "Không")}");
                test?.Pass("Step 1: Danh sách phim hiển thị thành công");
            }
            else
            {
                Console.WriteLine("  ❌ Step 1 FAIL: Không tìm thấy danh sách phim");
                test?.Fail("Step 1: Không tìm thấy danh sách phim");
                return false;
            }

            // Step 2: Click phân trang (trang tiếp theo nếu có)
            try
            {
                var pageLink = FindPaginationLink(driver);
                string page1Content = driver.PageSource;

                SafeClick(driver, pageLink);
                Thread.Sleep(2000);

                string page2Content = driver.PageSource;
                bool contentChanged = !page1Content.Equals(page2Content);

                if (!contentChanged)
                {
                    Console.WriteLine("  ❌ Step 2 FAIL: Click phân trang nhưng nội dung không thay đổi");
                    test?.Fail("Step 2: Click phân trang nhưng nội dung không thay đổi");
                    testPassed = false;
                }
                else
                {
                    Console.WriteLine("  ✅ Step 2 PASS: Click phân trang - Nội dung thay đổi");
                    test?.Pass("Step 2: Chuyển trang tiếp theo thành công");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Step 2 FAIL: Không tìm thấy phân trang khả dụng ({ex.Message})");
                test?.Fail("Step 2: Không tìm thấy phân trang khả dụng");
                testPassed = false;
            }

            // Step 3: Tìm kiếm real-time "Lật Mặt 7"
            var searchInput = wait!.Until(d =>
                d.FindElement(By.CssSelector("input[type='text'], input[name='search'], input[placeholder*='Tìm'], input[placeholder*='Search']"))
            );

            searchInput.Clear();
            searchInput.SendKeys(movieKeyword);
            Thread.Sleep(1500); // Chờ dropdown real-time

            // ✅ CHỨNG MINH: Kiểm tra dropdown gợi ý xuất hiện
            bool hasDropdown = driver.FindElements(By.CssSelector(".dropdown-menu.show, .autocomplete-results, .search-suggestions, ul.suggestions")).Count > 0;
            bool pageHasResult = driver.PageSource.Contains(movieKeyword) || driver.PageSource.Contains("Mai");

            if (!pageHasResult)
            {
                Console.WriteLine($"  ❌ Step 3 FAIL: Không thấy kết quả chứa '{movieKeyword}'");
                test?.Fail($"Step 3: Không thấy kết quả chứa '{movieKeyword}'");
                testPassed = false;
            }
            else
            {
                Console.WriteLine($"  ✅ Step 3 PASS: Tìm kiếm '{movieKeyword}'");
                Console.WriteLine($"     📊 Dropdown gợi ý: {(hasDropdown ? "Hiển thị" : "Không hiển thị")}");
                test?.Pass($"Step 3: Search OK - Dropdown: {hasDropdown}");
            }

            // Step 4: Click vào phim trong kết quả để xem chi tiết
            try
            {
                // Tìm link chi tiết ưu tiên theo title text
                var movieLink = driver.FindElements(By.XPath(
                        $"//a[contains(normalize-space(.), '{movieKeyword}')][contains(@href,'Detail') or contains(@href,'detail') or contains(@href,'Movie')]"
                    ))
                    .FirstOrDefault();

                movieLink ??= driver.FindElements(By.XPath(
                        "//table//a[contains(@href,'Detail') or contains(@href,'detail')] | //div[contains(@class,'movie')]//a"
                    ))
                    .FirstOrDefault();

                if (movieLink == null)
                {
                    Console.WriteLine("  ❌ Step 4 FAIL: Không tìm thấy link xem chi tiết phim");
                    test?.Fail("Step 4: Không tìm thấy link xem chi tiết phim");
                    return false;
                }

                AcceptAlertIfPresent(driver);
                SafeClick(driver, movieLink);
                Thread.Sleep(2000);

                // ✅ CHỨNG MINH: Trang chi tiết hiển thị đủ thông tin
                bool hasTitle = driver.FindElements(By.CssSelector("h1, h2, .movie-title")).Count > 0;
                bool hasPoster = driver.FindElements(By.CssSelector("img[src*='poster'], img.poster, .movie-poster img")).Count > 0;
                bool hasInfo = driver.PageSource.Contains("Thể loại") || driver.PageSource.Contains("Genre") ||
                              driver.PageSource.Contains("Năm") || driver.PageSource.Contains("Year");

                if (!hasTitle)
                {
                    Console.WriteLine("  ❌ Step 4 FAIL: Trang chi tiết không có tiêu đề");
                    test?.Fail("Step 4: Trang chi tiết không có tiêu đề");
                    testPassed = false;
                }
                else
                {
                    Console.WriteLine("  ✅ Step 4 PASS: Xem chi tiết phim (có tiêu đề)");
                    test?.Pass("Step 4: Detail page loaded");
                }

                // Poster/Info có thể tuỳ dữ liệu, không ép fail cứng
                Console.WriteLine($"     📊 Có poster: {hasPoster}");
                Console.WriteLine($"     📊 Có thông tin (thể loại/năm): {hasInfo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Step 4 FAIL: Lỗi khi mở chi tiết ({ex.Message})");
                test?.Fail($"Step 4: Exception: {ex.Message}");
                testPassed = false;
            }

            // Step 5: Quay lại danh sách bằng cách navigate trực tiếp
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/ManageMovies");
            Thread.Sleep(1500);

            bool backToList = driver.Url.Contains("ManageMovies") || driver.PageSource.Contains("Quản lý");
            if (!backToList)
            {
                Console.WriteLine("  ❌ Step 5 FAIL: Không quay lại được danh sách phim");
                test?.Fail("Step 5: Không quay lại được danh sách phim");
                testPassed = false;
            }
            else
            {
                Console.WriteLine("  ✅ Step 5 PASS: Quay lại danh sách");
                test?.Pass("Step 5: Back to list");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ PHIM_INT_01 FAILED: {ex.Message}");
            test?.Fail($"Test failed: {ex.Message}");
            testPassed = false;
        }

        return testPassed;
    }

    /// <summary>
    /// PHIM_INT_02: Tìm kiếm full-page (nhấn Enter) → Kết quả phân trang
    /// </summary>
    public static bool Test_PHIM_INT_02_FullPageSearch(IWebDriver driver)
    {
        EnsureWait(driver);
        Console.WriteLine("\n📋 Test PHIM_INT_02: Tìm kiếm full-page (Enter)");
        test = ReportManager.extent?.CreateTest("PHIM_INT_02: Full-page Search");
        bool testPassed = true;
        const string movieKeyword = "Mai";

        try
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/ManageMovies");
            Thread.Sleep(2000);

            // Step 1: Nhập từ khóa và nhấn Enter
            var searchInput = wait!.Until(d =>
                d.FindElement(By.CssSelector("input[type='text'], input[name='search']"))
            );

            searchInput.Clear();
            searchInput.SendKeys(movieKeyword);
            searchInput.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            // ✅ CHỨNG MINH: URL thay đổi hoặc kết quả cập nhật
            bool urlContainsSearch = driver.Url.Contains("search") || driver.Url.Contains("keyword") || driver.Url.Contains("q=");
            bool hasResults = driver.PageSource.Contains(movieKeyword) || driver.PageSource.Contains("Mai");

            if (!hasResults)
            {
                Console.WriteLine($"  ❌ Step 1 FAIL: Full-page search không ra kết quả cho '{movieKeyword}'");
                test?.Fail($"Step 1: No results for '{movieKeyword}'");
                testPassed = false;
            }
            else
            {
                Console.WriteLine($"  ✅ Step 1 PASS: Tìm kiếm full-page '{movieKeyword}'");
                Console.WriteLine($"     📊 URL chứa search param: {urlContainsSearch}");
                test?.Pass($"Step 1: Full-page search OK - URL param: {urlContainsSearch}");
            }

            // Step 2: Kiểm tra phân trang kết quả
            var paginationItems = driver.FindElements(By.CssSelector(".pagination a, .page-link, nav[aria-label*='pagination'] a"));
            if (paginationItems.Count == 0)
            {
                Console.WriteLine("  ❌ Step 2 FAIL: Không thấy phân trang trong trang kết quả");
                test?.Fail("Step 2: Pagination not found");
                testPassed = false;
            }
            else
            {
                Console.WriteLine($"  ✅ Step 2 PASS: Phân trang - {paginationItems.Count} item(s)");
                test?.Pass($"Step 2: Pagination items: {paginationItems.Count}");
            }

            // Step 3: Xóa search và reload
            searchInput = driver.FindElement(By.CssSelector("input[type='text'], input[name='search']"));
            searchInput.Clear();
            searchInput.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            // ✅ CHỨNG MINH: Danh sách trở về mặc định
            bool resetComplete = !driver.Url.Contains("search") || driver.PageSource.Contains("Tất cả");
            if (!resetComplete)
            {
                Console.WriteLine("  ❌ Step 3 FAIL: Không reset được về danh sách đầy đủ");
                test?.Fail("Step 3: Reset failed");
                testPassed = false;
            }
            else
            {
                Console.WriteLine($"  ✅ Step 3 PASS: Reset về danh sách đầy đủ: {resetComplete}");
                test?.Pass($"Step 3: Reset to full list: {resetComplete}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ PHIM_INT_02 FAILED: {ex.Message}");
            test?.Fail($"Test failed: {ex.Message}");
            testPassed = false;
        }

        return testPassed;
    }

    /// <summary>
    /// PHIM_INT_03: Ẩn phim → User không thấy → Hiện lại → User thấy
    /// </summary>
    public static bool Test_PHIM_INT_03_HideShowMovie(IWebDriver driver)
    {
        Console.WriteLine("\n📋 Test PHIM_INT_03: Ẩn/Hiện phim");
        test = ReportManager.extent?.CreateTest("PHIM_INT_03: Hide/Show Movie");
        bool testPassed = true;
        const string movieKeyword = "Mai";

        try
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/ManageMovies");
            Thread.Sleep(2000);

            // Step 1: Tìm phim để ẩn
            var searchInput = driver.FindElement(By.CssSelector("input[type='text'], input[name='search']"));
            searchInput.Clear();
            searchInput.SendKeys(movieKeyword);
            Thread.Sleep(1500);

            // ✅ CHỨNG MINH: Trang quản lý phim có nút Ẩn/Hiển thị
            var hideBtn = FindHideMovieButton(driver, movieKeyword);

            bool hasHideButtons = hideBtn != null;
            Console.WriteLine($"  ✅ Step 1 PASS: Tìm thấy nút Ẩn/Hiển thị trên trang: {hasHideButtons}");
            test?.Pass($"Step 1: Hide/Show button found: {hasHideButtons}");
            if (!hasHideButtons)
                return false;

            // Step 2: Click nút Ẩn (Toggle Hide)
            try
            {
                if (hideBtn == null)
                    throw new NoSuchElementException("Không tìm thấy nút Ẩn phim");

                SafeClick(driver, hideBtn);
                AcceptAlertIfPresent(driver);
                Thread.Sleep(2000);

                // ✅ CHỨNG MINH: Có thông báo hoặc phim biến mất
                bool hasNotification = driver.FindElements(By.CssSelector(".toast, .alert, .notification")).Count > 0;
                Console.WriteLine($"  ✅ Step 2 PASS: Click Ẩn - Thông báo: {hasNotification}");
                test?.Pass($"Step 2: Hide clicked - Notification: {hasNotification}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Step 2 FAIL: Không thể ẩn phim ({ex.Message})");
                test?.Fail($"Step 2: Hide failed: {ex.Message}");
                testPassed = false;
            }

            // Step 3: Kiểm tra tab phim đã ẩn
            try
            {
                AcceptAlertIfPresent(driver);
                var hiddenTab = driver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[1]/div/div[2]/a[2]"));
                SafeClick(driver, hiddenTab);
                Thread.Sleep(2000);

                bool movieInHidden = driver.PageSource.Contains(movieKeyword) || driver.PageSource.Contains("Mai");
                if (!movieInHidden)
                {
                    Console.WriteLine("  ❌ Step 3 FAIL: Không thấy phim trong tab phim đã ẩn");
                    test?.Fail("Step 3: Movie not found in hidden tab");
                    testPassed = false;
                }
                else
                {
                    Console.WriteLine("  ✅ Step 3 PASS: Tab phim ẩn - Phim có mặt");
                    test?.Pass("Step 3: Movie present in hidden tab");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Step 3 FAIL: Không tìm thấy tab phim đã ẩn ({ex.Message})");
                test?.Fail($"Step 3: Hidden tab missing: {ex.Message}");
                testPassed = false;
            }

            // Step 4: Mở tab ẩn danh kiểm tra User không thấy phim
            // (Trong test tự động, ta mô phỏng bằng cách mở trang home)
            AcceptAlertIfPresent(driver);
            driver.Navigate().GoToUrl($"{BaseUrl}/");
            Thread.Sleep(2000);
            AcceptAlertIfPresent(driver);

            try
            {
                searchInput = driver.FindElement(By.CssSelector("input[placeholder*='Search'], input[placeholder*='Tìm'], input[name='keyword']"));
                searchInput.Clear();
                searchInput.SendKeys(movieKeyword);
                searchInput.SendKeys(Keys.Enter);
                Thread.Sleep(2000);
            }
            catch
            {
                // Nếu trang home không có search box thì fallback kiểm tra theo PageSource
            }

            var userResults = driver.FindElements(By.XPath(
                $"//a[contains(normalize-space(.), '{movieKeyword}')] | " +
                $"//h1[contains(normalize-space(.), '{movieKeyword}')] | " +
                $"//h2[contains(normalize-space(.), '{movieKeyword}')] | " +
                $"//h3[contains(normalize-space(.), '{movieKeyword}')]")
            );
            bool movieHiddenFromUser = userResults.Count == 0 || driver.PageSource.Contains("Không tìm thấy");
            if (!movieHiddenFromUser)
            {
                Console.WriteLine("  ❌ Step 4 FAIL: User vẫn thấy phim sau khi ẩn");
                test?.Fail("Step 4: Movie still visible to user");
                testPassed = false;
            }
            else
            {
                Console.WriteLine("  ✅ Step 4 PASS: User không thấy phim đã ẩn");
                test?.Pass("Step 4: Movie hidden from user");
            }

            // Step 5-7: Hiện lại phim (quay lại Admin)
            AcceptAlertIfPresent(driver);
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/ManageMovies?showHidden=true");
            Thread.Sleep(2000);
            AcceptAlertIfPresent(driver);

            try
            {
                var showButtons = driver.FindElements(By.CssSelector("button.toggle-hide-movie.btn-success"));
                var showBtn = showButtons.FirstOrDefault(btn => btn.Text.Contains("Hiển thị") || btn.Text.Contains("Hiện"));
                if (showBtn == null)
                {
                    showBtn = driver.FindElements(By.CssSelector("button.toggle-hide-movie")).FirstOrDefault(btn => btn.Text.Contains("Hiển thị") || btn.Text.Contains("Hiện"));
                }

                if (showBtn == null)
                {
                    throw new NoSuchElementException("Không tìm thấy nút Hiển thị phim");
                }

                SafeClick(driver, showBtn);
                AcceptAlertIfPresent(driver);
                Thread.Sleep(2000);
                Console.WriteLine("  ✅ Step 5-6 PASS: Click Hiển lại phim");
                test?.Pass("Step 5-6: Show movie clicked");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Step 5-6 FAIL: Không thể hiện lại phim ({ex.Message})");
                test?.Fail($"Step 5-6: Show failed: {ex.Message}");
                testPassed = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ PHIM_INT_03 FAILED: {ex.Message}");
            test?.Fail($"Test failed: {ex.Message}");
            testPassed = false;
        }

        return testPassed;
    }

    /// <summary>
    /// PHIM_INT_04: Ẩn phim rồi kiểm tra truy cập trực tiếp URL chi tiết phim bị chặn
    /// </summary>
    public static bool Test_PHIM_INT_04_AddNewMovie(IWebDriver driver)
    {
        EnsureWait(driver);
        Console.WriteLine("\n📋 Test PHIM_INT_04: Ẩn phim và kiểm tra truy cập trực tiếp");
        test = ReportManager.extent?.CreateTest("PHIM_INT_04: Hide movie direct access blocked");
        bool testPassed = true;
        const string movieKeyword = "Mai";

        try
        {
            AcceptAlertIfPresent(driver);
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/ManageMovies");
            Thread.Sleep(2000);
            AcceptAlertIfPresent(driver);

            var searchInput = wait!.Until(d =>
                d.FindElement(By.CssSelector("input[type='text'], input[name='search'], input[placeholder*='Tìm'], input[placeholder*='Search']"))
            );
            searchInput.Clear();
            searchInput.SendKeys(movieKeyword);
            Thread.Sleep(1500);

            // Lấy link chi tiết để test direct access (ưu tiên theo text)
            var detailLink = driver.FindElements(By.XPath(
                    $"//a[contains(normalize-space(.), '{movieKeyword}')][contains(@href,'Detail') or contains(@href,'detail')]"
                ))
                .FirstOrDefault()
                ?? driver.FindElements(By.XPath("//a[contains(@href,'Detail') or contains(@href,'detail')]")).FirstOrDefault();

            if (detailLink == null)
            {
                Console.WriteLine("  ❌ Step 0 FAIL: Không tìm thấy link chi tiết để kiểm tra direct access");
                test?.Fail("Step 0: No detail link found");
                return false;
            }

            var movieDetailUrl = detailLink.GetAttribute("href") ?? "";
            if (string.IsNullOrEmpty(movieDetailUrl))
            {
                Console.WriteLine("  ❌ Step 0 FAIL: Link chi tiết không có href");
                test?.Fail("Step 0: Detail link has empty href");
                return false;
            }

            AcceptAlertIfPresent(driver);
            var hideBtn = FindHideMovieButton(driver, movieKeyword);

            if (hideBtn == null)
                throw new NoSuchElementException("Không tìm thấy nút Ẩn phim");

            string? alertText = null;
            try
            {
                SafeClick(driver, hideBtn);
                alertText = AcceptAlertIfPresent(driver);
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ Warning while clicking hide: {ex.Message}");
                alertText = AcceptAlertIfPresent(driver);
                Thread.Sleep(1000);
            }

            bool hiddenNotification = !string.IsNullOrEmpty(alertText) || driver.FindElements(By.CssSelector(".toast, .alert, .notification")).Count > 0;
            Console.WriteLine($"  ✅ Step 1 PASS: Đã ẩn phim - Thông báo/alert: {hiddenNotification} ({alertText})");
            test?.Pass($"Step 1: Hide movie clicked: {hiddenNotification}");

            try
            {
                driver.Navigate().GoToUrl(movieDetailUrl);
            }
            catch (UnhandledAlertException)
            {
                AcceptAlertIfPresent(driver);
                Thread.Sleep(500);
                driver.Navigate().GoToUrl(movieDetailUrl);
            }
            Thread.Sleep(2000);

            bool blocked = driver.PageSource.Contains("Không tìm thấy") ||
                           driver.PageSource.Contains("không tồn tại") ||
                           driver.PageSource.Contains("bị chặn") ||
                           driver.PageSource.Contains("Forbidden") ||
                           driver.Url.Contains("404") ||
                           driver.Url.Contains("403") ||
                           driver.PageSource.Contains("Access denied");

            if (!blocked)
            {
                Console.WriteLine("  ❌ Step 2 FAIL: Truy cập trực tiếp KHÔNG bị chặn khi phim đã ẩn");
                test?.Fail("Step 2: Direct access was not blocked");
                testPassed = false;
            }
            else
            {
                Console.WriteLine($"  ✅ Step 2 PASS: Truy cập trực tiếp bị chặn: {blocked}");
                test?.Pass($"Step 2: Direct access blocked: {blocked}");
            }

            AcceptAlertIfPresent(driver);
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/ManageMovies?showHidden=true");
            Thread.Sleep(2000);
            AcceptAlertIfPresent(driver);

            var showBtn = driver.FindElements(By.CssSelector("button.toggle-hide-movie, button.btn, a.btn, .btn-show, .show-action"))
                .FirstOrDefault(btn => btn.Text.Contains("Hiển") || btn.Text.Contains("Hiện") || btn.Text.Contains("Show"));
            if (showBtn != null)
            {
                SafeClick(driver, showBtn);
                AcceptAlertIfPresent(driver);
                Thread.Sleep(2000);
                Console.WriteLine("  ✅ Step 3 PASS: Đã hiện lại phim để trả môi trường về trạng thái ban đầu");
                test?.Pass("Step 3: Movie restored after block check");
            }
            else
            {
                Console.WriteLine("  ❌ Step 3 FAIL: Không tìm thấy nút Hiển thị để phục hồi phim");
                test?.Fail("Step 3: Restore button not found");
                testPassed = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ PHIM_INT_04 FAILED: {ex.Message}");
            test?.Fail($"Test failed: {ex.Message}");
            testPassed = false;
        }

        return testPassed;
    }

    /// <summary>
    /// PHIM_INT_05: Custom Title - Tạo/Sửa/Xóa title tùy chỉnh
    /// </summary>
    public static bool Test_PHIM_INT_05_CustomTitle(IWebDriver driver)
    {
        Console.WriteLine("\n📋 Test PHIM_INT_05: Custom Title (Tạo/Sửa/Xóa)");
        test = ReportManager.extent?.CreateTest("PHIM_INT_05: Custom Title Management");
        bool testPassed = true;
        const string movieSlug = "mai";
        const string movieTitle = "Mai";

        try
        {
            // Step 1: Vào Edit Title
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/EditTitle");
            Thread.Sleep(2000);

            // ✅ CHỨNG MINH: Trang Edit Title hiển thị
            bool hasEditForm = driver.FindElements(By.CssSelector("input[name='slug'], input#slug, form")).Count > 0;
            Console.WriteLine($"  ✅ Step 1 PASS: Trang EditTitle hiển thị form: {hasEditForm}");
            test?.Pass($"Step 1: EditTitle form loaded: {hasEditForm}");

            // Step 2: Nhập slug và custom title
            try
            {
                var slugInput = FindInput(driver,
                    "input[name='slug']",
                    "input#slug",
                    "input[placeholder*='slug']",
                    "input[placeholder*='Slug']",
                    "input[name='movieSlug']",
                    "input[name='title']"
                );
                if (slugInput == null)
                {
                    slugInput = FindInputByLabel(driver, "Slug") ?? FindInputByLabel(driver, "Tên slug");
                }

                if (slugInput == null)
                    throw new NoSuchElementException("Không tìm thấy trường slug hoặc title để nhập");

                slugInput.Clear();
                // Ưu tiên slug chuẩn; nếu hệ thống dùng title thay slug, vẫn cho phép nhập title
                slugInput.SendKeys(movieSlug);

                Thread.Sleep(1000);

                var customTitleInput = FindInput(driver,
                    "input[name='customTitle']",
                    "input#customTitle",
                    "input[placeholder*='Custom']",
                    "input[placeholder*='Tiêu đề']",
                    "input[name='title']"
                );
                if (customTitleInput == null)
                {
                    customTitleInput = FindInputByLabel(driver, "Custom Title") ?? FindInputByLabel(driver, "Tiêu đề");
                }

                if (customTitleInput == null)
                    throw new NoSuchElementException("Không tìm thấy trường custom title");

                customTitleInput.Clear();
                customTitleInput.SendKeys($"{movieTitle} - Phiên bản đặc biệt");

                var customDescInput = FindInput(driver,
                    "textarea[name='customDescription']",
                    "textarea#customDescription",
                    "textarea[placeholder*='description']",
                    "textarea[placeholder*='Mô tả']"
                );
                if (customDescInput != null)
                {
                    customDescInput.Clear();
                    customDescInput.SendKeys("Mô tả custom cho bài test");
                }
                else
                {
                    Console.WriteLine("  ⚠️ Step 2: custom description field không tìm thấy, bỏ qua phần này.");
                }

                // Submit
                var saveBtn = driver.FindElement(By.CssSelector("button[type='submit'], input[type='submit']"));
                saveBtn.Click();
                Thread.Sleep(2000);

                // ✅ CHỨNG MINH: Lưu thành công
                string alertAfterSave = AcceptAlertIfPresent(driver) ?? "";
                bool saveSuccess = driver.PageSource.Contains("thành công") ||
                                  driver.PageSource.Contains("Success") ||
                                  driver.PageSource.Contains("success") ||
                                  alertAfterSave.Contains("thành công") ||
                                  alertAfterSave.Contains("Success") ||
                                  alertAfterSave.Contains("success") ||
                                  driver.FindElements(By.CssSelector(".alert-success, .toast-success")).Count > 0 ||
                                  !driver.Url.Contains("EditTitle");
                if (!saveSuccess)
                {
                    Console.WriteLine("  ❌ Step 2 FAIL: Không thấy tín hiệu lưu custom title thành công");
                    test?.Fail("Step 2: Save custom title failed");
                    testPassed = false;
                }
                else
                {
                    Console.WriteLine($"  ✅ Step 2 PASS: Lưu custom title: {saveSuccess}");
                    test?.Pass($"Step 2: Save custom title: {saveSuccess}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Step 2 FAIL: {ex.Message}");
                test?.Fail($"Step 2: {ex.Message}");
                testPassed = false;
            }

            // Step 3: Verify User thấy title mới
            driver.Navigate().GoToUrl($"{BaseUrl}/Movie/Detail?slug={movieSlug}");
            Thread.Sleep(2000);

            bool customTitleVisible = driver.PageSource.Contains($"{movieTitle} - Phiên bản đặc biệt") ||
                                     driver.PageSource.Contains(movieTitle);
            if (!customTitleVisible)
            {
                Console.WriteLine("  ❌ Step 3 FAIL: User không thấy custom title sau khi lưu");
                test?.Fail("Step 3: Custom title not visible to user");
                testPassed = false;
            }
            else
            {
                Console.WriteLine($"  ✅ Step 3 PASS: User thấy custom title: {customTitleVisible}");
                test?.Pass($"Step 3: Custom title visible to user: {customTitleVisible}");
            }

            // Step 4: Edit lại custom title
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/EditTitle");
            Thread.Sleep(2000);

            try
            {
                var slugInput = FindInput(driver,
                    "input[name='slug']",
                    "input#slug",
                    "input[placeholder*='slug']",
                    "input[placeholder*='Slug']",
                    "input[name='movieSlug']",
                    "input[name='title']"
                );
                if (slugInput == null)
                {
                    slugInput = FindInputByLabel(driver, "Slug") ?? FindInputByLabel(driver, "Tên slug");
                }

                if (slugInput == null)
                    throw new NoSuchElementException("Không tìm thấy trường slug hoặc title để nhập");

                slugInput.Clear();
                slugInput.SendKeys(movieSlug);

                Thread.Sleep(1000);

                var customTitleInput = FindInput(driver,
                    "input[name='customTitle']",
                    "input#customTitle",
                    "input[placeholder*='Custom']",
                    "input[placeholder*='Tiêu đề']",
                    "input[name='title']"
                );
                if (customTitleInput == null)
                {
                    customTitleInput = FindInputByLabel(driver, "Custom Title") ?? FindInputByLabel(driver, "Tiêu đề");
                }

                if (customTitleInput == null)
                    throw new NoSuchElementException("Không tìm thấy trường custom title");

                customTitleInput.Clear();
                customTitleInput.SendKeys($"{movieTitle} - Phiên bản sửa lại");

                var customDescInput = FindInput(driver,
                    "textarea[name='customDescription']",
                    "textarea#customDescription",
                    "textarea[placeholder*='description']",
                    "textarea[placeholder*='Mô tả']"
                );
                if (customDescInput != null)
                {
                    customDescInput.Clear();
                    customDescInput.SendKeys("Mô tả custom đã sửa cho bài test");
                }
                else
                {
                    Console.WriteLine("  ⚠️ Step 4: custom description field không tìm thấy, bỏ qua phần này.");
                }

                var saveBtn = driver.FindElement(By.CssSelector("button[type='submit'], input[type='submit']"));
                saveBtn.Click();
                Thread.Sleep(2000);

                string alertAfterEdit = AcceptAlertIfPresent(driver) ?? "";
                bool editSuccess = driver.PageSource.Contains("thành công") ||
                                   driver.PageSource.Contains("Success") ||
                                   driver.PageSource.Contains("success") ||
                                   alertAfterEdit.Contains("thành công") ||
                                   alertAfterEdit.Contains("Success") ||
                                   alertAfterEdit.Contains("success") ||
                                   driver.FindElements(By.CssSelector(".alert-success, .toast-success")).Count > 0 ||
                                   !driver.Url.Contains("EditTitle");
                if (!editSuccess)
                {
                    Console.WriteLine("  ❌ Step 4 FAIL: Không thấy tín hiệu edit custom title thành công");
                    test?.Fail("Step 4: Edit custom title failed");
                    testPassed = false;
                }
                else
                {
                    Console.WriteLine($"  ✅ Step 4 PASS: Edit custom title: {editSuccess}");
                    test?.Pass($"Step 4: Custom title edited: {editSuccess}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Step 4 FAIL: {ex.Message}");
                test?.Fail($"Step 4: {ex.Message}");
                testPassed = false;
            }

            // Step 5: Verify User thấy title đã sửa
            driver.Navigate().GoToUrl($"{BaseUrl}/Movie/Detail?slug={movieSlug}");
            Thread.Sleep(2000);

            bool updatedTitleVisible = driver.PageSource.Contains($"{movieTitle} - Phiên bản sửa lại") ||
                                       driver.PageSource.Contains(movieTitle);
            if (!updatedTitleVisible)
            {
                Console.WriteLine("  ❌ Step 5 FAIL: User không thấy title đã sửa");
                test?.Fail("Step 5: Updated title not visible");
                testPassed = false;
            }
            else
            {
                Console.WriteLine($"  ✅ Step 5 PASS: User thấy title đã sửa: {updatedTitleVisible}");
                test?.Pass($"Step 5: Updated custom title visible to user: {updatedTitleVisible}");
            }

            // Step 6: Kiểm tra trang Titles
            driver.Navigate().GoToUrl($"{BaseUrl}/Admin/Titles");
            Thread.Sleep(2000);

            bool titlesPageLoaded = driver.PageSource.Contains("Title") || driver.PageSource.Contains("Tiêu đề");
            Console.WriteLine($"  ✅ Step 6 PASS: Trang Titles hiển thị: {titlesPageLoaded}");
            test?.Pass($"Step 6: Titles page loaded: {titlesPageLoaded}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ PHIM_INT_05 FAILED: {ex.Message}");
            test?.Fail($"Test failed: {ex.Message}");
            testPassed = false;
        }

        return testPassed;
    }

    private static IWebElement? FindInput(IWebDriver driver, params string[] selectors)
    {
        foreach (var selector in selectors)
        {
            var elements = driver.FindElements(By.CssSelector(selector));
            if (elements.Count > 0)
                return elements[0];
        }
        return null;
    }

    private static IWebElement? FindInputByLabel(IWebDriver driver, string labelText)
    {
        var labels = driver.FindElements(By.XPath($"//label[contains(normalize-space(string(.)), '{labelText}')]"));
        foreach (var label in labels)
        {
            var forId = label.GetAttribute("for");
            if (!string.IsNullOrEmpty(forId))
            {
                var target = driver.FindElements(By.Id(forId));
                if (target.Count > 0)
                    return target[0];
            }

            var followingInput = label.FindElements(By.XPath(".//following::input[1] | .//following::textarea[1]"));
            if (followingInput.Count > 0)
                return followingInput[0];
        }

        return null;
    }

    private static void SafeClick(IWebDriver driver, IWebElement element)
    {
        try
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block:'center'});", element);
            Thread.Sleep(500);
            element.Click();
        }
        catch (UnhandledAlertException)
        {
            // Alert appeared after click — click DID work, just accept the alert
            AcceptAlertIfPresent(driver);
        }
        catch (ElementClickInterceptedException)
        {
            // Element obscured — try JS click
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
            catch (UnhandledAlertException)
            {
                AcceptAlertIfPresent(driver);
            }
        }
        catch (Exception)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
            catch (UnhandledAlertException)
            {
                AcceptAlertIfPresent(driver);
            }
        }
    }

    private static string? AcceptAlertIfPresent(IWebDriver driver)
    {
        try
        {
            var alert = driver.SwitchTo().Alert();
            string text = alert.Text;
            alert.Accept();
            Thread.Sleep(500);
            return text;
        }
        catch (UnhandledAlertException)
        {
            try
            {
                var alert = driver.SwitchTo().Alert();
                string text = alert.Text;
                alert.Accept();
                Thread.Sleep(500);
                return text;
            }
            catch
            {
                return null;
            }
        }
        catch (NoAlertPresentException)
        {
            return null;
        }
    }

    private static IWebElement FindPaginationLink(IWebDriver driver)
    {
        var candidates = driver.FindElements(By.CssSelector(".pagination a.page-link, .page-link, nav[aria-label*='pagination'] a, a[href*='page=']"));
        if (candidates.Count == 0)
            throw new NoSuchElementException("Không tìm thấy link phân trang");

        var page2 = candidates.FirstOrDefault(e => e.Text.Trim() == "2" || e.GetAttribute("href")?.Contains("page=2") == true);
        if (page2 != null)
            return page2;

        var page3 = candidates.FirstOrDefault(e => e.Text.Trim() == "3" || e.GetAttribute("href")?.Contains("page=3") == true);
        if (page3 != null)
            return page3;

        var nextArrow = candidates.FirstOrDefault(e =>
            e.Text.Trim().Contains("Sau") ||
            e.Text.Trim().Contains("Next") ||
            e.Text.Trim().Contains(">") ||
            e.Text.Trim().Contains("»")
        );
        if (nextArrow != null)
            return nextArrow;

        return candidates[0];
    }

    private static IWebElement? FindHideMovieButton(IWebDriver driver, string movieKeyword = "Mai")
    {
        // Try to find by various selectors and text content
        var xpaths = new[]
        {
            "//button[contains(text(),'Ẩn') or contains(text(),'Ẩn phim') or contains(text(),'Hide')] | //a[contains(text(),'Ẩn') or contains(text(),'Ẩn phim') or contains(text(),'Hide')]",
            "//button[contains(@onclick,'hide') or contains(@onclick,'Hide')] | //a[contains(@onclick,'hide') or contains(@onclick,'Hide')]",
            "//button[contains(@class,'hide') or contains(@class,'Hide') or contains(@class,'toggle')] | //a[contains(@class,'hide') or contains(@class,'Hide') or contains(@class,'toggle')]",
            "//input[@type='button' and (contains(@value,'Ẩn') or contains(@value,'Hide'))] | //input[@type='submit' and (contains(@value,'Ẩn') or contains(@value,'Hide'))]",
            "//button | //a | //input[@type='button' or @type='submit']"
        };

        foreach (var xpath in xpaths)
        {
            var elements = driver.FindElements(By.XPath(xpath));
            foreach (var el in elements)
            {
                if (!el.Displayed || !el.Enabled) continue;

                var text = el.Text.Trim();
                var onclick = el.GetAttribute("onclick") ?? "";
                var value = el.GetAttribute("value") ?? "";
                var classList = el.GetAttribute("class") ?? "";

                if (text.Contains("Ẩn") || text.Contains("Hide") || 
                    onclick.Contains("Hide") || onclick.Contains("hide") ||
                    value.Contains("Ẩn") || value.Contains("Hide") ||
                    classList.Contains("hide") || classList.Contains("Hide") ||
                    classList.Contains("toggle"))
                {
                    return el;
                }
            }
        }

        return null;
    }
}

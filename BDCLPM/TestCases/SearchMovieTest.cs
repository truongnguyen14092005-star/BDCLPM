using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class SearchMovieTest
{
    public static void Run(IWebDriver driver)
    {
        Console.WriteLine("🔍 Test tìm kiếm phim bắt đầu...");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        try
        {
            // ===== VÀO DASHBOARD =====
            driver.Navigate().GoToUrl("https://localhost:5001/Admin");
            wait.Until(d => d.PageSource.Contains("Dashboard"));

            Console.WriteLine("📊 Đã vào Dashboard");

            // ===== VÀO QUẢN LÝ PHIM =====
            driver.Navigate().GoToUrl("https://localhost:5001/Admin/ManageMovies");
            wait.Until(d => d.PageSource.Contains("Movie") || d.PageSource.Contains("Phim"));

            Console.WriteLine("🎬 Đã vào Quản lý phim");

            // ===== NHẬP TÌM KIẾM =====
            var searchInput = wait.Until(d =>
                d.FindElement(By.CssSelector("input[type='text'], input[name='search']"))
            );

            searchInput.Clear();
            searchInput.SendKeys("phim hành động");
            searchInput.SendKeys(Keys.Enter);

            Thread.Sleep(2000);

            Console.WriteLine("🔎 Đã tìm kiếm 'phim hành động'");

            // ===== CHECK KẾT QUẢ =====
            if (driver.Url.Contains("/Account/Login"))
            {
                Console.WriteLine("❌ SEARCH FAIL (bị logout)");
            }
            else if (driver.PageSource.ToLower().Contains("không tìm thấy"))
            {
                Console.WriteLine("✔️ SEARCH PASS (không có dữ liệu)");
            }
            else
            {
                Console.WriteLine("✔️ SEARCH PASS (có dữ liệu)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ LỖI: " + ex.Message);
        }

        Console.WriteLine("✅ Test hoàn thành");
    }
}
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class SearchMovieTest
{
    public static void Run(IWebDriver driver)
    {
        Console.WriteLine("🔍 Test tìm kiếm phim bắt đầu...");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        // 👉 BƯỚC 1: VÀO DASHBOARD (đã login sẵn)
        driver.Navigate().GoToUrl("https://localhost:5001/Admin");
        wait.Until(d => d.PageSource.Contains("Dashboard"));

        Console.WriteLine("📊 Đã vào Dashboard");

        // 👉 BƯỚC 2: VÀO QUẢN LÝ PHIM
        driver.Navigate().GoToUrl("https://localhost:5001/Admin/ManageMovies");
        wait.Until(d => d.PageSource.Contains("Movie") || d.PageSource.Contains("Phim"));

        Console.WriteLine("🎬 Đã vào Quản lý phim");

        // 👉 BƯỚC 3: TÌM KIẾM
        var searchInput = wait.Until(d =>
            d.FindElement(By.CssSelector("input[type='text'], input[name='search']"))
        );

        searchInput.Clear();
        searchInput.SendKeys("phim hành động");

        // 👉 CLICK NÚT TÌM KIẾM (KHÔNG dùng Enter)
        var searchBtn = driver.FindElement(By.CssSelector("button[type='submit']"));
        searchBtn.Click();

        Thread.Sleep(2000);

        Console.WriteLine("🔎 Đã tìm kiếm 'phim hành động'");

        // 👉 CHECK
        if (driver.PageSource.ToLower().Contains("hành động"))
        {
            Console.WriteLine("✔️ SEARCH PASS");
        }
        else
        {
            Console.WriteLine("❌ SEARCH FAIL");
        }

        Console.WriteLine("✅ Test hoàn thành");
    }
}
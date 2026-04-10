using OpenQA.Selenium;

public class UserTest
{
    public static void Run(IWebDriver driver)
    {
        Console.WriteLine("👤 User Test bắt đầu...");

        ReportManager.test = ReportManager.extent.CreateTest("User Flow");

        driver.Navigate().GoToUrl("https://localhost:5001/");

        Thread.Sleep(2000);

        // search
        var search = driver.FindElement(By.CssSelector("input[placeholder='Search...']"));
        search.SendKeys("ga");
        search.SendKeys(Keys.Enter);

        Thread.Sleep(2000);

        Console.WriteLine("🔍 Đã search");

        // click phim
        driver.FindElement(By.CssSelector("a[href*='/Movie/Detail']")).Click();

        Thread.Sleep(2000);

        Console.WriteLine("🎬 Đã vào phim");

        // comment
        driver.FindElement(By.CssSelector("textarea")).SendKeys("Test Selenium");
        driver.FindElement(By.XPath("//button[contains(text(),'Gửi bình luận')]")).Click();

        Thread.Sleep(2000);

        Console.WriteLine("💬 Đã comment");

        ReportManager.test.Pass("User flow OK");
    }
}
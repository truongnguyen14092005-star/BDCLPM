using OpenQA.Selenium;

public class LoginTest
{
    public static void Run(IWebDriver driver)
    {
        Console.WriteLine("🔐 Login Test bắt đầu...");

        driver.Navigate().GoToUrl("https://localhost:5001/Account/Login");

        Thread.Sleep(2000);

        var emailInput = driver.FindElement(By.CssSelector("input[type='email'], input[name='Email']"));
        var passwordInput = driver.FindElement(By.CssSelector("input[type='password']"));

        emailInput.Clear();
        emailInput.SendKeys("admin@webmovie.com");

        passwordInput.Clear();
        passwordInput.SendKeys("Admin123!");

        Console.WriteLine("👉 Đã nhập email + password");

        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        Thread.Sleep(3000);

        if (!driver.Url.Contains("Login"))
        {
            Console.WriteLine("✅ LOGIN THÀNH CÔNG");
        }
        else
        {
            Console.WriteLine("❌ LOGIN THẤT BẠI");
        }
    }
}
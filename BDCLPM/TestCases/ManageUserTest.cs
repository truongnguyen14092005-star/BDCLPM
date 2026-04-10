using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class ManageUserTest
{
    public static void Run(IWebDriver driver)
    {
        Console.WriteLine("👤 User Management Test bắt đầu...");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        try
        {
            // ===== 1. VÀO DASHBOARD =====
            driver.Navigate().GoToUrl("https://localhost:5001/Admin");
            wait.Until(d => d.PageSource.Contains("Dashboard"));

            Console.WriteLine("📊 Đã vào Dashboard");

            // ===== 2. VÀO QUẢN LÝ USER =====
            driver.Navigate().GoToUrl("https://localhost:5001/Admin/ManageUsers");
            Thread.Sleep(2000);

            Console.WriteLine("👤 Đã vào Quản lý người dùng");

            // ===== 3. CLICK NÚT XÓA =====
            var deleteBtn = wait.Until(d =>
                d.FindElement(By.CssSelector("button[title='Xóa người dùng']"))
            );

            // 👉 Scroll tới button (tránh lỗi không click được)
            ((IJavaScriptExecutor)driver)
                .ExecuteScript("arguments[0].scrollIntoView(true);", deleteBtn);

            Thread.Sleep(500);

            deleteBtn.Click();

            Console.WriteLine("🗑️ Đã click nút xóa user");

            // ===== 4. XỬ LÝ ALERT =====
            try
            {
                IAlert alert = wait.Until(d => d.SwitchTo().Alert());

                Console.WriteLine("⚠️ Alert: " + alert.Text);

                alert.Accept(); // 👉 bấm OK

                Console.WriteLine("✔️ Đã bấm OK");
            }
            catch
            {
                Console.WriteLine("❌ Không thấy alert");
            }

            Thread.Sleep(2000);

            Console.WriteLine("✔️ XÓA USER HOÀN TẤT (nếu hệ thống OK)");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ LỖI: " + ex.Message);
        }

        Console.WriteLine("✅ Test hoàn thành");
    }
}
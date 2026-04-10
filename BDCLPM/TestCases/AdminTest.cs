using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class AdminTest
{
    public static void Run(IWebDriver driver)
    {
        Console.WriteLine("👨‍💼 Admin Test bắt đầu...");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        try
        {
            // 👉 Vào Dashboard
            driver.Navigate().GoToUrl("https://localhost:5001/Admin");
            wait.Until(d => d.PageSource.Contains("Dashboard"));

            Console.WriteLine("📊 Đã vào Dashboard");

            // 👉 Vào quản lý bình luận
            driver.Navigate().GoToUrl("https://localhost:5001/Admin/ManageComments");
            Thread.Sleep(2000);

            Console.WriteLine("💬 Đã vào trang quản lý bình luận");

            // 👉 Click nút xóa
            var deleteBtn = wait.Until(d =>
                d.FindElement(By.CssSelector(".deleteCommentBtn"))
            );

            deleteBtn.Click();

            Console.WriteLine("🗑️ Đã click nút xóa");

            // ===== XỬ LÝ ALERT =====
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

            Console.WriteLine("✔️ XÓA HOÀN TẤT (nếu hệ thống OK)");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ LỖI: " + ex.Message);
        }

        Console.WriteLine("✅ Test hoàn thành");
    }
}
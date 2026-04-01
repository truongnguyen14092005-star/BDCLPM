using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class AdminTest
{
    public static void Run(IWebDriver driver)
    {
        Console.WriteLine("👨‍💼 Admin Test bắt đầu...");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        // 👉 Vào Dashboard
        driver.Navigate().GoToUrl("https://localhost:5001/Admin");
        wait.Until(d => d.PageSource.Contains("Dashboard"));

        Console.WriteLine("📊 Đã vào Dashboard");

        // 👉 Vào trang quản lý bình luận
        driver.Navigate().GoToUrl("https://localhost:5001/Admin/ManageComments");
        Thread.Sleep(2000);

        Console.WriteLine("💬 Đã vào trang quản lý bình luận");

        // 👉 XÓA COMMENT CHUẨN 100%
        try
        {
            // 👉 Lấy dòng đầu tiên (bỏ header)
            var firstRow = wait.Until(d =>
                d.FindElement(By.XPath("(//table//tr)[2]"))
            );

            // 👉 Lấy cột Action (cuối)
            var lastCell = firstRow.FindElement(By.XPath("./td[last()]"));

            // 👉 CHỌN ĐÚNG NÚT XÓA THEO CLASS
            var deleteBtn = lastCell.FindElement(By.CssSelector(".deleteCommentBtn"));

            deleteBtn.Click();

            Console.WriteLine("🗑️ Đã click nút xóa");

            // 👉 Xử lý confirm (nếu có)
            try
            {
                var confirmBtn = wait.Until(d =>
                    d.FindElement(By.XPath("//button[contains(text(),'OK') or contains(text(),'Yes') or contains(text(),'Xác nhận')]"))
                );

                confirmBtn.Click();

                Console.WriteLine("✔️ Đã xác nhận xóa");
            }
            catch
            {
                Console.WriteLine("⚠️ Không có popup xác nhận");
            }
        }
        catch
        {
            Console.WriteLine("❌ Không tìm thấy nút xóa");
        }

        Console.WriteLine("✅ Admin Test hoàn thành");
    }
}
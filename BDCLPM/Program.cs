using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== MENU TEST =====");
            Console.WriteLine("1. Login Test");
            Console.WriteLine("2. Admin Test (Xóa comment)");
            Console.WriteLine("3. Search Movie Test");
            Console.WriteLine("0. Thoát");
            Console.Write("👉 Chọn chức năng: ");

            string choice = Console.ReadLine();

            if (choice == "0") break;

            IWebDriver driver = new ChromeDriver();

            try
            {
                switch (choice)
                {
                    case "1":
                        LoginTest.Run(driver);
                        break;

                    case "2":
                        LoginTest.Run(driver); // cần login trước
                        AdminTest.Run(driver);
                        break;

                    case "3":
                        LoginTest.Run(driver); // cần login trước
                        SearchMovieTest.Run(driver);
                        break;

                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ LỖI: " + ex.Message);
            }

            Console.WriteLine("\n👉 Nhấn Enter để quay lại menu...");
            Console.ReadLine();

            driver.Quit();
        }
    }
}
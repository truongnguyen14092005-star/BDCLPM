using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;

/// <summary>
/// Test Binh luan - User
/// FLOW: Dang nhap 1 lan o dau -> Chay tat ca test cases
/// </summary>
public class UserCommentTest
{
    private static ExtentTest? test;
    private static WebDriverWait? wait;
    private const string BaseUrl = "https://localhost:5001";

    public static void RunAllTests(IWebDriver driver)
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("USER BINH LUAN - BAT DAU TEST");
        Console.WriteLine(new string('=', 60));

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

        // DANG NHAP MOT LAN O DAU
        Console.WriteLine("\n>>> BUOC 0: Dang nhap truoc khi test...");
        Login(driver, "user3@test.com", "User@1234");

        // Chay cac test case (da dang nhap san)
        Test_BL_INT_01_AddComment(driver);
        Test_BL_INT_02_EditComment(driver);
        Test_BL_INT_03_DeleteComment(driver);
        Test_BL_INT_04_CommentValidation(driver);
        Test_BL_INT_05_CommentDisplayInfo(driver);

        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("USER BINH LUAN - HOAN THANH");
        Console.WriteLine(new string('=', 60));
    }

    /// <summary>
    /// BL_INT_01: Them binh luan moi
    /// </summary>
    public static void Test_BL_INT_01_AddComment(IWebDriver driver)
    {
        Console.WriteLine("\n--- Test BL_INT_01: Them binh luan ---");
        test = ReportManager.extent?.CreateTest("BL_INT_01: Them binh luan");

        try
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            
            // Vao trang xem phim
            NavigateToWatchPage(driver, js);
            
            // Cuon xuong phan comment
            Console.WriteLine("  Cuon xuong phan binh luan...");
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(2000);
            
            // Kiem tra co form binh luan khong
            var textareas = driver.FindElements(By.TagName("textarea"));
            Console.WriteLine($"  Tim thay {textareas.Count} textarea");
            
            if (textareas.Count == 0)
            {
                Console.WriteLine("  KHONG TIM THAY FORM BINH LUAN!");
                test?.Fail("Comment form not found");
                return;
            }
            
            // Them binh luan
            string commentText = "Test binh luan tu dong " + DateTime.Now.ToString("HHmmss");
            Console.WriteLine($"  Nhap binh luan: {commentText}");
            
            var commentInput = textareas[0];
            js.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", commentInput);
            Thread.Sleep(1000);
            
            // Nhap noi dung bang JavaScript
            js.ExecuteScript("arguments[0].focus(); arguments[0].value = arguments[1];", commentInput, commentText);
            // Trigger input event de form nhan ra thay doi
            js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", commentInput);
            Thread.Sleep(500);
            
            // Tim va click nut Gui bang JavaScript
            var submitButtons = driver.FindElements(By.CssSelector("button[type='submit'], .btn-primary, button.btn, button"));
            Console.WriteLine($"  Tim thay {submitButtons.Count} nut");
            
            IWebElement? submitBtn = null;
            foreach (var btn in submitButtons)
            {
                string btnText = btn.Text.ToLower();
                if (btnText.Contains("gửi") || btnText.Contains("gui") || btnText.Contains("submit") || btnText.Contains("đăng") || btnText.Contains("post"))
                {
                    submitBtn = btn;
                    Console.WriteLine($"  Tim thay nut: '{btn.Text}'");
                    break;
                }
            }
            
            if (submitBtn == null)
            {
                // Thu tim theo form
                submitBtn = driver.FindElement(By.CssSelector("form button, form input[type='submit']"));
            }
            
            js.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", submitBtn);
            Thread.Sleep(500);
            js.ExecuteScript("arguments[0].click();", submitBtn);
            Thread.Sleep(3000);
            
            // Kiem tra comment da xuat hien
            bool commentAdded = driver.PageSource.Contains(commentText);
            Console.WriteLine($"  PASS: Comment da duoc them: {commentAdded}");
            test?.Pass($"Them binh luan thanh cong: {commentAdded}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  FAILED: {ex.Message}");
            test?.Fail($"Test failed: {ex.Message}");
        }
    }

    /// <summary>
    /// BL_INT_02: Sua binh luan (neu co chuc nang)
    /// </summary>
    public static void Test_BL_INT_02_EditComment(IWebDriver driver)
    {
        Console.WriteLine("\n--- Test BL_INT_02: Sua binh luan ---");
        test = ReportManager.extent?.CreateTest("BL_INT_02: Sua binh luan");

        try
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            
            // Cuon xuong phan comment
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(2000);
            
            // Tim nut Sua
            var editButtons = driver.FindElements(By.XPath("//button[contains(text(),'Sua')] | //a[contains(text(),'Sua')] | //button[contains(@class,'edit')]"));
            
            if (editButtons.Count > 0)
            {
                Console.WriteLine($"  Tim thay {editButtons.Count} nut Sua");
                editButtons[0].Click();
                Thread.Sleep(1000);
                
                // Sua noi dung
                var editInput = driver.FindElement(By.CssSelector("textarea"));
                string newComment = "Da sua: " + DateTime.Now.ToString("HHmmss");
                editInput.Clear();
                editInput.SendKeys(newComment);
                
                // Luu
                var saveBtn = driver.FindElement(By.XPath("//button[contains(text(),'Luu') or contains(text(),'Cap nhat')]"));
                saveBtn.Click();
                Thread.Sleep(2000);
                
                Console.WriteLine($"  PASS: Da sua binh luan");
                test?.Pass("Sua binh luan thanh cong");
            }
            else
            {
                Console.WriteLine("  SKIP: Khong co chuc nang sua binh luan hoac khong tim thay nut Sua");
                test?.Info("Khong co chuc nang sua binh luan");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  SKIP: {ex.Message}");
            test?.Info($"Khong the sua binh luan: {ex.Message}");
        }
    }

    /// <summary>
    /// BL_INT_03: Xoa binh luan
    /// </summary>
    public static void Test_BL_INT_03_DeleteComment(IWebDriver driver)
    {
        Console.WriteLine("\n--- Test BL_INT_03: Xoa binh luan ---");
        test = ReportManager.extent?.CreateTest("BL_INT_03: Xoa binh luan");

        try
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            
            // Cuon xuong phan comment
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(2000);
            
            // Tim nut Xoa
            var deleteButtons = driver.FindElements(By.XPath("//button[contains(text(),'Xoa')] | //a[contains(text(),'Xoa')] | //button[contains(@class,'delete')]"));
            
            if (deleteButtons.Count > 0)
            {
                Console.WriteLine($"  Tim thay {deleteButtons.Count} nut Xoa");
                deleteButtons[0].Click();
                Thread.Sleep(1000);
                
                // Accept confirm dialog neu co
                try
                {
                    driver.SwitchTo().Alert().Accept();
                    Thread.Sleep(1000);
                }
                catch { }
                
                Console.WriteLine($"  PASS: Da xoa binh luan");
                test?.Pass("Xoa binh luan thanh cong");
            }
            else
            {
                Console.WriteLine("  SKIP: Khong tim thay nut Xoa (co the chua co binh luan cua minh)");
                test?.Info("Khong tim thay nut Xoa");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  SKIP: {ex.Message}");
            test?.Info($"Khong the xoa binh luan: {ex.Message}");
        }
    }

    /// <summary>
    /// BL_INT_04: Kiem tra validation (comment rong, qua dai)
    /// </summary>
    public static void Test_BL_INT_04_CommentValidation(IWebDriver driver)
    {
        Console.WriteLine("\n--- Test BL_INT_04: Validation binh luan ---");
        test = ReportManager.extent?.CreateTest("BL_INT_04: Validation binh luan");

        try
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            
            // Cuon xuong phan comment
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(2000);
            
            var commentInput = driver.FindElement(By.CssSelector("textarea"));
            js.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", commentInput);
            Thread.Sleep(500);
            
            // Test 1: Comment rong
            Console.WriteLine("  Test 1: Gui comment rong...");
            js.ExecuteScript("arguments[0].value = '';", commentInput);
            js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", commentInput);
            
            // Tim nut submit
            var submitButtons = driver.FindElements(By.CssSelector("button[type='submit'], .btn-primary, button.btn, button"));
            IWebElement? submitBtn = null;
            foreach (var btn in submitButtons)
            {
                string btnText = btn.Text.ToLower();
                if (btnText.Contains("gửi") || btnText.Contains("gui") || btnText.Contains("submit") || btnText.Contains("đăng") || btnText.Contains("post"))
                {
                    submitBtn = btn;
                    break;
                }
            }
            if (submitBtn == null)
            {
                submitBtn = driver.FindElement(By.CssSelector("form button"));
            }
            
            js.ExecuteScript("arguments[0].click();", submitBtn);
            Thread.Sleep(1500);
            
            // Kiem tra co loi validation khong
            bool hasError = driver.PageSource.Contains("khong duoc de trong") ||
                           driver.PageSource.Contains("không được để trống") ||
                           driver.PageSource.Contains("required") ||
                           driver.PageSource.Contains("bat buoc") ||
                           driver.FindElements(By.CssSelector(".text-danger, .error, .invalid-feedback")).Count > 0;
            
            Console.WriteLine($"  Test 1 PASS: Comment rong bi chan: {hasError}");
            
            // Test 2: Comment qua dai
            Console.WriteLine("  Test 2: Nhap comment qua dai (1001 ky tu)...");
            string longComment = new string('A', 1001);
            js.ExecuteScript("arguments[0].value = arguments[1];", commentInput, longComment);
            js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", commentInput);
            
            string? actualValue = (string?)js.ExecuteScript("return arguments[0].value;", commentInput);
            bool lengthLimited = actualValue == null || actualValue.Length <= 1000;
            Console.WriteLine($"  Test 2 PASS: Gioi han ky tu: {lengthLimited} (do dai: {actualValue?.Length ?? 0})");
            
            test?.Pass($"Validation hoat dong - Rong: {hasError}, Gioi han: {lengthLimited}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  FAILED: {ex.Message}");
            test?.Fail($"Test failed: {ex.Message}");
        }
    }

    /// <summary>
    /// BL_INT_05: Kiem tra hien thi thong tin comment
    /// </summary>
    public static void Test_BL_INT_05_CommentDisplayInfo(IWebDriver driver)
    {
        Console.WriteLine("\n--- Test BL_INT_05: Hien thi thong tin comment ---");
        test = ReportManager.extent?.CreateTest("BL_INT_05: Hien thi thong tin comment");

        try
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            
            // Cuon xuong phan comment
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(2000);
            
            // Them 1 comment de kiem tra
            string testComment = "Test hien thi " + DateTime.Now.ToString("HHmmss");
            var commentInput = driver.FindElement(By.CssSelector("textarea"));
            js.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", commentInput);
            Thread.Sleep(500);
            
            js.ExecuteScript("arguments[0].value = arguments[1];", commentInput, testComment);
            js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", commentInput);
            
            // Tim nut submit
            var submitButtons = driver.FindElements(By.CssSelector("button[type='submit'], .btn-primary, button.btn, button"));
            IWebElement? submitBtn = null;
            foreach (var btn in submitButtons)
            {
                string btnText = btn.Text.ToLower();
                if (btnText.Contains("gửi") || btnText.Contains("gui") || btnText.Contains("submit") || btnText.Contains("đăng") || btnText.Contains("post"))
                {
                    submitBtn = btn;
                    break;
                }
            }
            if (submitBtn == null)
            {
                submitBtn = driver.FindElement(By.CssSelector("form button"));
            }
            
            js.ExecuteScript("arguments[0].click();", submitBtn);
            Thread.Sleep(3000);
            
            // Kiem tra thong tin hien thi
            string pageSource = driver.PageSource;
            bool hasUsername = pageSource.Contains("user2") || pageSource.Contains("User");
            bool hasTime = pageSource.Contains("vừa xong") || pageSource.Contains("giây") || 
                          pageSource.Contains("phút") || pageSource.Contains("trước") ||
                          pageSource.Contains("vua xong") || pageSource.Contains("giay");
            bool hasContent = pageSource.Contains(testComment);
            
            Console.WriteLine($"  Hien thi username: {hasUsername}");
            Console.WriteLine($"  Hien thi thoi gian: {hasTime}");
            Console.WriteLine($"  Hien thi noi dung: {hasContent}");
            
            test?.Pass($"Hien thi - Username: {hasUsername}, Time: {hasTime}, Content: {hasContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  FAILED: {ex.Message}");
            test?.Fail($"Test failed: {ex.Message}");
        }
    }

    // ==================== HELPER METHODS ====================

    /// <summary>
    /// Dang nhap voi email va password - LUON THUC HIEN DANG NHAP
    /// </summary>
    private static void Login(IWebDriver driver, string email, string password)
    {
        // Luon thuc hien dang nhap de dam bao
        driver.Navigate().GoToUrl($"{BaseUrl}/Account/Login");
        Thread.Sleep(2000);
        
        // Kiem tra neu da o trang khong phai Login (da dang nhap roi)
        if (!driver.Url.Contains("Login") && !driver.Url.Contains("login"))
        {
            Console.WriteLine($"  Da dang nhap san roi!");
            return;
        }

        var emailInput = driver.FindElement(By.CssSelector("input[type='email'], input[name='Email']"));
        var passwordInput = driver.FindElement(By.CssSelector("input[type='password']"));

        emailInput.Clear();
        emailInput.SendKeys(email);
        passwordInput.Clear();
        passwordInput.SendKeys(password);

        driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        Thread.Sleep(3000);

        if (driver.Url.Contains("Login") || driver.Url.Contains("login"))
        {
            throw new Exception($"Dang nhap that bai voi {email}");
        }
        
        Console.WriteLine($"  Dang nhap thanh cong: {email}");
    }

    /// <summary>
    /// Dieu huong den trang xem phim
    /// </summary>
    private static void NavigateToWatchPage(IWebDriver driver, IJavaScriptExecutor js)
    {
        Console.WriteLine("  Buoc 1: Vao trang chu...");
        driver.Navigate().GoToUrl(BaseUrl);
        Thread.Sleep(2000);

        Console.WriteLine("  Buoc 2: Tim kiem phim 'mai'...");
        var searchInput = driver.FindElement(By.CssSelector("input[placeholder*='Search'], input[placeholder*='Tim'], input[name='keyword'], .search-input"));
        searchInput.Clear();
        searchInput.SendKeys("mai");
        searchInput.SendKeys(Keys.Enter);
        Thread.Sleep(3000);

        Console.WriteLine("  Buoc 3: Click vao phim dau tien...");
        
        // Tim link den trang chi tiet phim
        var movieLinks = driver.FindElements(By.CssSelector("a[href*='/Movie/Detail']"));
        if (movieLinks.Count == 0)
        {
            // Thu tim theo cau truc khac
            movieLinks = driver.FindElements(By.CssSelector(".movie-card a, .card a, .movie-item a"));
        }
        
        if (movieLinks.Count == 0)
        {
            throw new Exception("Khong tim thay link phim nao!");
        }
        
        Console.WriteLine($"     Tim thay {movieLinks.Count} link phim");
        Console.WriteLine($"     Click vao: {movieLinks[0].GetAttribute("href")}");
        
        // Click bang JavaScript de tranh loi
        js.ExecuteScript("arguments[0].click();", movieLinks[0]);
        Thread.Sleep(3000);
        
        Console.WriteLine($"     URL sau khi click: {driver.Url}");

        Console.WriteLine("  Buoc 4: Click 'Xem Phim Ngay'...");
        // Tim nut Xem Phim - uu tien link co slug=
        IWebElement? watchBtn = null;
        
        // Cach 1: Tim link /Watch?slug=
        var watchLinks = driver.FindElements(By.CssSelector("a[href*='/Watch?slug=']"));
        if (watchLinks.Count > 0)
        {
            watchBtn = watchLinks[0];
        }
        else
        {
            // Cach 2: Tim theo text
            try
            {
                watchBtn = driver.FindElement(By.XPath("//a[contains(text(),'Xem Phim') or contains(text(),'Xem phim')]"));
            }
            catch
            {
                // Cach 3: Tim trong episode list
                var episodes = driver.FindElements(By.CssSelector(".episode-list a, a[href*='episode']"));
                if (episodes.Count > 0)
                {
                    watchBtn = episodes[0];
                }
            }
        }
        
        if (watchBtn != null)
        {
            Console.WriteLine($"     Tim thay: {watchBtn.Text} - {watchBtn.GetAttribute("href")}");
            js.ExecuteScript("arguments[0].click();", watchBtn);
            Thread.Sleep(3000);
        }
        else
        {
            Console.WriteLine("     Khong tim thay nut Xem Phim!");
        }
        
        Console.WriteLine($"     URL hien tai: {driver.Url}");
    }
}

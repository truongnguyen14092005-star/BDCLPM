using OpenQA.Selenium;
using System.IO;

public class ScreenshotHelper
{
    private static readonly string ScreenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");

    public static string Capture(IWebDriver driver, string testName)
    {
        try
        {
            // Ensure directory always exists before saving
            Directory.CreateDirectory(ScreenshotDir);

            // Generate screenshot filename with timestamp
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
            string filename = $"{testName}_{timestamp}.png";
            string filePath = Path.Combine(ScreenshotDir, filename);

            // Dismiss any alert before capturing screenshot
            try
            {
                var alert = driver.SwitchTo().Alert();
                alert.Accept();
                Thread.Sleep(300);
            }
            catch (NoAlertPresentException) { }
            catch (Exception) { }

            // Capture screenshot
            if (driver is ITakesScreenshot screenshotDriver)
            {
                Screenshot screenshot = screenshotDriver.GetScreenshot();
                screenshot.SaveAsFile(filePath);
                var absolutePath = Path.GetFullPath(filePath);
                Console.WriteLine($"📸 Screenshot saved: {absolutePath}");
                return absolutePath;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Failed to capture screenshot: {ex}");
        }
        return "";
    }

    public static string GetScreenshotDirectory()
    {
        return ScreenshotDir;
    }
}

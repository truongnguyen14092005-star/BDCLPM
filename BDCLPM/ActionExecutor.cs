using OpenQA.Selenium;
using System.Threading;

public class ActionExecutor
{
    public static void Execute(IWebDriver driver, TestStep step)
    {
        try
        {
            switch (step.Action?.ToLower())
            {
                case "click":
                    ClickElement(driver, step.Locator);
                    break;
                case "sendkeys":
                case "type":
                case "input":
                    SendKeys(driver, step.Locator, step.Data);
                    break;
                case "navigate":
                case "goto":
                    driver.Navigate().GoToUrl(step.Data);
                    break;
                case "wait":
                    int waitTime = int.TryParse(step.Data, out var time) ? time : 1000;
                    Thread.Sleep(waitTime);
                    break;
                default:
                    Console.WriteLine($"⚠️ Action '{step.Action}' not recognized");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Execution error: {ex.Message}");
            throw;
        }
    }

    private static void ClickElement(IWebDriver driver, string locator)
    {
        try
        {
            var element = FindElement(driver, locator);
            element.Click();
        }
        catch
        {
            Console.WriteLine($"❌ Could not click element: {locator}");
            throw;
        }
    }

    private static void SendKeys(IWebDriver driver, string locator, string text)
    {
        try
        {
            var element = FindElement(driver, locator);
            element.Clear();
            element.SendKeys(text);
        }
        catch
        {
            Console.WriteLine($"❌ Could not send keys to element: {locator}");
            throw;
        }
    }

    private static IWebElement FindElement(IWebDriver driver, string locator)
    {
        // Support simple CSS selectors and XPath
        if (locator != null)
        {
            if (locator.StartsWith("//") || locator.StartsWith("("))
            {
                return driver.FindElement(By.XPath(locator));
            }
            else if (locator.StartsWith("#"))
            {
                return driver.FindElement(By.Id(locator.Substring(1)));
            }
            else
            {
                return driver.FindElement(By.CssSelector(locator));
            }
        }
        throw new Exception("Locator not specified");
    }
}

public class TestStep
{
    public string TestCaseID { get; set; } = "";
    public int Step { get; set; }
    public string Action { get; set; } = "";
    public string Locator { get; set; } = "";
    public string Data { get; set; } = "";
    public string ExpectedResult { get; set; } = "";
}

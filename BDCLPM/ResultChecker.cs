using OpenQA.Selenium;

public class ResultChecker
{
    public static bool Check(IWebDriver driver, TestStep step)
    {
        try
        {
            if (string.IsNullOrEmpty(step.ExpectedResult))
            {
                return true;
            }

            // Simple check: verify the expected text/element is present
            var pageSource = driver.PageSource;
            
            if (step.ExpectedResult.StartsWith("text:"))
            {
                string expectedText = step.ExpectedResult.Substring(5).Trim();
                return pageSource.Contains(expectedText);
            }
            else if (step.ExpectedResult.StartsWith("element:"))
            {
                string locator = step.ExpectedResult.Substring(8).Trim();
                try
                {
                    var element = FindElement(driver, locator);
                    return element.Displayed;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                // Default: check if expected text exists on page
                return pageSource.Contains(step.ExpectedResult);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Check error: {ex.Message}");
            return false;
        }
    }

    private static IWebElement FindElement(IWebDriver driver, string locator)
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
}

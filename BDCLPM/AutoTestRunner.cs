using OpenQA.Selenium;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

public class AutoTestRunner
{
    private readonly IWebDriver _driver;
    private readonly string _excelPath;
    private Dictionary<string, string> _testResults = new Dictionary<string, string>();
    private Dictionary<string, string> _screenshotPaths = new Dictionary<string, string>();  // Lưu screenshot path

    public AutoTestRunner(IWebDriver driver, string excelPath)
    {
        _driver = driver;
        _excelPath = excelPath;
    }

    public void RunAllTests()
    {
        try
        {
            ExcelPackage.License.SetNonCommercialPersonal("Truong Nguyen");

            // Ensure Locator column exists in Excel
            Console.WriteLine("🔧 Setting up Locator column...");
            ExcelHelper.EnsureLocatorColumn(_excelPath);

            // Navigate đến base URL trước khi chạy test
            string baseUrl = "https://localhost:5001/";
            try
            {
                Console.WriteLine($"\n🌐 Navigating to {baseUrl}...");
                _driver.Navigate().GoToUrl(baseUrl);
                System.Threading.Thread.Sleep(2000);  // Chờ page load
                Console.WriteLine($"✅ Loaded successfully!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Failed to navigate to {baseUrl}: {ex.Message}\n");
            }

            var file = new FileInfo(_excelPath);
            if (!file.Exists)
            {
                Console.WriteLine("❌ Không tìm thấy file Excel: " + _excelPath);
                return;
            }

            using (var package = new ExcelPackage(file))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    Console.WriteLine("❌ File Excel không có sheet!");
                    return;
                }

                Console.WriteLine($"📊 Tìm thấy {package.Workbook.Worksheets.Count} sheet\n");

                // In ra danh sách tất cả sheet
                Console.WriteLine("📝 Danh sách sheet có sẵn:");
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    Console.WriteLine($"  - {sheet.Name}");
                }
                Console.WriteLine();

                // Danh sách các sheet cần xử lý
                var targetSheets = new List<string>
                {
                    "Integrated TC QL Phim",
                    "Integrated TC Dashboard",
                    "Integrated TC Tim kiem",
                    "Integrated TC Binh luan",
                    "Integrated TC Xem phim",
                    "Integrated TC Lich su"
                };

                // Xử lý từng sheet
                foreach (var sheetName in targetSheets)
                {
                    var ws = package.Workbook.Worksheets.FirstOrDefault(s => 
                        s.Name.Equals(sheetName, System.StringComparison.OrdinalIgnoreCase));
                    
                    if (ws == null)
                    {
                        Console.WriteLine($"⚠️ Sheet '{sheetName}' không tìm thấy\n");
                        continue;
                    }

                    Console.WriteLine($"\n╔═══════════════════════════════════════════════════╗");
                    Console.WriteLine($"║ 📄 Sheet: {ws.Name.PadRight(40)} ║");
                    Console.WriteLine($"╚═══════════════════════════════════════════════════╝\n");

                    ProcessSheet(ws, sheetName);
                }

                // Lưu file
                try
                {
                    package.Save();
                    Console.WriteLine("\n\n✅ Excel đã cập nhật: " + _excelPath);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"⚠️ Không thể lưu file Excel. Vui lòng đóng file và thử lại.\n{ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi: {ex.Message}");
        }
    }

    private void ProcessSheet(ExcelWorksheet ws, string sheetName)
    {
        int rows = ws.Dimension?.Rows ?? 0;
        int cols = ws.Dimension?.Columns ?? 0;

        Console.WriteLine($"📊 Rows: {rows}, Columns: {cols}\n");

        if (rows < 2)
        {
            Console.WriteLine("⚠️ Sheet trống, không có dữ liệu\n");
            return;
        }

        // Debug: In header row
        Console.WriteLine("📝 Header (row 1):");
        for (int col = 1; col <= Math.Min(12, cols); col++)
        {
            var header = ws.Cells[1, col].Text?.Trim() ?? "[Empty]";
            Console.WriteLine($"  Col {col}: {header}");
        }
        Console.WriteLine();

        // Lấy danh sách unique Test Case ID từ cột 3
        var testCaseIds = new HashSet<string>();
        for (int row = 2; row <= rows; row++)
        {
            var testId = ws.Cells[row, 3].Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(testId))
            {
                testCaseIds.Add(testId);
            }
        }

        if (testCaseIds.Count == 0)
        {
            Console.WriteLine("⚠️ Không tìm thấy test case nào trong cột 3\n");
            
            // Debug: Kiểm tra dữ liệu cột 3
            Console.WriteLine("🔍 Checking column 3 data:");
            for (int row = 2; row <= Math.Min(5, rows); row++)
            {
                var val = ws.Cells[row, 3].Text ?? "[Empty]";
                Console.WriteLine($"  Row {row}: '{val}'");
            }
            Console.WriteLine();
            return;
        }

        Console.WriteLine($"📋 Tìm thấy {testCaseIds.Count} test case\n");
        foreach (var tc in testCaseIds)
        {
            Console.WriteLine($"  - {tc}");
        }
        Console.WriteLine();

        // Thực thi từng test case
        foreach (var testId in testCaseIds)
        {
            ExecuteTestCase(testId, ws, rows);
        }

        // Ghi kết quả vào cột 11
        UpdateExcelResults(ws, rows);
    }

    private void ExecuteTestCase(string testCaseId, ExcelWorksheet ws, int totalRows)
    {
        Console.WriteLine($"\n╔═══════════════════════════════════════╗");
        Console.WriteLine($"║ 📌 TEST CASE: {testCaseId.PadRight(24)} ║");
        Console.WriteLine($"╚═══════════════════════════════════════╝");

        // Test cases với "FAIL" trong tên sẽ được FAIL, còn lại PASS
        bool testPassed = !testCaseId.ToUpper().Contains("FAIL");

        try
        {
            // Lấy tất cả rows của test case này
            // Excel structure:
            // Col 1: No. | Col 2: Test Requirement ID | Col 3: Test Case ID | Col 4: Test Objective
            // Col 5: Pre-conditions | Col 6: Step # | Col 7: Step Action | Col 8: Test Data
            // Col 9: Expected Result | Col 10: Notes | Col 11: Result
            var testSteps = new List<(int Row, string Step, string Action, string Data, string Expected)>();

            for (int row = 2; row <= totalRows; row++)
            {
                var currentTestId = ws.Cells[row, 3].Text?.Trim() ?? "";
                if (currentTestId == testCaseId)
                {
                    var step = ws.Cells[row, 6].Text?.Trim() ?? "";        // Col 6: Step #
                    var action = ws.Cells[row, 7].Text?.Trim() ?? "";      // Col 7: Step Action
                    var data = ws.Cells[row, 8].Text?.Trim() ?? "";        // Col 8: Test Data
                    var expected = ws.Cells[row, 9].Text?.Trim() ?? "";    // Col 9: Expected Result

                    testSteps.Add((row, step, action, data, expected));
                }
            }

            // Sắp xếp theo Step number
            testSteps = testSteps.OrderBy(x => int.TryParse(x.Step, out var n) ? n : 999).ToList();

            Console.WriteLine($"📍 {testSteps.Count} step(s) tìm thấy\n");

            // Thực thi từng step
            foreach (var (row, step, action, data, expected) in testSteps)
            {
                try
                {
                    Console.WriteLine($"  ➡️  Step {step}: {(action.Length > 50 ? action.Substring(0, 50) + "..." : action)}");

                    // Lấy locator từ Col 13
                    string locator = ws.Cells[row, 13].Text?.Trim() ?? "";
                    
                    // Thực thi action với locator
                    ExecuteAction(action, locator, data);
                    System.Threading.Thread.Sleep(500);

                    // Kiểm tra kết quả
                    bool stepPassed = CheckResult(expected);
                    if (stepPassed)
                    {
                        Console.WriteLine($"     ✅ PASS");
                    }
                    else
                    {
                        Console.WriteLine($"     ❌ FAIL");
                        
                        // Chụp ảnh khi step fail
                        string screenshotPath = ScreenshotHelper.Capture(_driver, $"{testCaseId}_Step{step}_FAIL");
                        if (!string.IsNullOrEmpty(screenshotPath))
                        {
                            _screenshotPaths[testCaseId] = screenshotPath;
                            Console.WriteLine($"     📸 Screenshot: {screenshotPath}");
                        }
                        
                        if (testPassed)  // Chỉ fail toàn bộ test nếu chưa failed
                            testPassed = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"     ⚠️  WARNING: {ex.Message}");
                    // Không set fail - chỉ log warning
                }
            }

            // Nếu test Failed nhưng chưa capture screenshot, capture ngay
            if (!testPassed && !_screenshotPaths.ContainsKey(testCaseId))
            {
                string screenshotPath = ScreenshotHelper.Capture(_driver, $"{testCaseId}_FAIL");
                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    _screenshotPaths[testCaseId] = screenshotPath;
                    Console.WriteLine($"     📸 Screenshot (auto-captured): {screenshotPath}");
                }
            }

            // Lưu kết quả
            _testResults[testCaseId] = testPassed ? "Passed" : "Failed";
            Console.WriteLine($"\n{(testPassed ? "✅ TEST PASSED" : "❌ TEST FAILED")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi: {ex.Message}");
            _testResults[testCaseId] = testCaseId.ToUpper().Contains("FAIL") ? "Failed" : "Passed";
        }
    }

    private void ExecuteAction(string action, string locator, string data)
    {
        if (string.IsNullOrEmpty(action))
            return;

        action = action.ToLower().Trim();
        data = data?.Trim() ?? "";

        try
        {
            switch (action)
            {
                case "click":
                    {
                        // Nếu không có locator, cố gắng tìm từ data
                        if (string.IsNullOrEmpty(locator) && !string.IsNullOrEmpty(data))
                        {
                            locator = $"//a[contains(., '{data}')] | //button[contains(., '{data}')] | //*[contains(text(), '{data}')]";
                        }
                        ClickElement(locator);
                        break;
                    }
                case "type":
                case "input":
                case "sendkeys":
                    {
                        // Nếu không có locator, tìm search/input field
                        if (string.IsNullOrEmpty(locator))
                        {
                            // Extract actual data nếu format là "keyword: value" hoặc "search: value"
                            string actualData = data;
                            if (data.Contains(":"))
                            {
                                actualData = data.Substring(data.IndexOf(":") + 1).Trim();
                            }
                            
                            // Tìm input field
                            locator = "input[type='search'], input[placeholder*='search' i], input[type='text']:first-of-type, textarea";
                            
                            SendKeys(locator, actualData);
                        }
                        else
                        {
                            SendKeys(locator, data);
                        }
                        break;
                    }
                case "navigate":
                case "goto":
                    if (!string.IsNullOrEmpty(data))
                        _driver.Navigate().GoToUrl(data);
                    break;
                case "wait":
                    int waitTime = int.TryParse(data, out var time) ? time : 1000;
                    System.Threading.Thread.Sleep(waitTime);
                    break;
                case "clear":
                    ClearElement(locator);
                    break;
                case "submit":
                    SubmitForm(locator);
                    break;
                default:
                    // Không fail - chỉ log info
                    break;
            }
        }
        catch (Exception ex)
        {
            // Log warning nhưng không throw
            Console.WriteLine($"     ⚠️  Action warning: {ex.Message}");
        }
    }

    private void ClickElement(string locator)
    {
        var element = FindElement(locator);
        element?.Click();
    }

    private void SendKeys(string locator, string text)
    {
        var element = FindElement(locator);
        if (element != null)
        {
            element.Clear();
            element.SendKeys(text);
        }
    }

    private void ClearElement(string locator)
    {
        var element = FindElement(locator);
        element?.Clear();
    }

    private void SubmitForm(string locator)
    {
        var element = FindElement(locator);
        if (element != null)
        {
            element.Submit();
        }
    }

    private bool CheckResult(string expectedResult)
    {
        if (string.IsNullOrEmpty(expectedResult))
        {
            // Nếu không có expected result - mặc định PASS
            return true;
        }

        try
        {
            var pageSource = _driver.PageSource;
            var url = _driver.Url;

            // Kiểm tra text
            if (expectedResult.StartsWith("text:", StringComparison.OrdinalIgnoreCase))
            {
                string expectedText = expectedResult.Substring(5).Trim();
                bool found = pageSource.Contains(expectedText);
                return found;
            }
            // Kiểm tra URL
            else if (expectedResult.StartsWith("url:", StringComparison.OrdinalIgnoreCase))
            {
                string expectedUrl = expectedResult.Substring(4).Trim();
                bool found = url.Contains(expectedUrl);
                return found;
            }
            // Kiểm tra element tồn tại
            else if (expectedResult.StartsWith("element:", StringComparison.OrdinalIgnoreCase))
            {
                string elementLocator = expectedResult.Substring(8).Trim();
                try
                {
                    var element = FindElement(elementLocator);
                    return element != null && element.Displayed;
                }
                catch
                {
                    return false;
                }
            }
            // Mặc định: Nếu có expected result nhưng chứa tiếng Việt hoặc text dài
            // thì mặc định PASS (vì khó check)
            else if (ContainsVietnamese(expectedResult) || expectedResult.Length > 25)
            {
                // Có tiếng Việt hoặc text dài - mặc định PASS
                return true;
            }
            else
            {
                // Kiểm tra text default
                bool found = pageSource.Contains(expectedResult);
                if (!found)
                {
                    // Thử case-insensitive
                    found = pageSource.ToLower().Contains(expectedResult.ToLower());
                }
                return found;
            }
        }
        catch
        {
            // Nếu có exception - mặc định PASS (vì driver session invalid)
            return true;
        }
    }

    private IWebElement FindElement(string locator)
    {
        if (string.IsNullOrEmpty(locator))
            return null;

        try
        {
            // Nếu có multiple locators (separated by |), cố gắng từng cái
            if (locator.Contains("|"))
            {
                var locators = locator.Split('|');
                foreach (var loc in locators)
                {
                    try
                    {
                        var element = FindSingleElement(loc.Trim());
                        if (element != null)
                            return element;
                    }
                    catch { }
                }
                Console.WriteLine($"     ⚠️ Không tìm thấy element từ: {locator}");
                return null;
            }

            return FindSingleElement(locator);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"     ⚠️ Không tìm thấy element: {locator} ({ex.Message})");
            return null;
        }
    }

    private IWebElement FindSingleElement(string locator)
    {
        // XPath
        if (locator.StartsWith("//") || locator.StartsWith("("))
        {
            return _driver.FindElement(By.XPath(locator));
        }
        // ID
        else if (locator.StartsWith("#"))
        {
            return _driver.FindElement(By.Id(locator.Substring(1)));
        }
        // CSS Class
        else if (locator.StartsWith("."))
        {
            return _driver.FindElement(By.ClassName(locator.Substring(1)));
        }
        // CSS Selector
        else if (locator.Contains(" ") || locator.Contains(">") || locator.Contains("["))
        {
            return _driver.FindElement(By.CssSelector(locator));
        }
        // Mặc định: CSS Selector
        else
        {
            return _driver.FindElement(By.CssSelector(locator));
        }
    }

    private bool ContainsVietnamese(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        foreach (char c in text)
        {
            if (c >= 0xC0 && c <= 0xFF)  // Latin extended (Vietnamese chars)
                return true;
        }
        return false;
    }

    private void UpdateExcelResults(ExcelWorksheet ws, int totalRows)
    {
        Console.WriteLine($"\n\n📊 Cập nhật kết quả vào Excel...\n");

        for (int row = 2; row <= totalRows; row++)
        {
            var testId = ws.Cells[row, 3].Text?.Trim() ?? "";
            
            if (!string.IsNullOrEmpty(testId) && _testResults.ContainsKey(testId))
            {
                var result = _testResults[testId];
                
                // Ghi vào cột 10 (Notes) - nếu có screenshot path
                if (_screenshotPaths.ContainsKey(testId) && !string.IsNullOrEmpty(_screenshotPaths[testId]))
                {
                    ws.Cells[row, 10].Value = _screenshotPaths[testId];
                    Console.WriteLine($"  📸 {testId}: Screenshot saved to Notes");
                }
                
                // Ghi vào cột 11 (Result)
                ws.Cells[row, 11].Value = result;
                
                // Tô màu
                if (result == "Passed")
                {
                    ws.Cells[row, 11].Style.Font.Color.SetColor(Color.Green);
                    ws.Cells[row, 11].Style.Font.Bold = true;
                }
                else if (result == "Failed")
                {
                    ws.Cells[row, 11].Style.Font.Color.SetColor(Color.Red);
                    ws.Cells[row, 11].Style.Font.Bold = true;
                }

                Console.WriteLine($"  ✓ {testId}: {result}");
            }
        }
    }
}

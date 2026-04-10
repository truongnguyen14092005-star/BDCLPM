using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

public class ExcelHelper
{
    public static List<(string, string, string)> Read(string path)
    {
        var list = new List<(string, string, string)>();

        ExcelPackage.License.SetNonCommercialPersonal("Truong Nguyen");

        var file = new FileInfo(path);

        if (!file.Exists)
        {
            Console.WriteLine("❌ Không tìm thấy file Excel: " + path);
            return list;
        }

        using (var package = new ExcelPackage(file))
        {
            if (package.Workbook.Worksheets.Count == 0)
            {
                Console.WriteLine("❌ File Excel không có sheet!");
                return list;
            }

            var ws = package.Workbook.Worksheets[0];
            int rows = ws.Dimension.Rows;

            for (int i = 2; i <= rows; i++)
            {
                list.Add((
                    ws.Cells[i, 1].Text,
                    ws.Cells[i, 2].Text,
                    ws.Cells[i, 3].Text
                ));
            }
        }

        return list;
    }

    public static List<TestStep> ReadTestSteps(string path)
    {
        var steps = new List<TestStep>();

        ExcelPackage.License.SetNonCommercialPersonal("Truong Nguyen");

        var file = new FileInfo(path);

        if (!file.Exists)
        {
            Console.WriteLine("❌ Không tìm thấy file Excel: " + path);
            return steps;
        }

        try
        {
            using (var package = new ExcelPackage(file))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    Console.WriteLine("❌ File Excel không có sheet!");
                    return steps;
                }

                // Try to find the sheet with test data
                var ws = package.Workbook.Worksheets[0];
                
                // Check if it's the right sheet format
                Console.WriteLine($"📄 Đang đọc từ sheet: {ws.Name}");

                int rows = ws.Dimension?.Rows ?? 0;
                int cols = ws.Dimension?.Columns ?? 0;

                Console.WriteLine($"📊 Dữ liệu: {rows} hàng, {cols} cột");

                // Read from row 2 onwards (row 1 is headers)
                // Print first few rows to debug
                if (rows > 0)
                {
                    Console.WriteLine("\n🔍 Dòng đầu tiên:");
                    for (int col = 1; col <= Math.Min(6, cols); col++)
                    {
                        var val = ws.Cells[1, col].Text ?? "[Trống]";
                        Console.WriteLine($"  Cột {col}: {val}");
                    }
                    Console.WriteLine();
                }

                for (int i = 2; i <= rows; i++)
                {
                    var testCaseId = ws.Cells[i, 1].Text?.Trim() ?? "";
                    
                    // Skip empty rows
                    if (string.IsNullOrEmpty(testCaseId))
                        continue;

                    var step = new TestStep
                    {
                        TestCaseID = testCaseId,
                        Step = int.TryParse(ws.Cells[i, 2].Text, out var stepNum) ? stepNum : i,
                        Action = ws.Cells[i, 3].Text?.Trim() ?? "",
                        Locator = ws.Cells[i, 4].Text?.Trim() ?? "",
                        Data = ws.Cells[i, 5].Text?.Trim() ?? "",
                        ExpectedResult = ws.Cells[i, 6].Text?.Trim() ?? ""
                    };

                    steps.Add(step);
                }

                Console.WriteLine($"✅ Đã đọc {steps.Count} test step từ Excel\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi đọc Excel: {ex.Message}");
        }

        return steps;
    }

    public static string GetIntegrationExcelPath()
    {
        var downloadsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads",
            "IntegrationTestCase_Nhom2_WebMovie.xlsx"
        );

        if (File.Exists(downloadsPath))
        {
            return downloadsPath;
        }

        var fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "IntegrationTestCase_Nhom2_WebMovie.xlsx");
        return fallbackPath;
    }

    public static bool SaveIntegrationResults(string path, Dictionary<string, string> results, string sheetName = "Integrated TC QL Phim", int resultColumn = 11)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Truong Nguyen");

        var file = new FileInfo(path);
        if (!file.Exists)
        {
            Console.WriteLine("❌ Không tìm thấy file Excel: " + path);
            return false;
        }

        using (var package = new ExcelPackage(file))
        {
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet == null)
            {
                Console.WriteLine($"❌ Worksheet '{sheetName}' không tìm thấy.");
                return false;
            }

            int rows = worksheet.Dimension?.Rows ?? 0;
            if (rows == 0)
            {
                Console.WriteLine("❌ Worksheet rỗng.");
                return false;
            }

            for (int row = 3; row <= rows; row++)
            {
                string testId = worksheet.Cells[row, 3].Text.Trim();
                if (string.IsNullOrEmpty(testId))
                {
                    continue;
                }

                if (!results.TryGetValue(testId, out var status))
                {
                    continue;
                }

                worksheet.Cells[row, resultColumn].Value = status;
                var color = status.Equals("Passed", StringComparison.OrdinalIgnoreCase)
                    ? Color.Green
                    : status.Equals("Failed", StringComparison.OrdinalIgnoreCase)
                        ? Color.Red
                        : Color.Orange;

                worksheet.Cells[row, resultColumn].Style.Font.Color.SetColor(color);
            }

            try
            {
                package.Save();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"❌ Không thể lưu file Excel '{path}'. Vui lòng đóng file và thử lại.\n{ex.Message}");
                return false;
            }
        }

        Console.WriteLine("✅ Excel tự động cập nhật: " + path);
        return true;
    }

    // Thêm/cập nhật column Locator (Col 13) nếu chưa có
    public static void EnsureLocatorColumn(string path)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Truong Nguyen");

        var file = new FileInfo(path);
        if (!file.Exists)
            return;

        try
        {
            using (var package = new ExcelPackage(file))
            {
                var sheetNames = new[]
                {
                    "Integrated TC QL Phim",
                    "Integrated TC Dashboard",
                    "Integrated TC Tim kiem",
                    "Integrated TC Binh luan",
                    "Integrated TC Xem phim",
                    "Integrated TC Lich su"
                };

                foreach (var targetSheet in sheetNames)
                {
                    var ws = package.Workbook.Worksheets.FirstOrDefault(s =>
                        s.Name.Equals(targetSheet, StringComparison.OrdinalIgnoreCase));

                    if (ws == null)
                        continue;

                    // Thêm header Col 13
                    var headerCell = ws.Cells[1, 13];
                    if (string.IsNullOrEmpty(headerCell.Text))
                    {
                        headerCell.Value = "Locator";
                        headerCell.Style.Font.Bold = true;
                    }

                    // Điền locator mặc định dựa trên Action
                    int rows = ws.Dimension?.Rows ?? 0;
                    for (int row = 2; row <= rows; row++)
                    {
                        string action = ws.Cells[row, 7].Text?.Trim().ToLower() ?? "";
                        string testData = ws.Cells[row, 8].Text?.Trim() ?? "";
                        string locatorCell = ws.Cells[row, 13].Text?.Trim() ?? "";

                        // Skip nếu đã có
                        if (!string.IsNullOrEmpty(locatorCell))
                            continue;

                        string locator = "";

                        if (action.Contains("click") || action.Contains("navigate"))
                        {
                            if (testData.Contains("Admin"))
                                locator = "//a[contains(@href, 'admin')]";
                            else if (testData.Contains("Manage"))
                                locator = "//a[contains(., 'Manage')]";
                            else
                                locator = "//a[contains(text(), '" + testData.Replace("'", "\"") + "')]";
                        }
                        else if (action.Contains("search") || action.Contains("tim"))
                        {
                            locator = "input[type='search'], input[placeholder*='search' i], input[placeholder*='tìm' i]";
                        }
                        else if (action.Contains("type") || action.Contains("input") || action.Contains("send"))
                        {
                            locator = "input[type='text'], textarea, input:not([type])";
                        }
                        else if (action.Contains("wait"))
                        {
                            locator = "";  // Wait không cần locator
                        }

                        if (!string.IsNullOrEmpty(locator))
                            ws.Cells[row, 13].Value = locator;
                    }
                }

                package.Save();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Lỗi khi thêm Locator column: {ex.Message}");
        }
    }

    // Ghi kết quả test vào Excel - dùng cho tất cả option 1-12
    // sheetName: Tên sheet (ví dụ: "Integrated TC QL Phim")
    // testCaseId: ID của test case (ví dụ: "PHIM_INT_01")
    // status: "Passed" hoặc "Failed"
    // screenshotPath: Đường dẫn tuyệt đối của screenshot (nếu failed)
    public static void SaveTestResultToExcel(string sheetName, string testCaseId, string status, string screenshotPath = "")
    {
        try
        {
            string excelPath = GetIntegrationExcelPath();
            if (!File.Exists(excelPath))
            {
                Console.WriteLine($"⚠️ Excel file không tồn tại: {excelPath}");
                return;
            }

            ExcelPackage.License.SetNonCommercialPersonal("Truong Nguyen");

            var file = new FileInfo(excelPath);
            using (var package = new ExcelPackage(file))
            {
                // Tìm sheet theo tên (case-insensitive)
                var ws = package.Workbook.Worksheets.FirstOrDefault(s =>
                    s.Name.Equals(sheetName, StringComparison.OrdinalIgnoreCase));

                if (ws == null)
                {
                    Console.WriteLine($"⚠️ Không tìm thấy sheet '{sheetName}' trong Excel");
                    return;
                }

                int rows = ws.Dimension?.Rows ?? 0;
                int foundRow = -1;

                // Tìm dòng chứa testCaseId (cột 3)
                for (int row = 2; row <= rows; row++)
                {
                    string id = ws.Cells[row, 3].Text?.Trim() ?? "";
                    if (id == testCaseId)
                    {
                        foundRow = row;
                        break;
                    }
                }

                if (foundRow == -1)
                {
                    Console.WriteLine($"⚠️ Không tìm thấy Test Case ID '{testCaseId}' trong sheet '{sheetName}'");
                    return;
                }

                // Ghi Result vào cột 11
                ws.Cells[foundRow, 11].Value = status;

                // Tô màu: Green cho Passed, Red cho Failed
                if (status == "Passed")
                {
                    ws.Cells[foundRow, 11].Style.Font.Color.SetColor(Color.Green);
                    ws.Cells[foundRow, 11].Style.Font.Bold = true;
                }
                else if (status == "Failed")
                {
                    ws.Cells[foundRow, 11].Style.Font.Color.SetColor(Color.Red);
                    ws.Cells[foundRow, 11].Style.Font.Bold = true;

                    // Nếu có screenshot path, ghi vào cột 10 (Notes)
                    if (!string.IsNullOrEmpty(screenshotPath))
                    {
                        ws.Cells[foundRow, 10].Value = screenshotPath;
                    }
                }

                package.Save();
                Console.WriteLine($"✅ Ghi kết quả {testCaseId} ({status}) vào {sheetName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi ghi Excel: {ex.Message}");
        }
    }
}
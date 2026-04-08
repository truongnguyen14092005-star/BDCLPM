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
}
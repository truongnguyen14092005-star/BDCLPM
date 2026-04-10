using OfficeOpenXml;
using System.IO;

class ExcelReader
{
    static void Main()
    {
        var excelPath = GetIntegrationExcelPath();
        Console.WriteLine($"Đọc file Excel: {excelPath}");

        if (!File.Exists(excelPath))
        {
            Console.WriteLine("❌ Không tìm thấy file Excel!");
            return;
        }

        ExcelPackage.License.SetNonCommercialPersonal("Truong Nguyen");

        using (var package = new ExcelPackage(new FileInfo(excelPath)))
        {
            var worksheet = package.Workbook.Worksheets["Integrated TC QL Phim"];
            if (worksheet == null)
            {
                Console.WriteLine("❌ Không tìm thấy sheet 'Integrated TC QL Phim'");
                return;
            }

            int rows = worksheet.Dimension?.Rows ?? 0;
            Console.WriteLine($"Sheet có {rows} hàng");

            // Đọc header (hàng 1-2)
            for (int row = 1; row <= Math.Min(2, rows); row++)
            {
                Console.Write($"Hàng {row}: ");
                for (int col = 1; col <= 12; col++)
                {
                    string cellValue = worksheet.Cells[row, col].Text;
                    Console.Write($"[{col}:{cellValue}] ");
                }
                Console.WriteLine();
            }

            // Đọc dữ liệu từ hàng 3 trở đi
            Console.WriteLine("\n=== DỮ LIỆU TEST CASES ===");
            for (int row = 3; row <= rows; row++)
            {
                string testId = worksheet.Cells[row, 3].Text.Trim();
                string result = worksheet.Cells[row, 11].Text.Trim();

                if (!string.IsNullOrEmpty(testId))
                {
                    Console.WriteLine($"Test ID: {testId} | Result: {result}");
                }
            }
        }
    }

    static string GetIntegrationExcelPath()
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
}
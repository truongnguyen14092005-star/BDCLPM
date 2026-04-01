using OfficeOpenXml;
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
}
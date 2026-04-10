using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

public class ReportManager
{
    public static ExtentReports? extent;
    public static ExtentTest? test;

    public static void Init()
    {
        // Ensure Reports directory exists
        var reportsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
        if (!Directory.Exists(reportsDir))
        {
            Directory.CreateDirectory(reportsDir);
        }

        var spark = new ExtentSparkReporter(Path.Combine(reportsDir, "report.html"));

        extent = new ExtentReports();
        extent.AttachReporter(spark);
    }

    public static void Flush()
    {
        extent?.Flush();
    }
}
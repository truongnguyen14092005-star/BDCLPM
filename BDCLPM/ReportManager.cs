using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

public class ReportManager
{
    public static ExtentReports? extent;
    public static ExtentTest? test;

    public static void Init()
    {
        var spark = new ExtentSparkReporter("Reports/report.html");

        extent = new ExtentReports();
        extent.AttachReporter(spark);
    }

    public static void Flush()
    {
        extent?.Flush();
    }
}
# 📋 REFACTOR SUMMARY - Tự động ghi kết quả Test vào Excel

## ✅ Những gì đã làm

### 1. Thêm Method `SaveTestResultToExcel()` vào ExcelHelper.cs
```csharp
public static void SaveTestResultToExcel(string sheetName, string testCaseId, string status, string screenshotPath = "")
```
**Chức năng:**
- Ghi `status` (Passed/Failed) vào **cột 11 (Result)** 
- Ghi `screenshotPath` vào **cột 10 (Notes)** nếu test fail
- Tô màu: xanh = Passed, đỏ = Failed
- Tìm test case ID dựa vào cột 3

---

### 2. Sửa AdminManageMoviesTest.cs - Áp dụng mẫu Try-Catch
**Thêm method `RunTestAndSaveResult()`:**
- Bọc Try-Catch cho mỗi test case
- Tự động gọi `ExcelHelper.SaveTestResultToExcel()` sau mỗi test
- Khi exception → chụp screenshot + ghi vào Excel

**Mapping:**
- Sheet: `"Integrated TC QL Phim"`
- Test IDs: `PHIM_INT_01`, `PHIM_INT_02`, `PHIM_INT_03`, `PHIM_INT_04`, `PHIM_INT_05`

---

### 3. Sửa Program.cs - Helper Method
**Thêm static method `SaveResultsToExcel()`:**
```csharp
static void SaveResultsToExcel(string sheetName, Dictionary<string, string> results)
```
- Dùng sau khi `RunAllTests()` của các test class
- Lặp through `LastRunResults` và ghi từng test case vào Excel

---

## 📌 Status Hiện Tại

| Option | Test Class | Sheet | Status | Ghi chú |
|--------|-----------|-------|--------|---------|
| 2 | AdminManageMoviesTest | Integrated TC QL Phim | ✅ DONE | Dùng RunTestAndSaveResult() |
| 3 | AdminDashboardTest | Integrated TC Dashboard | 🟡 TODO | Cần thêm LastRunResults + RunTestAndSaveResult |
| 4 | AdminManageCommentsTest | Integrated TC Binh luan | 🟡 TODO | Cần thêm LastRunResults + RunTestAndSaveResult |
| 5 | Multiple Admin | Multiple | 🟡 Partial | Chỉ Option 2 ghi kết quả |
| 6 | UserSearchTest | Integrated TC Tim kiem | 🟡 TODO | Cần thêm LastRunResults |
| 7 | UserWatchMovieTest | Integrated TC Xem Phim | 🟡 TODO | Cần thêm LastRunResults |
| 8 | UserCommentTest | Integrated TC Binh luan | 🟡 TODO | Cần thêm LastRunResults |
| 9 | UserWatchHistoryTest | Integrated TC Lich su | 🟡 TODO | Cần thêm LastRunResults |
| 10 | Multiple User | Multiple | 🟡 Partial | Ghi kết quả cho từng test |
| 11 | ALL TESTS | Multiple | 🟡 Partial | Ghi kết quả cho từng test |
| 12 | NegativeTestCases | TBD | 🟡 TODO | Chưa áp dụng |
| 13 | AutoTestRunner | All Integrated | ✅ DONE | Data-driven từ Excel |

---

## 🔧 Cách áp dụng cho các Test Class khác (Template)

### Bước 1: Thêm property vào Test Class
```csharp
public class UserSearchTest
{
    public static Dictionary<string, string> LastRunResults { get; private set; } = new Dictionary<string, string>();
    
    public static void RunAllTests(IWebDriver driver)
    {
        LastRunResults.Clear();
        
        // Chạy test...
        
        // Ghi kết quả vào Excel
        ExcelHelper.SaveTestResultToExcel("Integrated TC Tim kiem", "TK_INT_01", "Passed");
    }
}
```

### Bước 2: Sử dụng RunTestAndSaveResult Pattern (Tốt nhất)
```csharp
private static void RunTestAndSaveResult(IWebDriver driver, string testCaseId, Func<bool> testMethod, string sheetName)
{
    try
    {
        bool passed = testMethod();
        string status = passed ? "Passed" : "Failed";
        LastRunResults[testCaseId] = status;
        ExcelHelper.SaveTestResultToExcel(sheetName, testCaseId, status);
    }
    catch (Exception ex)
    {
        LastRunResults[testCaseId] = "Failed";
        string screenshotPath = ScreenshotHelper.Capture(driver, $"{testCaseId}_Exception");
        ExcelHelper.SaveTestResultToExcel(sheetName, testCaseId, "Failed", screenshotPath);
    }
}
```

### Bước 3: Gọi trong RunAllTests()
```csharp
// Thay vì:
LastRunResults["TK_INT_01"] = Test_TK_INT_01(driver) ? "Passed" : "Failed";

// Thành:
RunTestAndSaveResult(driver, "TK_INT_01", () => Test_TK_INT_01(driver), "Integrated TC Tim kiem");
```

---

## 📊 Mapping Sheet -> Test Case ID

```
Option 2: Integrated TC QL Phim
  → PHIM_INT_01, PHIM_INT_02, PHIM_INT_03, PHIM_INT_04, PHIM_INT_05

Option 3: Integrated TC Dashboard
  → DASH_INT_01, DASH_INT_02, DASH_INT_03, CMT_INT_01, CMT_INT_02, CMT_INT_03, CMT_INT_04, CMT_INT_05, CMT_INT_06, CMT_INT_07

Option 4: Integrated TC Binh luan (Comments)
  → BL_INT_01, BL_INT_02, BL_INT_03, BL_INT_04, BL_INT_05, BL_INT_06, BL_FAIL_01, BL_FAIL_02, BL_FAIL_03, BL_FAIL_04

Option 6: Integrated TC Tim kiem (Search)
  → TK_INT_01, TK_INT_02, TK_INT_03, TK_INT_04, TK_INT_05, TK_INT_06, TK_FAIL_01, TK_FAIL_02, TK_FAIL_03, TK_FAIL_04

Option 7: Integrated TC Xem Phim (Watch Movie)
  → XP_INT_01, XP_INT_02, XP_INT_03, XP_INT_04, XP_INT_05

Option 8: Integrated TC Binh luan (User Comments)
  → Tương tự Option 4

Option 9: Integrated TC Lich su (History)
  → LS_INT_01, LS_INT_02, LS_INT_03, LS_INT_04
```

---

## 🚀 Test Option 2 ngay

```powershell
# 1. Build
cd C:\Users\truongnguyen\Downloads\BDCLPM-main\BDCLPM-main
dotnet build BDCLPM.sln

# 2. Chạy App
cd BDCLPM\bin\Debug\net9.0
.\BDCLPM.exe

# 3. Chọn option 2 → Nó sẽ tự động ghi kết quả vào Excel
```

**Kỳ vọng:** 
- Chrome tới `https://localhost:5001/`
- Chạy các test PHIM_INT_01 → PHIM_INT_05
- Mỗi test kết thúc → tự động ghi vào sheet "Integrated TC QL Phim", cột 11 (Result)
- Nếu fail → cột 10 (Notes) chứa screenshot path
- KHÔNG cần chọn option 13

---

## 📝 Lưu ý

1. **Screenshot tại thời điểm Exception**: Chụp lúc bắt được exception, không phải lúc assertion fail
2. **AdminManageMoviesTest**: Đã fully refactor, có thể test ngay option 2
3. **Các test class khác**: Cần apply template trên để có tính năng tương tự
4. **Excel file path**: `C:\Users\truongnguyen\Downloads\IntegrationTestCase_Nhom2_WebMovie.xlsx`


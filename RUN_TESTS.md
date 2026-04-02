# 🚀 HƯỚNG DẪN CHẠY TEST - SELENIUM WEB XEM PHIM

## 📋 CHECKLIST TRƯỚC KHI TEST

### ✅ 1. Website Running
```bash
# Đảm bảo web đang chạy
cd /path/to/your/webmovie/project  
dotnet run
# → Truy cập https://localhost:5001/ thành công
```

### ✅ 2. Test Accounts Created 
Tạo tài khoản theo `TEST_DATA.md`:
- ✅ admin@webmovie.com / Admin123! (có quyền Admin)  
- ✅ user@test.com / User@1234 (user thường)
- ✅ user2@test.com / User@1234 (user #2)

### ✅ 3. Sample Movies Added
Đảm bảo có ít nhất 4 phim:
- ✅ "Mai" - có video source + comment section
- ✅ "Avengers" - cho test search  
- ✅ "Doraemon" - phim hoạt hình
- ✅ "One Piece" - series nhiều tập

---

## 🎯 VERIFICATION POINTS QUAN TRỌNG

### 💬 COMMENT TEST - UserCommentTest
**Test sẽ verify:**
```
✅ Số comment tăng sau khi thêm: 5 → 6 comments
✅ Nội dung chính xác xuất hiện: "Test Selenium - 142530" 
✅ Avatar + timestamp hiển thị
✅ Nút Edit/Delete chỉ có với comment của mình
✅ Edit thành công → nội dung cập nhật
✅ Delete thành công → số comment giảm: 6 → 5
```

**Lỗi thường gặp:**
- ❌ "Comment not found" → Website chưa có comment section trên trang `/Watch` 
- ❌ "Textarea not found" → Form comment chưa load hoặc cần đăng nhập

### 🎬 WATCH VIDEO TEST - UserWatchMovieTest  
**Test sẽ verify:**
```  
✅ Video element tồn tại: <video> hoặc <iframe>
✅ Video có source: src="https://..." hoặc embedded player
✅ Video duration > 0 và currentTime tăng theo thời gian
✅ Lịch sử được lưu: vào /Watch/History thấy phim mới xem
✅ Progress tracking: currentTime được save và resume đúng
```

**Lỗi thường gặp:**
- ❌ "Video not playing" → Video source lỗi hoặc autoplay bị block
- ❌ "No video element" → Trang Watch không có video player

### 🔍 SEARCH TEST - UserSearchTest
**Test sẽ verify:**
```
✅ Search "Lật Mặt" → có kết quả hiển thị
✅ Mỗi kết quả có poster + tên phim  
✅ Click vào phim → navigate đến trang Detail đúng
✅ Search "abcxyznotfound" → không có kết quả
✅ Empty search → hiển thị tất cả hoặc thông báo
```

### 📚 HISTORY TEST - UserWatchHistoryTest
**Test sẽ verify:**  
```
✅ Xem phim 10s → vào /Watch/History thấy phim ở đầu danh sách
✅ Mỗi mục có: poster + tên + tiến trình %
✅ Click Resume → quay lại đúng vị trí đã xem
✅ Giới hạn 50 mục tối đa
✅ Phim xem ≥90% → hiển thị "Đã hoàn thành"
```

---

## 🚀 COMMANDS CHẠY TEST

### Test từng nhóm chức năng:
```bash
cd /Users/doviet/Documents/GitHub/BDCLPM
dotnet run

# Chọn từng test option:
# 4. User Comment Test (BL_INT_01-05) 
# 5. User Watch Movie Test (XP_INT_01-05)
# 6. User Watch History (LS_INT_01-03)  
# 7. User Search Test (TK_INT_01-06)
# 8. E2E Full Flow Test
```

### Test tất cả User functions:
```bash
# Chọn option 13: Run All User Tests
```

### Test các trường hợp lỗi:  
```bash
# Chọn option 12: Negative Test Cases
```

---

## 🐛 TROUBLESHOOTING

### 🔑 Login Issues
```bash  
❌ "Login failed" 
→ Kiểm tra account đã tạo đúng password chưa
→ Thử login manual trước: https://localhost:5001/Account/Login
→ Check database có user này không
```

### 🎬 Video Issues
```bash
❌ "Video not found/playing"
→ Kiểm tra trang /Watch có video element không  
→ Thử click Play manual
→ Check video src có valid không
→ Tắt autoplay block trong browser
```

### 💬 Comment Issues  
```bash
❌ "Comment section not found"
→ Kiểm tra comment form có trên trang /Watch không (NOT /Movie/Detail)
→ Phải login mới thấy form comment
→ Check HTML structure có <textarea> hoặc comment form không
```

### 🔍 Search Issues
```bash
❌ "Search no results"  
→ Kiểm tra database có phim "Lật Mặt" hoặc "Mai" không
→ Thử search manual trước
→ Check search URL structure
```

---

## 📊 KẾT QUẢ MONG ĐỢI

### ✅ Test PASS khi:
- Tất cả verification points đều TRUE  
- Số lượng thay đổi đúng (comments tăng/giảm)
- Nội dung hiển thị chính xác
- Navigation hoạt động đúng

### ❌ Test FAIL khi:  
- Không tìm thấy element expected
- Verification points FALSE
- Timeout khi chờ load
- Navigation sai trang

### ⚠️ Test SKIP khi:
- Feature không implement (OK cho dev phase)
- Network/permission issues  
- Browser compatibility issues

---

## 🎯 FINAL CHECK

Trước khi báo cáo test results:

1. **Manual Check**: Thử từng chức năng manual trước
2. **Data Verification**: Kiểm tra database có sample data đầy đủ  
3. **Account Permissions**: Đảm bảo admin/user roles đúng
4. **Browser Setting**: Tắt popup block, cho phép autoplay
5. **Network**: Đảm bảo https://localhost:5001 accessible

**🎊 Khi tất cả test PASS → Chức năng web hoạt động đúng theo yêu cầu!**
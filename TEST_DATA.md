# TEST DATA - WEB XEM PHIM

## 📋 TÀI KHOẢN TEST CẦN TẠO

### 👨‍💼 ADMIN ACCOUNT
```
Email: admin@webmovie.com  
Password: Admin123!
Vai trò: Administrator
```

### 👤 USER ACCOUNT  
```
Email: user@test.com
Password: User@1234  
Vai trò: User thường
```

### 👤 USER ACCOUNT #2 (cho test conflict)
```
Email: user2@test.com  
Password: User@1234
Vai trò: User thường
```

---

## 🎬 PHIM TEST CẦN CÓ

### Phim chính để test:
1. **"Mai"** - Phim có đủ chức năng comment, yêu thích
2. **"Avengers"** - Phim để test tìm kiếm
3. **"Doraemon"** - Phim hoạt hình
4. **"One Piece"** - Anime/Series có nhiều tập

### Yêu cầu cho mỗi phim:
- ✅ Có video source (có thể fake/demo video)  
- ✅ Slug đường dẫn rõ ràng (vd: `/Watch?slug=mai-2024`)
- ✅ Comment section hoạt động trên trang `/Watch`
- ✅ Hệ thống yêu thích (favorite) 
- ✅ Lưu lịch sử xem phim

---

## 🚀 HƯỚNG DẪN ĐĂNG KÝ

### Bước 1: Tạo Admin Account
1. Vào `https://localhost:5001/Account/Register`
2. Điền thông tin Admin ở trên
3. **Quan trọng**: Cần set role Admin trong database

### Bước 2: Tạo User Accounts  
1. Vào `https://localhost:5001/Account/Register`
2. Điền thông tin 2 User accounts ở trên

### Bước 3: Thêm Sample Movies
Qua trang Admin hoặc database, đảm bảo có ít nhất 4 phim test ở trên

---

## ⚡ KIỂM TRA SẴN SÀNG

Trước khi chạy test, kiểm tra:

### ✅ Website hoạt động:
- [ ] `https://localhost:5001` load được
- [ ] Login/Register hoạt động  
- [ ] Trang Admin/ManageMovies accessible với admin
- [ ] Trang Search có kết quả
- [ ] Trang Watch có video player + comment section

### ✅ Test Accounts:
- [ ] admin@webmovie.com đăng nhập được với quyền Admin
- [ ] user@test.com đăng nhập được  
- [ ] user2@test.com đăng nhập được

### ✅ Sample Data:
- [ ] Có ít nhất 4 phim: Mai, Avengers, Doraemon, One Piece
- [ ] Mỗi phim có slug để truy cập `/Watch?slug=...`
- [ ] Comment section hiển thị trên trang Watch

---

## 🔧 TROUBLESHOOTING

### Nếu đăng nhập thất bại:
1. Kiểm tra password chính xác: `Admin123!` và `User@1234`
2. Kiểm tra database có user này chưa
3. Thử reset password nếu cần

### Nếu Admin không có quyền:
```sql
-- Update user role in database
UPDATE AspNetUsers SET Role = 'Admin' WHERE Email = 'admin@webmovie.com';
-- OR
UPDATE UserRoles SET RoleId = 'admin-role-id' WHERE UserId = 'admin-user-id';
```

### Nếu không có phim:
1. Vào Admin/ManageMovies thêm phim mới
2. Hoặc import sample data vào database
3. Đảm bảo phim có video source (có thể fake URL)

---

## 📊 TEST VERIFICATION POINTS

### ✅ Comment Test sẽ verify:
- Comment mới xuất hiện sau khi gửi
- Nội dung comment chính xác (text + timestamp)  
- Avatar user hiển thị
- Nút Edit/Delete chỉ hiện với comment của mình
- Số lượng comment tăng/giảm khi thêm/xóa

### ✅ Watch Test sẽ verify:  
- Video player load và chạy (currentTime > 0)
- Lịch sử xem được lưu
- Progress được track
- Resume hoạt động đúng vị trí

### ✅ Search Test sẽ verify:
- Kết quả search chứa keyword
- Click vào kết quả navigate đúng trang
- No results khi search từ không tồn tại

---

**🎯 Khi đã setup xong, chạy test để kiểm tra từng chức năng có hoạt động đúng không!**
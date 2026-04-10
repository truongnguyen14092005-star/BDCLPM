# Integrated Test Cases - Admin Dashboard

| No. | Requirement | Test Case | Test Objective / Condition | Step # | Step Action | Test Data | Expected Result | Notes | Result | By Tester | Note |
| --- | ----------- | --------- | -------------------------- | ------ | ----------- | --------- | --------------- | ----- | ------ | --------- | ---- |
| 1 | D1 | DASH_INT_01 | Kiểm tra Dashboard Admin đã hiển thị thống kê đầy đủ | 1 | Vào Admin > Dashboard | - | Trang Dashboard load thành công | D1 |  |  |  |
| 1 | D1 | DASH_INT_01 | Kiểm tra Dashboard Admin đã hiển thị thống kê đầy đủ | 2 | Kiểm tra card "Tổng người dùng" | - | Hiển thị số đúng (khớp với Manage Users) | D1.1 |  |  |  |
| 1 | D1 | DASH_INT_01 | Kiểm tra Dashboard Admin đã hiển thị thống kê đầy đủ | 3 | Kiểm tra card "Lượt xem hôm nay" | - | Hiển thị số >= 0 | D1.2 |  |  |  |
| 1 | D1 | DASH_INT_01 | Kiểm tra Dashboard Admin đã hiển thị thống kê đầy đủ | 4 | Kiểm tra card "Tổng bình luận" | - | Hiển thị số đúng (khớp với Manage Comments) | D1.3 |  |  |  |
| 1 | D1 | DASH_INT_01 | Kiểm tra Dashboard Admin đã hiển thị thống kê đầy đủ | 5 | Kiểm tra card "Tổng phim" và "Phim đã ẩn" | - | Số phim từ API hiển thị. Số phim ẩn = số tab phim ẩn. | D1.4 |  |  |  |
| 1 | D1 | DASH_INT_01 | Kiểm tra Dashboard Admin đã hiển thị thống kê đầy đủ | 6 | Kiểm tra khu vực Top 10 phim xem nhiều nhất | - | Danh sách/biểu đồ Top 10 hiển thị đúng: tên phim + số lượt xem. Sắp xếp giảm dần. | D1.6 |  |  |  |
| 2 | D1.7 | DASH_INT_02 | Kiểm tra Dashboard cập nhật sau user activity | 1 | Ghi nhớ: "Tổng bình luận" = X, "Lượt xem hôm nay" = Y | - | Số liệu hiện tại được ghi nhận | D1.7 |  |  |  |
| 2 | D1.7 | DASH_INT_02 | Kiểm tra Dashboard cập nhật sau user activity | 2 | Mở tab mới / đăng nhập user thường / vào xem 1 phim | Movie: bất kỳ | Video player hiển thị, phim phát | D1.7 |  |  |  |
| 2 | D1.7 | DASH_INT_02 | Kiểm tra Dashboard cập nhật sau user activity | 3 | Thêm 1 bình luận vào phim đang xem | Comment: "Test dashboard sync" | Bình luận thêm thành công | D1.7 |  |  |  |
| 2 | D1.7 | DASH_INT_02 | Kiểm tra Dashboard cập nhật sau user activity | 4 | Quay lại tab Admin / Refresh Dashboard | - | "Tổng bình luận" = X+1, "Lượt xem hôm nay" tăng (Y+1 hoặc hơn) | D1.7 |  |  |  |
| 3 | D1.7 | DASH_INT_03 | Kiểm tra Dashboard cập nhật sau khi Admin xóa bình luận | 1 | Vào Manage Comments / Xóa tất cả bình luận của 1 phim | Giả sử có 5 comment | Xóa thành công | D1.7 |  |  |  |
| 3 | D1.7 | DASH_INT_03 | Kiểm tra Dashboard cập nhật sau khi Admin xóa bình luận | 2 | Vào Dashboard | - | "Tổng bình luận" = Z-5 | D1.7 |  |  |  |

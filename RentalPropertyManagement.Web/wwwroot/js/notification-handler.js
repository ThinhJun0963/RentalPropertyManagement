"use strict";

// Khởi tạo kết nối đến Hub
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/mainHub")
    .build();

// Các phần tử UI
var notificationList = document.getElementById("notificationList");
var notificationCount = document.getElementById("notificationCount");
var currentCount = 0;

// Bật/tắt nút Gửi
connection.start().then(function () {
    console.log("SignalR Notification Hub Connected.");
}).catch(function (err) {
    console.error("SignalR Connection Error: " + err.toString());
});

// 1. Hàm nhận thông báo mới từ Server
connection.on("ReceiveNotification", function (title, body, url) {
    // 1. Tăng số lượng và hiển thị badge
    currentCount++;
    notificationCount.textContent = currentCount;
    notificationCount.style.display = 'inline';

    // 2. Thêm thông báo vào danh sách dropdown

    // Xóa mục "Không có thông báo nào" nếu nó còn tồn tại
    var noNotificationItem = notificationList.querySelector('.text-muted');
    if (noNotificationItem) {
        noNotificationItem.remove();
    }

    var li = document.createElement("li");
    var a = document.createElement("a");

    a.className = 'dropdown-item dropdown-item-unread';
    a.href = url || "#";
    a.innerHTML = `<i class="fas fa-info-circle me-2 text-primary"></i> <strong>${title}</strong><br><small class="text-secondary">${body}</small>`;

    li.appendChild(a);
    notificationList.prepend(li); // Đặt thông báo mới lên đầu

    // Tùy chọn: Phát âm thanh hoặc hiển thị Toast Notification
    console.log(`NEW NOTIFICATION: ${title} - ${body}`);
});

// 2. Xử lý khi nhấn vào chuông (reset số lượng thông báo)
document.getElementById('notificationDropdown').addEventListener('click', function () {
    // Đánh dấu tất cả là đã đọc
    var unreadItems = notificationList.querySelectorAll('.dropdown-item-unread');
    unreadItems.forEach(item => {
        item.classList.remove('dropdown-item-unread');
    });

    // Reset số lượng
    currentCount = 0;
    notificationCount.textContent = '0';
    notificationCount.style.display = 'none';
});
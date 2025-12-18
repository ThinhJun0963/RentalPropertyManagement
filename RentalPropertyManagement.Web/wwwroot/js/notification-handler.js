"use strict";
// Khởi tạo kết nối đến Hub với tự động reconnect
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/mainHub")
    .withAutomaticReconnect() // Tự reconnect nếu mất kết nối
    .build();

// Các phần tử UI
var notificationList = document.getElementById("notificationList");
var notificationCount = document.getElementById("notificationCount");
var currentCount = 0;

// Start connection
connection.start().then(function () {
    console.log("SignalR Notification Hub Connected.");
}).catch(function (err) {
    console.error("SignalR Connection Error: " + err.toString());
});

// 1. Hàm nhận thông báo mới từ Server
connection.on("ReceiveNotification", function (title, body, url) {
    if (!notificationList || !notificationCount) {
        console.warn("UI elements for notifications not found.");
        return;
    }

    // 1. Tăng số lượng và hiển thị badge
    currentCount++;
    notificationCount.textContent = currentCount;
    notificationCount.style.display = 'inline';

    // 2. Xóa mục "Không có thông báo nào" nếu tồn tại
    var noNotificationItems = notificationList.querySelectorAll('.text-muted.small.text-center');
    noNotificationItems.forEach(item => item.remove());

    // 3. Tạo timestamp (ví dụ: "Vừa xong" hoặc thời gian cụ thể)
    var timestamp = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });

    // 4. Thêm thông báo vào danh sách dropdown (sử dụng createElement để tránh innerHTML vulnerable)
    var li = document.createElement("li");
    var a = document.createElement("a");
    a.className = 'dropdown-item dropdown-item-unread';
    a.href = url || "#";

    // Tạo nội dung an toàn (escape HTML nếu cần)
    var strong = document.createElement("strong");
    strong.textContent = title;
    var br = document.createElement("br");
    var smallBody = document.createElement("small");
    smallBody.className = "text-secondary";
    smallBody.textContent = body;
    var smallTime = document.createElement("small");
    smallTime.className = "text-muted ms-2";
    smallTime.textContent = `(${timestamp})`;

    a.appendChild(document.createElement("i")).className = "fas fa-info-circle me-2 text-primary";
    a.appendChild(strong);
    a.appendChild(br);
    a.appendChild(smallBody);
    a.appendChild(smallTime);

    li.appendChild(a);
    notificationList.prepend(li); // Đặt thông báo mới lên đầu

    // Tùy chọn: Phát âm thanh hoặc hiển thị Toast (Bootstrap)
    // var toast = new bootstrap.Toast(document.getElementById('myToast')); // Giả định có toast element
    // toast.show();

    console.log(`NEW NOTIFICATION: ${title} - ${body}`);
});

// 2. Xử lý khi nhấn vào chuông (reset số lượng thông báo)
var notificationDropdown = document.getElementById('notificationDropdown');
if (notificationDropdown) {
    notificationDropdown.addEventListener('click', function () {
        if (!notificationList) return;

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
}
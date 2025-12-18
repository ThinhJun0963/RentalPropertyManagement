/* File: RentalPropertyManagement.Web/wwwroot/js/chat.js
   Mô tả: Xử lý kết nối SignalR cho Chat Box và logic ẩn/hiện khung chat.
*/

document.addEventListener("DOMContentLoaded", function () {
    // 0. KHAI BÁO CÁC PHẦN TỬ UI
    const toggleBtn = document.getElementById("toggleChatBtn");
    const chatBox = document.getElementById("chatBox");
    const closeBtn = document.getElementById("closeChatBtn");
    const sendButton = document.getElementById("sendButton");
    const messageInput = document.getElementById("messageInput");
    const messagesList = document.getElementById("messagesList");

    // Kiểm tra nếu không có chatbox trên trang (ví dụ khi chưa đăng nhập) thì dừng script
    if (!toggleBtn || !chatBox) return;

    // 1. CẤU HÌNH KẾT NỐI SIGNALR
    // Đổi tên biến thành 'chatConnection' để tránh xung đột với biến 'connection' trong _Layout.cshtml
    const chatConnection = new signalR.HubConnectionBuilder()
        .withUrl("/mainHub")
        .withAutomaticReconnect()
        .build();

    // Tắt nút gửi cho đến khi kết nối thành công
    sendButton.disabled = true;

    // 2. LẮNG NGHE TIN NHẮN TỪ SERVER
    chatConnection.on("ReceiveGlobalMessage", function (user, message, timestamp) {
        const div = document.createElement("div");

        // Lấy thông tin người dùng hiện tại (nếu có element lưu email trong Layout)
        const currentUser = document.getElementById("userEmail")?.innerText || "";
        const isMe = user === currentUser;

        div.classList.add("message-item");
        div.classList.add(isMe ? "msg-me" : "msg-others");

        div.innerHTML = `
            <div class="msg-meta fw-bold">${user} <span class="fw-normal ms-1">${timestamp}</span></div>
            <div>${message}</div>
        `;

        messagesList.appendChild(div);

        // Tự động cuộn xuống dưới cùng khi có tin nhắn mới
        messagesList.scrollTop = messagesList.scrollHeight;
    });

    // 3. KHỞI ĐỘNG KẾT NỐI
    chatConnection.start().then(function () {
        sendButton.disabled = false;
        console.log("Chat Hub: Connected");
    }).catch(function (err) {
        console.error("Chat Hub Error: " + err.toString());
    });

    // 4. LOGIC GỬI TIN NHẮN
    function sendMessage() {
        const message = messageInput.value;
        if (message.trim() === "") return;

        // Gọi hàm SendGlobalMessage trên Server
        chatConnection.invoke("SendGlobalMessage", message).catch(function (err) {
            return console.error(err.toString());
        });

        messageInput.value = "";
        messageInput.focus();
    }

    // Sự kiện Click nút Gửi
    sendButton.addEventListener("click", function (event) {
        sendMessage();
        event.preventDefault();
    });

    // Sự kiện nhấn phím Enter trong ô nhập liệu
    messageInput.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            sendMessage();
            event.preventDefault();
        }
    });

    // 5. XỬ LÝ ẨN/HIỆN CHAT BOX
    toggleBtn.addEventListener("click", function () {
        // Kiểm tra trạng thái hiển thị thực tế bằng getComputedStyle
        const isHidden = chatBox.style.display === "none" ||
            window.getComputedStyle(chatBox).display === "none";

        if (isHidden) {
            chatBox.style.display = "block";
            messageInput.focus();
        } else {
            chatBox.style.display = "none";
        }
    });

    closeBtn.addEventListener("click", function () {
        chatBox.style.display = "none";
    });
});
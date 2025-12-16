// Kết nối tới Hub (sử dụng đường dẫn đã map trong Program.cs)
var connection = new signalR.HubConnectionBuilder().withUrl("/mainHub").build();

// Nút gửi và Input
var sendButton = document.getElementById("sendButton");
var messageInput = document.getElementById("messageInput");
var messagesList = document.getElementById("messagesList");

// Tắt nút gửi cho đến khi kết nối thành công
sendButton.disabled = true;

// 1. NHẬN TIN NHẮN TỪ SERVER
connection.on("ReceiveGlobalMessage", function (user, message, timestamp) {
    var div = document.createElement("div");

    // Kiểm tra xem tin nhắn là của mình hay người khác (Logic đơn giản dựa trên tên)
    // Lưu ý: Trong thực tế nên so sánh UserID ẩn
    var currentUser = document.getElementById("userEmail")?.innerText || "Khách"; // Cần 1 chỗ lưu email trong Layout

    var isMe = user === currentUser; // Logic tạm thời

    div.classList.add("message-item");
    div.classList.add(isMe ? "msg-me" : "msg-others");

    div.innerHTML = `
        <div class="msg-meta fw-bold">${user} <span class="fw-normal ms-1">${timestamp}</span></div>
        <div>${message}</div>
    `;

    messagesList.appendChild(div);

    // Tự động cuộn xuống dưới cùng
    messagesList.scrollTop = messagesList.scrollHeight;
});

// 2. KHỞI ĐỘNG KẾT NỐI
connection.start().then(function () {
    sendButton.disabled = false;
    console.log("Connected to Chat Hub");
}).catch(function (err) {
    return console.error(err.toString());
});

// 3. GỬI TIN NHẮN
function sendMessage() {
    var message = messageInput.value;
    if (message.trim() === "") return;

    connection.invoke("SendGlobalMessage", message).catch(function (err) {
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

// Sự kiện nhấn Enter
messageInput.addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        sendMessage();
        event.preventDefault();
    }
});

// 4. XỬ LÝ ẨN/HIỆN CHAT BOX
document.getElementById("toggleChatBtn").addEventListener("click", function () {
    var chatBox = document.getElementById("chatBox");
    if (chatBox.style.display === "none" || chatBox.style.display === "") {
        chatBox.style.display = "block";
    } else {
        chatBox.style.display = "none";
    }
});

document.getElementById("closeChatBtn").addEventListener("click", function () {
    document.getElementById("chatBox").style.display = "none";
});
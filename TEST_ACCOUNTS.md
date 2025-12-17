# 🔐 Tài Khoản Test - Payment System

⚠️ **BẢO MẬT:** File này chỉ dùng cho mục đích test. Không commit lên Git hoặc chia sẻ công khai.

## Tài Khoản Đăng Nhập Test

### Admin Account
- **Email:** `admin@test.com`
- **Password:** `Admin@123456`
- **Role:** Landlord (Quản Lý)
- **Quyền:** Quản lý toàn bộ hệ thống

### Landlord Account
- **Email:** `landlord@test.com`
- **Password:** `Landlord@123456`
- **Role:** Landlord (Chủ Nhà)
- **Quyền:** Quản lý bất động sản, xem thanh toán

### Tenant Account
- **Email:** `tenant@test.com`
- **Password:** `Tenant@123456`
- **Role:** Tenant (Người Thuê)
- **Quyền:** Xem hóa đơn, thanh toán tiền thuê

### Service Provider Account
- **Email:** `provider@test.com`
- **Password:** `Provider@123456`
- **Role:** Service Provider (Nhà Cung Cấp Dịch Vụ)
- **Quyền:** Xem và xử lý yêu cầu bảo trì

---

## Cách Sử Dụng

### 1. Chạy SQL Script
```sql
-- Chạy CREATE_DATABASE_PAYMENT_SYSTEM.sql
-- Nó sẽ insert các user với placeholder passwords
```

### 2. Hash Mật Khẩu
Sử dụng script C# dưới đây để hash các mật khẩu:

```csharp
using Microsoft.AspNetCore.Identity;

var hasher = new PasswordHasher<object>();

var accounts = new[]
{
    new { Email = "admin@test.com", Password = "Admin@123456" },
    new { Email = "landlord@test.com", Password = "Landlord@123456" },
    new { Email = "tenant@test.com", Password = "Tenant@123456" },
    new { Email = "provider@test.com", Password = "Provider@123456" }
};

foreach (var account in accounts)
{
    var hash = hasher.HashPassword(null, account.Password);
    Console.WriteLine($"-- {account.Email}");
    Console.WriteLine($"UPDATE Users SET PasswordHash = '{hash}' WHERE Email = '{account.Email}';");
    Console.WriteLine();
}
```

### 3. Chạy Update Statement
Lấy output từ script trên rồi chạy trong SQL Server:

```sql
-- Admin
UPDATE Users SET PasswordHash = '[HASH_VALUE_HERE]' WHERE Email = 'admin@test.com';

-- Landlord
UPDATE Users SET PasswordHash = '[HASH_VALUE_HERE]' WHERE Email = 'landlord@test.com';

-- Tenant
UPDATE Users SET PasswordHash = '[HASH_VALUE_HERE]' WHERE Email = 'tenant@test.com';

-- Provider
UPDATE Users SET PasswordHash = '[HASH_VALUE_HERE]' WHERE Email = 'provider@test.com';
```

---

## Test Payment Flow

### Quy Trình Test

1. **Đăng nhập với Tenant**
   - Email: `tenant@test.com`
   - Password: `Tenant@123456`

2. **Đi đến trang Pay Rent**
   - URL: `/Tenant/PayRent`
   - Sẽ thấy hóa đơn: "Tiền thuê tháng 1 năm 2025" - 5,000,000 VND

3. **Nhấn "Pay Now"**
   - Chuyển hướng đến trang ProcessPayment
   - Xác nhận thông tin thanh toán

4. **Kiểm Tra VNPay Callback**
   - Sử dụng VNPay Test Card
   - Hoàn thành giao dịch

5. **Xác Nhận Thanh Toán**
   - Xem trang Success
   - Transaction ID được lưu trong database

---

## Dữ Liệu Test

### Property Test
| ID | Địa Chỉ | Thành Phố | Giá Thuê |
|-----|---------|-----------|----------|
| 1 | 123 Đường Lê Lợi | Hồ Chí Minh | 5,000,000 VND |
| 2 | 456 Đường Nguyễn Huệ | Hà Nội | 8,000,000 VND |

### Contract Test
| ID | Property | Tenant | Ngày Bắt Đầu | Ngày Kết Thúc | Giá Thuê |
|-----|----------|--------|--------------|--------------|----------|
| 1 | 1 | Tenant | 2025-01-01 | 2026-01-01 | 5,000,000 VND |

### Payment Invoice Test
| ID | Contract | Tenant | Hạn Chót | Số Tiền | Trạng Thái |
|-----|----------|--------|----------|---------|-----------|
| 1 | 1 | Tenant | 2025-01-31 | 5,000,000 VND | Pending |

---

## Hỗ Trợ

- Nếu quên mật khẩu, dùng tài khoản khác để test
- Nếu muốn tạo tài khoản mới, sử dụng trang đăng ký (nếu có)
- Để xóa test data, chạy lại SQL script (nó sẽ drop database cũ)

---

## ⚠️ Lưu Ý Bảo Mật

**KHÔNG** commit file này lên repository công khai!

Thêm vào `.gitignore`:
```
TEST_ACCOUNTS.md
```

**Trong Production:**
- Đừng dùng tài khoản test này
- Tất cả mật khẩu phải được hash strong
- Sử dụng environment variables cho credentials

-- =====================================================
-- SQL Script: Tạo Database và Bảng cho Payment System
-- =====================================================
-- Execution: Chạy script này trên SQL Server Management Studio
-- Database: RentalManagementDB

USE master;
GO

-- =====================================================
-- 1. TẠO DATABASE
-- =====================================================
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'RentalManagementDB')
BEGIN
    ALTER DATABASE RentalManagementDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RentalManagementDB;
END
GO

CREATE DATABASE RentalManagementDB;
GO

USE RentalManagementDB;
GO

-- =====================================================
-- 2. TẠO BẢNG USERS
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        Role INT NOT NULL,  -- 1=Landlord, 2=Tenant, 3=ServiceProvider
        PhoneNumber NVARCHAR(20)
    );
    
    CREATE INDEX idx_Users_Email ON Users(Email);
END
GO

-- =====================================================
-- 3. TẠO BẢNG PROPERTIES
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Properties')
BEGIN
    CREATE TABLE Properties (
        Id INT PRIMARY KEY IDENTITY(1,1),
        [Address] NVARCHAR(255) NOT NULL,
        City NVARCHAR(100),
        SquareFootage INT,
        MonthlyRent DECIMAL(18, 2),
        [Description] NVARCHAR(MAX),
        IsOccupied BIT
    );
END
GO

-- =====================================================
-- 4. TẠO BẢNG CONTRACTS
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Contracts')
BEGIN
    CREATE TABLE Contracts (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PropertyId INT NOT NULL,
        TenantId INT NOT NULL,
        StartDate DATETIME2,
        EndDate DATETIME2,
        RentAmount DECIMAL(18, 2),
        [Status] INT NOT NULL,  -- 1=Pending, 2=Active, 3=Expired, 4=Terminated
        FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE NO ACTION,
        FOREIGN KEY (TenantId) REFERENCES Users(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX idx_Contracts_PropertyId ON Contracts(PropertyId);
    CREATE INDEX idx_Contracts_TenantId ON Contracts(TenantId);
END
GO

-- =====================================================
-- 5. TẠO BẢNG MAINTENANCE_REQUESTS
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MaintenanceRequests')
BEGIN
    CREATE TABLE MaintenanceRequests (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PropertyId INT NOT NULL,
        TenantId INT NOT NULL,
        AssignedProviderId INT,
        Title NVARCHAR(255),
        [Description] NVARCHAR(MAX),
        Priority INT,  -- 1=Low, 2=Medium, 3=High, 4=Urgent
        [Status] INT,  -- 1=New, 2=Approved, 3=Rejected, 4=InProgress, 5=Completed
        CreatedDate DATETIME2,
        CompletedDate DATETIME2,
        FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE NO ACTION,
        FOREIGN KEY (TenantId) REFERENCES Users(Id) ON DELETE NO ACTION,
        FOREIGN KEY (AssignedProviderId) REFERENCES Users(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX idx_MaintenanceRequests_PropertyId ON MaintenanceRequests(PropertyId);
    CREATE INDEX idx_MaintenanceRequests_TenantId ON MaintenanceRequests(TenantId);
    CREATE INDEX idx_MaintenanceRequests_AssignedProviderId ON MaintenanceRequests(AssignedProviderId);
END
GO

-- =====================================================
-- 6. TẠO BẢNG PAYMENT_INVOICES ⭐ MỚI
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PaymentInvoices')
BEGIN
    CREATE TABLE PaymentInvoices (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ContractId INT NOT NULL,
        TenantId INT NOT NULL,
        InvoiceDate DATETIME2 NOT NULL,
        DueDate DATETIME2 NOT NULL,
        PaidDate DATETIME2,
        Amount DECIMAL(18, 2) NOT NULL,
        [Description] NVARCHAR(MAX),
        [Status] INT NOT NULL,  -- 1=Pending, 2=Processing, 3=Completed, 4=Failed, 5=Cancelled, 6=Refunded
        TransactionReference NVARCHAR(MAX),
        FOREIGN KEY (ContractId) REFERENCES Contracts(Id) ON DELETE NO ACTION,
        FOREIGN KEY (TenantId) REFERENCES Users(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX idx_PaymentInvoices_ContractId ON PaymentInvoices(ContractId);
    CREATE INDEX idx_PaymentInvoices_TenantId ON PaymentInvoices(TenantId);
    CREATE INDEX idx_PaymentInvoices_Status ON PaymentInvoices([Status]);
END
GO

-- =====================================================
-- 7. TẠO BẢNG PAYMENTS ⭐ MỚI
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payments')
BEGIN
    CREATE TABLE Payments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PaymentInvoiceId INT NOT NULL,
        TenantId INT NOT NULL,
        PaymentDate DATETIME2 NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        PaymentMethod NVARCHAR(100),  -- VNPay, Stripe, etc.
        TransactionId NVARCHAR(255),  -- VNPay transaction ID
        ResponseCode NVARCHAR(10),    -- VNPay response code
        ResponseMessage NVARCHAR(MAX), -- VNPay response message
        [Status] INT NOT NULL,  -- 1=Pending, 2=Processing, 3=Completed, 4=Failed, 5=Cancelled, 6=Refunded
        FOREIGN KEY (PaymentInvoiceId) REFERENCES PaymentInvoices(Id) ON DELETE NO ACTION,
        FOREIGN KEY (TenantId) REFERENCES Users(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX idx_Payments_PaymentInvoiceId ON Payments(PaymentInvoiceId);
    CREATE INDEX idx_Payments_TenantId ON Payments(TenantId);
    CREATE INDEX idx_Payments_TransactionId ON Payments(TransactionId);
    CREATE INDEX idx_Payments_Status ON Payments([Status]);
END
GO

-- =====================================================
-- 8. THÊM DỮ LIỆU TEST (TÙY CHỌN)
-- =====================================================

-- ⭐ MẬT KHẨU TEST (TRƯỚC KHI HASH)
-- Admin:    admin@test.com / Admin@123456
-- Landlord: landlord@test.com / Landlord@123456
-- Tenant:   tenant@test.com / Tenant@123456
-- Provider: provider@test.com / Provider@123456

-- =====================================================
-- USERS TEST DATA (4 accounts)
-- =====================================================
INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Role, PhoneNumber)
VALUES 
    ('Admin', 'Quản Lý', 'admin@test.com', '$2a$12$.kFRvPFUH4AjN6ChMAVIc.hBuLsfKDvjvZuc4BD.GSlfraDX3mJ4e', 1, '0911111111'),
    ('Nguyễn', 'Chủ Nhà', 'landlord@test.com', '$2a$12$.kFRvPFUH4AjN6ChMAVIc.hBuLsfKDvjvZuc4BD.GSlfraDX3mJ4e', 1, '0912345678'),
    ('Trần', 'Người Thuê 1', 'tenant@test.com', '$2a$12$.kFRvPFUH4AjN6ChMAVIc.hBuLsfKDvjvZuc4BD.GSlfraDX3mJ4e', 2, '0987654321'),
    ('Lý', 'Dịch Vụ', 'provider@test.com', '$2a$12$.kFRvPFUH4AjN6ChMAVIc.hBuLsfKDvjvZuc4BD.GSlfraDX3mJ4e', 3, '0966666666'),
    ('Hoàng', 'Người Thuê 2', 'tenant2@test.com', '$2a$12$.kFRvPFUH4AjN6ChMAVIc.hBuLsfKDvjvZuc4BD.GSlfraDX3mJ4e', 2, '0987123456'),
    ('Phạm', 'Người Thuê 3', 'tenant3@test.com', '$2a$12$.kFRvPFUH4AjN6ChMAVIc.hBuLsfKDvjvZuc4BD.GSlfraDX3mJ4e', 2, '0987654000');
GO

-- =====================================================
-- PROPERTIES TEST DATA (5 properties)
-- =====================================================
INSERT INTO Properties ([Address], City, SquareFootage, MonthlyRent, [Description], IsOccupied)
VALUES 
    ('123 Đường Lê Lợi', 'Hồ Chí Minh', 50, 5000000, 'Căn hộ 1 phòng ngủ, đầy đủ tiện nghi', 1),
    ('456 Đường Nguyễn Huệ', 'Hà Nội', 80, 8000000, 'Căn hộ 2 phòng ngủ, gần trung tâm', 1),
    ('789 Đường Bạch Đằng', 'Hồ Chí Minh', 65, 6500000, 'Căn hộ 1 phòng ngủ, view biển', 0),
    ('321 Đường Hàng Bài', 'Hà Nội', 75, 7500000, 'Căn hộ 2 phòng ngủ, gần chợ', 1),
    ('654 Đường Trần Hưng Đạo', 'Đà Nẵng', 60, 5500000, 'Căn hộ 1 phòng ngủ, gần biển', 1);
GO

-- =====================================================
-- CONTRACTS TEST DATA (5 contracts)
-- =====================================================
INSERT INTO Contracts (PropertyId, TenantId, StartDate, EndDate, RentAmount, [Status])
VALUES 
    (1, 3, '2025-01-01', '2026-01-01', 5000000, 2),      -- Tenant 1 - Active
    (2, 4, '2025-01-15', '2026-01-15', 8000000, 2),      -- Tenant 2 - Active
    (4, 5, '2024-12-01', '2025-12-01', 7500000, 2),      -- Tenant 3 - Active
    (5, 3, '2025-02-01', '2026-02-01', 5500000, 1),      -- Tenant 1 - Pending (Secondary property)
    (3, 4, '2025-03-01', '2026-03-01', 6500000, 1);      -- Tenant 2 - Pending
GO

-- =====================================================
-- PAYMENT INVOICES TEST DATA (12 invoices for testing)
-- =====================================================
INSERT INTO PaymentInvoices (ContractId, TenantId, InvoiceDate, DueDate, PaidDate, Amount, [Description], [Status], TransactionReference)
VALUES 
    -- Tenant 1 - Contract 1 (5 invoices)
    (1, 3, '2025-01-01', '2025-01-31', NULL, 5000000, 'Tiền thuê tháng 1 năm 2025', 1, NULL),
    (1, 3, '2025-02-01', '2025-02-28', '2025-02-15', 5000000, 'Tiền thuê tháng 2 năm 2025', 3, 'VNP20250215001'),
    (1, 3, '2025-03-01', '2025-03-31', NULL, 5000000, 'Tiền thuê tháng 3 năm 2025', 1, NULL),
    (1, 3, '2025-04-01', '2025-04-30', NULL, 5000000, 'Tiền thuê tháng 4 năm 2025', 1, NULL),
    (1, 3, '2025-05-01', '2025-05-31', NULL, 5000000, 'Tiền thuê tháng 5 năm 2025', 4, NULL),

    -- Tenant 2 - Contract 2 (4 invoices)
    (2, 4, '2025-01-15', '2025-02-14', NULL, 8000000, 'Tiền thuê tháng 1 năm 2025', 1, NULL),
    (2, 4, '2025-02-15', '2025-03-14', '2025-03-01', 8000000, 'Tiền thuê tháng 2 năm 2025', 3, 'VNP20250301001'),
    (2, 4, '2025-03-15', '2025-04-14', NULL, 8000000, 'Tiền thuê tháng 3 năm 2025', 2, NULL),
    (2, 4, '2025-04-15', '2025-05-14', NULL, 8000000, 'Tiền thuê tháng 4 năm 2025', 1, NULL),

    -- Tenant 3 - Contract 3 (3 invoices)
    (3, 5, '2024-12-01', '2024-12-31', '2024-12-28', 7500000, 'Tiền thuê tháng 12 năm 2024', 3, 'VNP20241228001'),
    (3, 5, '2025-01-01', '2025-01-31', NULL, 7500000, 'Tiền thuê tháng 1 năm 2025', 1, NULL),
    (3, 5, '2025-02-01', '2025-02-28', NULL, 7500000, 'Tiền thuê tháng 2 năm 2025', 4, NULL);
GO

-- =====================================================
-- PAYMENT RECORDS TEST DATA (3 successful payments)
-- =====================================================
INSERT INTO Payments (PaymentInvoiceId, TenantId, PaymentDate, Amount, PaymentMethod, TransactionId, ResponseCode, ResponseMessage, [Status])
VALUES 
    (2, 3, '2025-02-15 14:30:00', 5000000, 'VNPay', 'VNP20250215001', '00', 'Successful payment', 3),
    (7, 4, '2025-03-01 10:15:00', 8000000, 'VNPay', 'VNP20250301001', '00', 'Successful payment', 3),
    (10, 5, '2024-12-28 16:45:00', 7500000, 'VNPay', 'VNP20241228001', '00', 'Successful payment', 3);
GO

-- =====================================================
-- 9. KIỂM TRA DỮ LIỆU
-- =====================================================
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Properties', COUNT(*) FROM Properties
UNION ALL
SELECT 'Contracts', COUNT(*) FROM Contracts
UNION ALL
SELECT 'PaymentInvoices', COUNT(*) FROM PaymentInvoices
UNION ALL
SELECT 'Payments', COUNT(*) FROM Payments;

-- In ra tất cả invoices
SELECT * FROM PaymentInvoices;
SELECT * FROM Payments;

PRINT 'Database và bảng đã được tạo thành công! ✅';
GO

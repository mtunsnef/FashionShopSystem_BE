-- 1. Tạo Database nếu chưa có
USE master;
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FashionShop')
    CREATE DATABASE FashionShop;
GO

-- 2. Sử dụng database này
USE FashionShop;
GO
-- 3. Tạo bảng Categories
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100),
    Description NVARCHAR(255)
);

-- 4. Tạo bảng Users
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    PasswordHash NVARCHAR(255),
    Phone NVARCHAR(20),
    Address NVARCHAR(255),
    Role NVARCHAR(20),
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- 5. Tạo bảng Products
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(200),
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2),
    Stock INT,
    CategoryID INT,
    Brand NVARCHAR(100),
    ImageUrl NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

-- 6. Tạo bảng Orders
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    ShippingAddress NVARCHAR(MAX) NOT NULL,
    Note NVARCHAR(MAX) NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(50) DEFAULT N'Pending',
    DeliveryStatus NVARCHAR(50) DEFAULT N'Pending',

    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- 7. Tạo bảng OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    ProductID INT,
    Quantity INT,
    Price DECIMAL(18,2),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- 8. Tạo bảng Favorites
CREATE TABLE Favorites (
    FavoriteID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    ProductID INT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

ALTER TABLE Users
ADD 
    Is2FAEnabled BIT NOT NULL DEFAULT 0,
    TwoFactorSecretKey NVARCHAR(255) NULL,
    Temp2FASecretKey NVARCHAR(255) NULL;
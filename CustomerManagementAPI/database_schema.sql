IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(200) NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'FirstName', N'LastName', N'PhoneNumber', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] ON;
INSERT INTO [Customers] ([Id], [CreatedAt], [Email], [FirstName], [LastName], [PhoneNumber], [UpdatedAt])
VALUES (1, '2026-02-24T23:03:14.9795970Z', N'john.doe@example.com', N'John', N'Doe', N'555-1234', NULL),
(2, '2026-02-24T23:03:14.9795973Z', N'jane.smith@example.com', N'Jane', N'Smith', N'555-5678', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'FirstName', N'LastName', N'PhoneNumber', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] OFF;
GO

CREATE INDEX [IX_Customers_Email] ON [Customers] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260224230315_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Price] decimal(18,2) NOT NULL,
    [SKU] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO

UPDATE [Customers] SET [CreatedAt] = '2026-02-25T01:07:46.2907689Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Customers] SET [CreatedAt] = '2026-02-25T01:07:46.2907695Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Description', N'Name', N'Price', N'SKU', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([Id], [CreatedAt], [Description], [Name], [Price], [SKU], [UpdatedAt])
VALUES (1, '2026-02-25T01:07:46.2907937Z', N'High-performance laptop with 16GB RAM', N'Laptop Computer', 999.99, N'LAPTOP-001', NULL),
(2, '2026-02-25T01:07:46.2907943Z', N'Ergonomic wireless mouse with Bluetooth', N'Wireless Mouse', 29.99, N'MOUSE-001', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Description', N'Name', N'Price', N'SKU', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] OFF;
GO

CREATE INDEX [IX_Products_Name] ON [Products] ([Name]);
GO

CREATE INDEX [IX_Products_SKU] ON [Products] ([SKU]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260225010746_AddProducts', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
);
GO

UPDATE [Customers] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Customers] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CustomerId', N'OrderDate', N'Status', N'TotalAmount', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Orders]'))
    SET IDENTITY_INSERT [Orders] ON;
INSERT INTO [Orders] ([Id], [CreatedAt], [CustomerId], [OrderDate], [Status], [TotalAmount], [UpdatedAt])
VALUES (1, '2026-02-01T10:00:00.0000000Z', 1, '2026-02-01T10:00:00.0000000Z', N'Completed', 1029.98, NULL),
(2, '2026-02-15T14:30:00.0000000Z', 2, '2026-02-15T14:30:00.0000000Z', N'Pending', 59.98, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CustomerId', N'OrderDate', N'Status', N'TotalAmount', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Orders]'))
    SET IDENTITY_INSERT [Orders] OFF;
GO

UPDATE [Products] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Products] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'OrderId', N'ProductId', N'Quantity', N'Subtotal', N'UnitPrice') AND [object_id] = OBJECT_ID(N'[OrderItems]'))
    SET IDENTITY_INSERT [OrderItems] ON;
INSERT INTO [OrderItems] ([Id], [OrderId], [ProductId], [Quantity], [Subtotal], [UnitPrice])
VALUES (1, 1, 1, 1, 999.99, 999.99),
(2, 1, 2, 1, 29.99, 29.99),
(3, 2, 2, 2, 59.98, 29.99);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'OrderId', N'ProductId', N'Quantity', N'Subtotal', N'UnitPrice') AND [object_id] = OBJECT_ID(N'[OrderItems]'))
    SET IDENTITY_INSERT [OrderItems] OFF;
GO

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
GO

CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
GO

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);
GO

CREATE INDEX [IX_Orders_OrderDate] ON [Orders] ([OrderDate]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260225021805_AddOrders', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(50) NOT NULL,
    [PasswordHash] nvarchar(200) NOT NULL,
    [Email] nvarchar(200) NULL,
    [FullName] nvarchar(100) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'FullName', N'PasswordHash', N'UpdatedAt', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [CreatedAt], [Email], [FullName], [PasswordHash], [UpdatedAt], [Username])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'admin@example.com', N'System Administrator', N'$2a$10$rQEYn6/U5rvBl7yx5gZ5/.qP.tMI8I9E7tTJ.H5KH.aZvGx6Z5OPK', NULL, N'admin'),
(2, '2026-01-01T00:00:00.0000000Z', N'user@example.com', N'Regular User', N'$2a$10$rP8BV8Z5F5YqG2YB6S5xOeQxvN5M8M5M5O5O5N5N5M5M5M5M5M5M', NULL, N'user');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'FullName', N'PasswordHash', N'UpdatedAt', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260225031433_AddUsers', N'8.0.0');
GO

COMMIT;
GO


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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [Businesses] (
        [BusinessId] int NOT NULL IDENTITY,
        [LocationName] nvarchar(100) NOT NULL,
        [Slogan] nvarchar(100) NOT NULL,
        [Street] nvarchar(200) NOT NULL,
        [City] nvarchar(100) NOT NULL,
        [PostalCode] nvarchar(10) NOT NULL,
        [StateProvince] nvarchar(100) NOT NULL,
        [Country] nvarchar(100) NOT NULL,
        [ContactPerson] nvarchar(100) NOT NULL,
        [ContactEmail] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [ModifiedDate] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Businesses] PRIMARY KEY ([BusinessId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [Customers] (
        [CustomerId] int NOT NULL IDENTITY,
        [FirstName] nvarchar(50) NOT NULL,
        [LastName] nvarchar(50) NOT NULL,
        [Title] nvarchar(10) NOT NULL,
        [EmailAddress] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(15) NOT NULL,
        [ProfilePhoto] varbinary(max) NULL,
        [BusinessRole] nvarchar(50) NOT NULL,
        [BusinessName] nvarchar(100) NOT NULL,
        [BusinessEmailAddress] nvarchar(100) NOT NULL,
        [BusinessPhoneNumber] nvarchar(15) NOT NULL,
        [BusinessType] nvarchar(50) NOT NULL,
        [Industry] nvarchar(50) NOT NULL,
        [StreetAddress] nvarchar(200) NOT NULL,
        [City] nvarchar(50) NOT NULL,
        [PostalCode] nvarchar(10) NOT NULL,
        [Province] nvarchar(50) NOT NULL,
        [Country] nvarchar(50) NOT NULL,
        [Password] nvarchar(256) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [ModifiedDate] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [Employees] (
        [EmployeeId] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [Role] nvarchar(100) NOT NULL,
        [DateOfHire] datetime2 NOT NULL,
        [Title] nvarchar(100) NOT NULL,
        [Responsibility] nvarchar(500) NOT NULL,
        [Password] nvarchar(256) NOT NULL,
        [ProfilePhoto] varbinary(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [ModifiedDate] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([EmployeeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [Settings] (
        [SettingId] int NOT NULL IDENTITY,
        [BusinessName] nvarchar(100) NOT NULL,
        [BusinessLogo] varbinary(max) NOT NULL,
        [CoverPhoto] varbinary(max) NOT NULL,
        [ContactEmail] nvarchar(max) NOT NULL,
        [ContactPhone] nvarchar(max) NOT NULL,
        [BusinessId] int NOT NULL,
        [ModifiedDate] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Settings] PRIMARY KEY ([SettingId]),
        CONSTRAINT [FK_Settings_Businesses_BusinessId] FOREIGN KEY ([BusinessId]) REFERENCES [Businesses] ([BusinessId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [Fridges] (
        [FridgeId] int NOT NULL IDENTITY,
        [SerialNumber] nvarchar(50) NOT NULL,
        [ModelType] nvarchar(100) NOT NULL,
        [DoorType] nvarchar(100) NOT NULL,
        [Size] nvarchar(100) NOT NULL,
        [Capacity] nvarchar(50) NOT NULL,
        [Condition] nvarchar(50) NOT NULL,
        [SupplierName] nvarchar(100) NOT NULL,
        [SupplierContact] nvarchar(50) NOT NULL,
        [DeliveryDocumentation] varbinary(max) NULL,
        [DeliveryDocumentationFileName] nvarchar(max) NULL,
        [FridgeImage] varbinary(max) NULL,
        [FridgeImageFileName] nvarchar(max) NULL,
        [WarrantyStartDate] datetime2 NULL,
        [WarrantyEndDate] datetime2 NULL,
        [Note] nvarchar(500) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        [IsInStock] bit NOT NULL,
        [IsScrapped] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [IsAllocated] bit NOT NULL,
        [CreatedBy] nvarchar(100) NOT NULL,
        [LastModifiedBy] nvarchar(100) NULL,
        [EmployeeId] int NULL,
        [LastModifiedDate] datetime2 NULL,
        CONSTRAINT [PK_Fridges] PRIMARY KEY ([FridgeId], [SerialNumber]),
        CONSTRAINT [FK_Fridges_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [FridgeAllocations] (
        [FridgeAllocationId] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [FridgeId] int NOT NULL,
        [EmployeeId] int NOT NULL,
        [SerialNumber] nvarchar(50) NULL,
        [AllocationDate] datetime2 NOT NULL,
        [Duration] int NOT NULL,
        [SpecialInstructions] nvarchar(500) NOT NULL,
        [IsProcessed] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_FridgeAllocations] PRIMARY KEY ([FridgeAllocationId]),
        CONSTRAINT [FK_FridgeAllocations_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FridgeAllocations_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FridgeAllocations_Fridges_FridgeId_SerialNumber] FOREIGN KEY ([FridgeId], [SerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [PurchaseRequests] (
        [PurchaseRequestId] int NOT NULL IDENTITY,
        [FridgeId] int NOT NULL,
        [EmployeeId] int NOT NULL,
        [FridgeModelType] nvarchar(max) NOT NULL,
        [SerialNumber] nvarchar(50) NULL,
        [Capacity] nvarchar(50) NOT NULL,
        [Quantity] int NOT NULL,
        [Urgency] nvarchar(20) NOT NULL,
        [Justification] nvarchar(500) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [RequestStatus] int NOT NULL,
        [IsApproved] bit NOT NULL,
        CONSTRAINT [PK_PurchaseRequests] PRIMARY KEY ([PurchaseRequestId]),
        CONSTRAINT [FK_PurchaseRequests_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseRequests_Fridges_FridgeId_SerialNumber] FOREIGN KEY ([FridgeId], [SerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [ScrappedFridges] (
        [ScrappedFridgeId] int NOT NULL IDENTITY,
        [FridgeId] int NOT NULL,
        [EmployeeId] int NOT NULL,
        [FridgeSerialNumber] nvarchar(50) NULL,
        [ScrapDate] datetime2 NOT NULL,
        [ScrapReason] nvarchar(500) NOT NULL,
        [Notes] nvarchar(1000) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedDate] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ScrappedFridges] PRIMARY KEY ([ScrappedFridgeId]),
        CONSTRAINT [FK_ScrappedFridges_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ScrappedFridges_Fridges_FridgeId_FridgeSerialNumber] FOREIGN KEY ([FridgeId], [FridgeSerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE TABLE [ProcessAllocations] (
        [ProcessAllocationId] int NOT NULL IDENTITY,
        [FridgeAllocationId] int NOT NULL,
        [CustomerName] nvarchar(100) NOT NULL,
        [CustomerLast] nvarchar(100) NOT NULL,
        [CustomerId] int NOT NULL,
        [EmployeeId] int NOT NULL,
        [FridgeId] int NOT NULL,
        [SerialNumber] nvarchar(50) NOT NULL,
        [AllocationDate] datetime2 NOT NULL,
        [DeliveryPickupDate] datetime2 NOT NULL,
        [SpecialInstructions] nvarchar(500) NULL,
        [ApprovalStatus] nvarchar(20) NOT NULL,
        [ApprovalNote] nvarchar(500) NULL,
        [LastModifiedBy] nvarchar(100) NULL,
        [LastModifiedDate] datetime2 NULL,
        CONSTRAINT [PK_ProcessAllocations] PRIMARY KEY ([ProcessAllocationId]),
        CONSTRAINT [FK_ProcessAllocations_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProcessAllocations_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProcessAllocations_FridgeAllocations_FridgeAllocationId] FOREIGN KEY ([FridgeAllocationId]) REFERENCES [FridgeAllocations] ([FridgeAllocationId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProcessAllocations_Fridges_FridgeId_SerialNumber] FOREIGN KEY ([FridgeId], [SerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_FridgeAllocations_CustomerId] ON [FridgeAllocations] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_FridgeAllocations_EmployeeId] ON [FridgeAllocations] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_FridgeAllocations_FridgeId_SerialNumber] ON [FridgeAllocations] ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_Fridges_EmployeeId] ON [Fridges] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_ProcessAllocations_CustomerId] ON [ProcessAllocations] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_ProcessAllocations_EmployeeId] ON [ProcessAllocations] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_ProcessAllocations_FridgeAllocationId] ON [ProcessAllocations] ([FridgeAllocationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_ProcessAllocations_FridgeId_SerialNumber] ON [ProcessAllocations] ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_PurchaseRequests_EmployeeId] ON [PurchaseRequests] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_PurchaseRequests_FridgeId_SerialNumber] ON [PurchaseRequests] ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_ScrappedFridges_EmployeeId] ON [ScrappedFridges] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_ScrappedFridges_FridgeId_FridgeSerialNumber] ON [ScrappedFridges] ([FridgeId], [FridgeSerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    CREATE INDEX [IX_Settings_BusinessId] ON [Settings] ([BusinessId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241016004040_New'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241016004040_New', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017071614_NotificationsTable'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [Message] nvarchar(max) NOT NULL,
        [ActionBy] nvarchar(max) NOT NULL,
        [Date] datetime2 NOT NULL,
        [IsRead] bit NOT NULL,
        [EmployeeId] int NOT NULL,
        [CustomerId] int NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Notifications_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017071614_NotificationsTable'
)
BEGIN
    CREATE INDEX [IX_Notifications_CustomerId] ON [Notifications] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017071614_NotificationsTable'
)
BEGIN
    CREATE INDEX [IX_Notifications_EmployeeId] ON [Notifications] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017071614_NotificationsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241017071614_NotificationsTable', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017191301_Adee'
)
BEGIN
    ALTER TABLE [Notifications] DROP CONSTRAINT [FK_Notifications_Customers_CustomerId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017191301_Adee'
)
BEGIN
    ALTER TABLE [Notifications] DROP CONSTRAINT [FK_Notifications_Employees_EmployeeId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017191301_Adee'
)
BEGIN
    DROP INDEX [IX_Notifications_CustomerId] ON [Notifications];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017191301_Adee'
)
BEGIN
    DROP INDEX [IX_Notifications_EmployeeId] ON [Notifications];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017191301_Adee'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Notifications]') AND [c].[name] = N'EmployeeId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Notifications] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Notifications] ALTER COLUMN [EmployeeId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017191301_Adee'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Notifications]') AND [c].[name] = N'CustomerId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Notifications] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Notifications] ALTER COLUMN [CustomerId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017191301_Adee'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241017191301_Adee', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017212420_Test'
)
BEGIN
    CREATE TABLE [EmployeeNotificationStatuses] (
        [Id] int NOT NULL IDENTITY,
        [NotificationId] int NOT NULL,
        [EmployeeId] int NULL,
        [CustomerId] int NULL,
        [IsRead] bit NOT NULL,
        CONSTRAINT [PK_EmployeeNotificationStatuses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EmployeeNotificationStatuses_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]),
        CONSTRAINT [FK_EmployeeNotificationStatuses_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]),
        CONSTRAINT [FK_EmployeeNotificationStatuses_Notifications_NotificationId] FOREIGN KEY ([NotificationId]) REFERENCES [Notifications] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017212420_Test'
)
BEGIN
    CREATE INDEX [IX_EmployeeNotificationStatuses_CustomerId] ON [EmployeeNotificationStatuses] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017212420_Test'
)
BEGIN
    CREATE INDEX [IX_EmployeeNotificationStatuses_EmployeeId] ON [EmployeeNotificationStatuses] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017212420_Test'
)
BEGIN
    CREATE INDEX [IX_EmployeeNotificationStatuses_NotificationId] ON [EmployeeNotificationStatuses] ([NotificationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241017212420_Test'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241017212420_Test', N'8.0.8');
END;
GO

COMMIT;
GO


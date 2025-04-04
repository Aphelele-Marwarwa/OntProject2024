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
    WHERE [MigrationId] = N'20240904234129_FixedProblem'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'ModifiedBy');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [ModifiedBy] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240904234129_FixedProblem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240904234129_FixedProblem', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    EXEC sp_rename N'[ScrappedFridges].[Note]', N'UpdatedBy', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    ALTER TABLE [ScrappedFridges] ADD [UpdatedDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessAllocations]') AND [c].[name] = N'SpecialInstructions');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ProcessAllocations] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [ProcessAllocations] ALTER COLUMN [SpecialInstructions] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessAllocations]') AND [c].[name] = N'LastModifiedBy');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ProcessAllocations] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [ProcessAllocations] ALTER COLUMN [LastModifiedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessAllocations]') AND [c].[name] = N'CustomerName');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ProcessAllocations] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [ProcessAllocations] ALTER COLUMN [CustomerName] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessAllocations]') AND [c].[name] = N'ApprovalStatus');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [ProcessAllocations] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [ProcessAllocations] ALTER COLUMN [ApprovalStatus] nvarchar(20) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessAllocations]') AND [c].[name] = N'ApprovalNote');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [ProcessAllocations] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [ProcessAllocations] ALTER COLUMN [ApprovalNote] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    ALTER TABLE [ProcessAllocations] ADD [CustomerId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'SupplierName');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [SupplierName] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'SupplierContact');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [SupplierContact] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'SerialNumber');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [SerialNumber] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'Note');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [Note] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'LastModifiedBy');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [LastModifiedBy] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'DeliveryDocumentationFileName');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [DeliveryDocumentationFileName] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'DeliveryDocumentation');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [DeliveryDocumentation] varbinary(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'CreatedBy');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [CreatedBy] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fridges]') AND [c].[name] = N'Condition');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Fridges] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [Fridges] ALTER COLUMN [Condition] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    ALTER TABLE [Fridges] ADD [FridgeImage] varbinary(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    ALTER TABLE [Fridges] ADD [FridgeImageFileName] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FridgeAllocations]') AND [c].[name] = N'SpecialInstructions');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [FridgeAllocations] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [FridgeAllocations] ALTER COLUMN [SpecialInstructions] nvarchar(500) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    ALTER TABLE [FridgeAllocations] ADD [RowVersion] rowversion NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'Email');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [Employees] ALTER COLUMN [Email] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'Title');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [Title] nvarchar(10) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'PhoneNumber');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [PhoneNumber] nvarchar(15) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'Industry');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [Industry] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'EmailAddress');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [EmailAddress] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'BusinessType');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [BusinessType] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'BusinessRole');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [BusinessRole] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'BusinessPhoneNumber');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [BusinessPhoneNumber] nvarchar(15) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'BusinessEmailAddress');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [BusinessEmailAddress] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    CREATE INDEX [IX_ProcessAllocations_CustomerId] ON [ProcessAllocations] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    CREATE INDEX [IX_ProcessAllocations_FridgeId] ON [ProcessAllocations] ([FridgeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    ALTER TABLE [ProcessAllocations] ADD CONSTRAINT [FK_ProcessAllocations_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    ALTER TABLE [ProcessAllocations] ADD CONSTRAINT [FK_ProcessAllocations_Fridges_FridgeId] FOREIGN KEY ([FridgeId]) REFERENCES [Fridges] ([FridgeId]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240905123751_FinalModelsSubA'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240905123751_FinalModelsSubA', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [FridgeAllocations] DROP CONSTRAINT [FK_FridgeAllocations_Fridges_ModelType_SerialNumber];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ProcessAllocations] DROP CONSTRAINT [FK_ProcessAllocations_Fridges_FridgeModelType_FridgeSerialNumber];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [PurchaseRequests] DROP CONSTRAINT [FK_PurchaseRequests_Fridges_FridgeModelType_FridgeSerialNumber];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ScrappedFridges] DROP CONSTRAINT [FK_ScrappedFridges_Fridges_FridgeModelType1_FridgeSerialNumber1];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ScrappedFridges] DROP CONSTRAINT [FK_ScrappedFridges_Fridges_FridgeModelType_FridgeSerialNumber];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [FaultTechs];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [Item];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [NewFridgeRequests];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [Reports];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [DeliveryNote];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [Procurement];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [PurchaseOrder];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [InventoryLiaisons];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [Quotation];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [RFQ];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP TABLE [Supplier];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP INDEX [IX_ScrappedFridges_FridgeModelType_FridgeSerialNumber] ON [ScrappedFridges];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP INDEX [IX_ScrappedFridges_FridgeModelType1_FridgeSerialNumber1] ON [ScrappedFridges];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP INDEX [IX_PurchaseRequests_FridgeModelType_FridgeSerialNumber] ON [PurchaseRequests];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP INDEX [IX_ProcessAllocations_FridgeModelType_FridgeSerialNumber] ON [ProcessAllocations];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [Fridges] DROP CONSTRAINT [PK_Fridges];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DROP INDEX [IX_FridgeAllocations_ModelType_SerialNumber] ON [FridgeAllocations];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ScrappedFridges]') AND [c].[name] = N'FridgeModelType1');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [ScrappedFridges] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [ScrappedFridges] DROP COLUMN [FridgeModelType1];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ScrappedFridges]') AND [c].[name] = N'FridgeModelType');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [ScrappedFridges] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [ScrappedFridges] ALTER COLUMN [FridgeModelType] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ScrappedFridges] ADD [FridgeId1] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseRequests]') AND [c].[name] = N'FridgeModelType');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseRequests] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [PurchaseRequests] ALTER COLUMN [FridgeModelType] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [PurchaseRequests] ADD [FridgeId1] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [PurchaseRequests] ADD [FridgeSerialNumber1] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DECLARE @var28 sysname;
    SELECT @var28 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProcessAllocations]') AND [c].[name] = N'FridgeModelType');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [ProcessAllocations] DROP CONSTRAINT [' + @var28 + '];');
    ALTER TABLE [ProcessAllocations] ALTER COLUMN [FridgeModelType] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ProcessAllocations] ADD [FridgeId1] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ProcessAllocations] ADD [FridgeSerialNumber1] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    DECLARE @var29 sysname;
    SELECT @var29 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FridgeAllocations]') AND [c].[name] = N'ModelType');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [FridgeAllocations] DROP CONSTRAINT [' + @var29 + '];');
    ALTER TABLE [FridgeAllocations] ALTER COLUMN [ModelType] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [FridgeAllocations] ADD [FridgeId1] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [FridgeAllocations] ADD [FridgeSerialNumber] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [Fridges] ADD CONSTRAINT [PK_Fridges] PRIMARY KEY ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    CREATE INDEX [IX_ScrappedFridges_FridgeId_FridgeSerialNumber] ON [ScrappedFridges] ([FridgeId], [FridgeSerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    CREATE INDEX [IX_ScrappedFridges_FridgeId1_FridgeSerialNumber1] ON [ScrappedFridges] ([FridgeId1], [FridgeSerialNumber1]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    CREATE INDEX [IX_PurchaseRequests_FridgeId_FridgeSerialNumber] ON [PurchaseRequests] ([FridgeId], [FridgeSerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    CREATE INDEX [IX_PurchaseRequests_FridgeId1_FridgeSerialNumber1] ON [PurchaseRequests] ([FridgeId1], [FridgeSerialNumber1]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    CREATE INDEX [IX_ProcessAllocations_FridgeId_FridgeSerialNumber] ON [ProcessAllocations] ([FridgeId], [FridgeSerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    CREATE INDEX [IX_ProcessAllocations_FridgeId1_FridgeSerialNumber1] ON [ProcessAllocations] ([FridgeId1], [FridgeSerialNumber1]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    CREATE INDEX [IX_FridgeAllocations_FridgeId_SerialNumber] ON [FridgeAllocations] ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    CREATE INDEX [IX_FridgeAllocations_FridgeId1_FridgeSerialNumber] ON [FridgeAllocations] ([FridgeId1], [FridgeSerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [FridgeAllocations] ADD CONSTRAINT [FK_FridgeAllocations_Fridges_FridgeId1_FridgeSerialNumber] FOREIGN KEY ([FridgeId1], [FridgeSerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [FridgeAllocations] ADD CONSTRAINT [FK_FridgeAllocations_Fridges_FridgeId_SerialNumber] FOREIGN KEY ([FridgeId], [SerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ProcessAllocations] ADD CONSTRAINT [FK_ProcessAllocations_Fridges_FridgeId1_FridgeSerialNumber1] FOREIGN KEY ([FridgeId1], [FridgeSerialNumber1]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ProcessAllocations] ADD CONSTRAINT [FK_ProcessAllocations_Fridges_FridgeId_FridgeSerialNumber] FOREIGN KEY ([FridgeId], [FridgeSerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [PurchaseRequests] ADD CONSTRAINT [FK_PurchaseRequests_Fridges_FridgeId1_FridgeSerialNumber1] FOREIGN KEY ([FridgeId1], [FridgeSerialNumber1]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [PurchaseRequests] ADD CONSTRAINT [FK_PurchaseRequests_Fridges_FridgeId_FridgeSerialNumber] FOREIGN KEY ([FridgeId], [FridgeSerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ScrappedFridges] ADD CONSTRAINT [FK_ScrappedFridges_Fridges_FridgeId1_FridgeSerialNumber1] FOREIGN KEY ([FridgeId1], [FridgeSerialNumber1]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    ALTER TABLE [ScrappedFridges] ADD CONSTRAINT [FK_ScrappedFridges_Fridges_FridgeId_FridgeSerialNumber] FOREIGN KEY ([FridgeId], [FridgeSerialNumber]) REFERENCES [Fridges] ([FridgeId], [SerialNumber]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240911211934_Coonect'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240911211934_Coonect', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240912174133_AddFridgeSerialNumberToScrappedFridges'
)
BEGIN
    DECLARE @var30 sysname;
    SELECT @var30 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ScrappedFridges]') AND [c].[name] = N'FridgeModelType');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [ScrappedFridges] DROP CONSTRAINT [' + @var30 + '];');
    ALTER TABLE [ScrappedFridges] DROP COLUMN [FridgeModelType];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240912174133_AddFridgeSerialNumberToScrappedFridges'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240912174133_AddFridgeSerialNumberToScrappedFridges', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240919155444_Add'
)
BEGIN
    ALTER TABLE [PurchaseRequests] ADD [Status] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240919155444_Add'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240919155444_Add', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241002004226_Night'
)
BEGIN
    ALTER TABLE [Fridges] ADD [EmployeeId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241002004226_Night'
)
BEGIN
    CREATE INDEX [IX_Fridges_EmployeeId] ON [Fridges] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241002004226_Night'
)
BEGIN
    ALTER TABLE [Fridges] ADD CONSTRAINT [FK_Fridges_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241002004226_Night'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241002004226_Night', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241012192454_Addinges'
)
BEGIN
    DECLARE @var31 sysname;
    SELECT @var31 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'Password');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var31 + '];');
    EXEC(N'UPDATE [Customers] SET [Password] = N'''' WHERE [Password] IS NULL');
    ALTER TABLE [Customers] ALTER COLUMN [Password] nvarchar(256) NOT NULL;
    ALTER TABLE [Customers] ADD DEFAULT N'' FOR [Password];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241012192454_Addinges'
)
BEGIN
    DECLARE @var32 sysname;
    SELECT @var32 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'CreatedBy');
    IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var32 + '];');
    EXEC(N'UPDATE [Customers] SET [CreatedBy] = N'''' WHERE [CreatedBy] IS NULL');
    ALTER TABLE [Customers] ALTER COLUMN [CreatedBy] nvarchar(max) NOT NULL;
    ALTER TABLE [Customers] ADD DEFAULT N'' FOR [CreatedBy];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241012192454_Addinges'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241012192454_Addinges', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241012202202_downi'
)
BEGIN
    DECLARE @var33 sysname;
    SELECT @var33 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'ModifiedBy');
    IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var33 + '];');
    EXEC(N'UPDATE [Customers] SET [ModifiedBy] = N'''' WHERE [ModifiedBy] IS NULL');
    ALTER TABLE [Customers] ALTER COLUMN [ModifiedBy] nvarchar(max) NOT NULL;
    ALTER TABLE [Customers] ADD DEFAULT N'' FOR [ModifiedBy];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241012202202_downi'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241012202202_downi', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241012202601_downing'
)
BEGIN
    DECLARE @var34 sysname;
    SELECT @var34 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'ModifiedBy');
    IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var34 + '];');
    ALTER TABLE [Customers] ALTER COLUMN [ModifiedBy] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241012202601_downing'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241012202601_downing', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019222246_updatedRequest'
)
BEGIN
    CREATE TABLE [NewFridgeRequests] (
        [NewFridgeRequestId] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [DateApplied] datetime2 NOT NULL,
        [FridgeType] nvarchar(100) NOT NULL,
        [Capacity] nvarchar(max) NOT NULL,
        [Condition] nvarchar(max) NOT NULL,
        [Duration] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_NewFridgeRequests] PRIMARY KEY ([NewFridgeRequestId]),
        CONSTRAINT [FK_NewFridgeRequests_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019222246_updatedRequest'
)
BEGIN
    CREATE INDEX [IX_NewFridgeRequests_CustomerId] ON [NewFridgeRequests] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019222246_updatedRequest'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241019222246_updatedRequest', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019224232_twenty3'
)
BEGIN
    EXEC sp_rename N'[NewFridgeRequests].[FridgeType]', N'ModelType', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019224232_twenty3'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241019224232_twenty3', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    ALTER TABLE [PurchaseRequests] ADD [SupplierDSupplierId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    ALTER TABLE [PurchaseRequests] ADD [SupplierId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE TABLE [Suppliers] (
        [SupplierId] int NOT NULL IDENTITY,
        [SupplierName] nvarchar(100) NOT NULL,
        [PhoneNumber] int NOT NULL,
        [EmailAddress] nvarchar(max) NOT NULL,
        [LegalStatus] nvarchar(max) NOT NULL,
        [StreetAddress] nvarchar(max) NOT NULL,
        [City] nvarchar(max) NOT NULL,
        [Code] int NOT NULL,
        [Province] nvarchar(max) NOT NULL,
        [Country] nvarchar(max) NOT NULL,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [AlternnativePhoneNumber] int NOT NULL,
        [BusinessRole] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Suppliers] PRIMARY KEY ([SupplierId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE TABLE [RFQs] (
        [RFQId] int NOT NULL IDENTITY,
        [PurchaseRequestId] int NOT NULL,
        [SupplierId] int NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_RFQs] PRIMARY KEY ([RFQId]),
        CONSTRAINT [FK_RFQs_PurchaseRequests_PurchaseRequestId] FOREIGN KEY ([PurchaseRequestId]) REFERENCES [PurchaseRequests] ([PurchaseRequestId]) ON DELETE CASCADE,
        CONSTRAINT [FK_RFQs_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([SupplierId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE TABLE [Quotations] (
        [QuotationId] int NOT NULL IDENTITY,
        [RFQId] int NOT NULL,
        [SupplierId] int NOT NULL,
        [UpdatedBy] nvarchar(max) NOT NULL,
        [UpdatedDate] datetime2 NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Quotations] PRIMARY KEY ([QuotationId]),
        CONSTRAINT [FK_Quotations_RFQs_RFQId] FOREIGN KEY ([RFQId]) REFERENCES [RFQs] ([RFQId]),
        CONSTRAINT [FK_Quotations_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([SupplierId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE TABLE [PurchaseOrders] (
        [PurchaseOrderId] int NOT NULL IDENTITY,
        [QuotationId] int NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        [OrderStatus] nvarchar(max) NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(max) NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY ([PurchaseOrderId]),
        CONSTRAINT [FK_PurchaseOrders_Quotations_QuotationId] FOREIGN KEY ([QuotationId]) REFERENCES [Quotations] ([QuotationId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE TABLE [DeliveryNotes] (
        [DeliveryNoteId] int NOT NULL IDENTITY,
        [PurchaseOrderId] int NOT NULL,
        [DeliveryDate] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(max) NOT NULL,
        [UpdatedDate] datetime2 NULL,
        [SupplierDSupplierId] int NULL,
        CONSTRAINT [PK_DeliveryNotes] PRIMARY KEY ([DeliveryNoteId]),
        CONSTRAINT [FK_DeliveryNotes_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([PurchaseOrderId]) ON DELETE CASCADE,
        CONSTRAINT [FK_DeliveryNotes_Suppliers_SupplierDSupplierId] FOREIGN KEY ([SupplierDSupplierId]) REFERENCES [Suppliers] ([SupplierId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE INDEX [IX_PurchaseRequests_SupplierDSupplierId] ON [PurchaseRequests] ([SupplierDSupplierId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE INDEX [IX_DeliveryNotes_PurchaseOrderId] ON [DeliveryNotes] ([PurchaseOrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE INDEX [IX_DeliveryNotes_SupplierDSupplierId] ON [DeliveryNotes] ([SupplierDSupplierId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrders_QuotationId] ON [PurchaseOrders] ([QuotationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE INDEX [IX_Quotations_RFQId] ON [Quotations] ([RFQId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE INDEX [IX_Quotations_SupplierId] ON [Quotations] ([SupplierId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE INDEX [IX_RFQs_PurchaseRequestId] ON [RFQs] ([PurchaseRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    CREATE INDEX [IX_RFQs_SupplierId] ON [RFQs] ([SupplierId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    ALTER TABLE [PurchaseRequests] ADD CONSTRAINT [FK_PurchaseRequests_Suppliers_SupplierDSupplierId] FOREIGN KEY ([SupplierDSupplierId]) REFERENCES [Suppliers] ([SupplierId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241019234507_Bonga'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241019234507_Bonga', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020042954_two'
)
BEGIN
    DECLARE @var35 sysname;
    SELECT @var35 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Quotations]') AND [c].[name] = N'UpdatedBy');
    IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Quotations] DROP CONSTRAINT [' + @var35 + '];');
    ALTER TABLE [Quotations] ALTER COLUMN [UpdatedBy] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020042954_two'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241020042954_two', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [FK_PurchaseOrders_Quotations_QuotationId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    DROP INDEX [IX_PurchaseOrders_QuotationId] ON [PurchaseOrders];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    DECLARE @var36 sysname;
    SELECT @var36 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'CreatedDate');
    IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var36 + '];');
    ALTER TABLE [PurchaseOrders] DROP COLUMN [CreatedDate];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    DECLARE @var37 sysname;
    SELECT @var37 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'QuotationId');
    IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var37 + '];');
    ALTER TABLE [PurchaseOrders] DROP COLUMN [QuotationId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    DECLARE @var38 sysname;
    SELECT @var38 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PurchaseOrders]') AND [c].[name] = N'UpdatedDate');
    IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [PurchaseOrders] DROP CONSTRAINT [' + @var38 + '];');
    ALTER TABLE [PurchaseOrders] DROP COLUMN [UpdatedDate];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    EXEC sp_rename N'[PurchaseOrders].[UpdatedBy]', N'SupplierName', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    EXEC sp_rename N'[PurchaseOrders].[OrderStatus]', N'QuotationNumber', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    EXEC sp_rename N'[PurchaseOrders].[OrderDate]', N'DateCreated', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    EXEC sp_rename N'[PurchaseOrders].[CreatedBy]', N'ItemDescription', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    ALTER TABLE [PurchaseOrders] ADD [Address] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    ALTER TABLE [PurchaseOrders] ADD [ContactNumber] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    ALTER TABLE [PurchaseOrders] ADD [QuotedPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020050415_PurchaseOrder'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241020050415_PurchaseOrder', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] DROP CONSTRAINT [FK_DeliveryNotes_PurchaseOrders_PurchaseOrderId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    DROP INDEX [IX_DeliveryNotes_PurchaseOrderId] ON [DeliveryNotes];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    DECLARE @var39 sysname;
    SELECT @var39 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DeliveryNotes]') AND [c].[name] = N'UpdatedDate');
    IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [DeliveryNotes] DROP CONSTRAINT [' + @var39 + '];');
    ALTER TABLE [DeliveryNotes] DROP COLUMN [UpdatedDate];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    EXEC sp_rename N'[DeliveryNotes].[UpdatedBy]', N'SupplierAddress', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    EXEC sp_rename N'[DeliveryNotes].[PurchaseOrderId]', N'Quantity', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    EXEC sp_rename N'[DeliveryNotes].[CreatedDate]', N'DateOfIssue', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    EXEC sp_rename N'[DeliveryNotes].[CreatedBy]', N'Status', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [DeliveryInstructions] nvarchar(500) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [ItemDescription] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [Notes] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [OrderNumber] nvarchar(50) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [PaymentTerms] nvarchar(200) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [RecipientAddress] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [RecipientName] nvarchar(100) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [SupplierName] nvarchar(100) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    ALTER TABLE [DeliveryNotes] ADD [UnitPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020054747_delivn'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241020054747_delivn', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020064145_three'
)
BEGIN
    DECLARE @var40 sysname;
    SELECT @var40 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DeliveryNotes]') AND [c].[name] = N'Notes');
    IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [DeliveryNotes] DROP CONSTRAINT [' + @var40 + '];');
    ALTER TABLE [DeliveryNotes] DROP COLUMN [Notes];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020064145_three'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241020064145_three', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020081604_four'
)
BEGIN
    DECLARE @var41 sysname;
    SELECT @var41 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DeliveryNotes]') AND [c].[name] = N'Status');
    IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [DeliveryNotes] DROP CONSTRAINT [' + @var41 + '];');
    ALTER TABLE [DeliveryNotes] DROP COLUMN [Status];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020081604_four'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241020081604_four', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020124340_testt'
)
BEGIN
    ALTER TABLE [RFQs] DROP CONSTRAINT [FK_RFQs_Suppliers_SupplierId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020124340_testt'
)
BEGIN
    ALTER TABLE [RFQs] ADD CONSTRAINT [FK_RFQs_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([SupplierId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020124340_testt'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241020124340_testt', N'8.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020144347_updatedSupplier'
)
BEGIN
    ALTER TABLE [Suppliers] ADD [CreatedBy] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020144347_updatedSupplier'
)
BEGIN
    ALTER TABLE [Suppliers] ADD [Password] nvarchar(256) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241020144347_updatedSupplier'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241020144347_updatedSupplier', N'8.0.8');
END;
GO

COMMIT;
GO


﻿BEGIN TRANSACTION;
GO

UPDATE [Account].[User] SET [CreatedOn] = '2021-07-27T08:41:39.6962598Z', [UpdatedOn] = '2021-07-27T08:41:39.6962859Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Account].[User] SET [CreatedOn] = '2021-07-27T08:41:39.6963970Z', [UpdatedOn] = '2021-07-27T08:41:39.6963973Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Account].[UserRole] SET [CreatedOn] = '2021-07-27T08:41:39.7097913Z', [UpdatedOn] = '2021-07-27T08:41:39.7098255Z'
WHERE [RoleId] = 1 AND [UserId] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Account].[UserRole] SET [CreatedOn] = '2021-07-27T08:41:39.7099539Z', [UpdatedOn] = '2021-07-27T08:41:39.7099541Z'
WHERE [RoleId] = 2 AND [UserId] = 2;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210727084142_Schoolmanagement00003', N'5.0.8');
GO

COMMIT;
GO


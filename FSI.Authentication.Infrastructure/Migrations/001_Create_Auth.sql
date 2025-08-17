-- Tabelas principais
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
  CREATE TABLE dbo.Users (
    UserId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    IsActive BIT NOT NULL DEFAULT(1),
    FailedLoginCount INT NOT NULL DEFAULT(0),
    LockoutEndUtc DATETIME2 NULL,
    ProfileName NVARCHAR(80) NOT NULL
  );
END

IF OBJECT_ID('dbo.UserPermissions', 'U') IS NULL
BEGIN
  CREATE TABLE dbo.UserPermissions (
    UserId UNIQUEIDENTIFIER NOT NULL,
    PermissionCode NVARCHAR(120) NOT NULL,
    PermissionDescription NVARCHAR(300) NULL,
    CONSTRAINT PK_UserPermissions PRIMARY KEY(UserId, PermissionCode),
    CONSTRAINT FK_UserPermissions_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE
  );
END

-- Outbox
IF OBJECT_ID('dbo.Outbox', 'U') IS NULL
BEGIN
  CREATE TABLE dbo.Outbox (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Type NVARCHAR(400) NOT NULL,
    Payload NVARCHAR(MAX) NOT NULL,
    OccurredOnUtc DATETIME2 NOT NULL,
    ProcessedOnUtc DATETIME2 NULL,
    Attempts INT NOT NULL DEFAULT(0)
  );
END

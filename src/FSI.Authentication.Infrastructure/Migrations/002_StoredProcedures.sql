-- GetByEmail
IF OBJECT_ID('dbo.usp_User_GetByEmail', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_User_GetByEmail;
GO
CREATE PROCEDURE dbo.usp_User_GetByEmail
  @Email NVARCHAR(256)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    u.UserId,
    u.Email,
    u.FirstName,
    u.LastName,
    u.PasswordHash,
    u.IsActive,
    u.FailedLoginCount,
    u.LockoutEndUtc,
    u.ProfileName,
    up.PermissionCode,
    up.PermissionDescription
  FROM dbo.Users u
  LEFT JOIN dbo.UserPermissions up ON up.UserId = u.UserId
  WHERE u.Email = @Email;
END
GO

-- Insert
IF OBJECT_ID('dbo.usp_User_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_User_Insert;
GO
CREATE PROCEDURE dbo.usp_User_Insert
  @UserId UNIQUEIDENTIFIER,
  @Email NVARCHAR(256),
  @FirstName NVARCHAR(100),
  @LastName NVARCHAR(100) = NULL,
  @PasswordHash NVARCHAR(500),
  @IsActive BIT,
  @ProfileName NVARCHAR(80)
AS
BEGIN
  SET NOCOUNT ON;

  INSERT dbo.Users(UserId, Email, FirstName, LastName, PasswordHash, IsActive, ProfileName)
  VALUES(@UserId, @Email, @FirstName, @LastName, @PasswordHash, @IsActive, @ProfileName);
END
GO

-- Update (atualiza todos os campos relevantes)
IF OBJECT_ID('dbo.usp_User_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_User_Update;
GO
CREATE PROCEDURE dbo.usp_User_Update
  @UserId UNIQUEIDENTIFIER,
  @Email NVARCHAR(256),
  @FirstName NVARCHAR(100),
  @LastName NVARCHAR(100) = NULL,
  @PasswordHash NVARCHAR(500),
  @IsActive BIT,
  @ProfileName NVARCHAR(80),
  @FailedLoginCount INT,
  @LockoutEndUtc DATETIME2 = NULL
AS
BEGIN
  SET NOCOUNT ON;

  UPDATE dbo.Users
     SET Email = @Email,
         FirstName = @FirstName,
         LastName = @LastName,
         PasswordHash = @PasswordHash,
         IsActive = @IsActive,
         ProfileName = @ProfileName,
         FailedLoginCount = @FailedLoginCount,
         LockoutEndUtc = @LockoutEndUtc
   WHERE UserId = @UserId;
END
GO

-- Outbox
IF OBJECT_ID('dbo.usp_Outbox_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Outbox_Insert;
GO
CREATE PROCEDURE dbo.usp_Outbox_Insert
  @Id UNIQUEIDENTIFIER,
  @Type NVARCHAR(400),
  @Payload NVARCHAR(MAX),
  @OccurredOnUtc DATETIME2
AS
BEGIN
  SET NOCOUNT ON;
  INSERT dbo.Outbox(Id, Type, Payload, OccurredOnUtc) VALUES (@Id, @Type, @Payload, @OccurredOnUtc);
END
GO

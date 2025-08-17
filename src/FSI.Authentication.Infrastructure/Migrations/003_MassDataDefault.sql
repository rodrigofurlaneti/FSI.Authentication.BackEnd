-- 003_MassDataDefault.sql
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRAN;

    DECLARE @Email         NVARCHAR(256) = N'admin@empresa.local';
    DECLARE @FirstName     NVARCHAR(100) = N'admin';
    DECLARE @LastName      NVARCHAR(100) = N'system';
    DECLARE @ProfileName   NVARCHAR(80)  = N'Administrador';
    -- PBKDF2-HMAC-SHA256, 100.000 iterações (senha: Mudar@12345)
    DECLARE @PasswordHash  NVARCHAR(500) = N'$PBKDF2$HMACSHA256$100000$yLydny4tS+A1W/g7StDBTg==$R/87EzSxnXdGI+WzzG/IsB7LbX18GGo5BPnmiM2PV3c=';
    DECLARE @UserId        UNIQUEIDENTIFIER;

    -- Upsert do usuário admin (sem CreatedAtUtc/UpdatedAtUtc)
    IF EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email)
    BEGIN
        SELECT @UserId = UserId FROM dbo.Users WHERE Email = @Email;

        UPDATE dbo.Users
           SET FirstName        = @FirstName,
               LastName         = @LastName,
               PasswordHash     = @PasswordHash,
               IsActive         = 1,
               FailedLoginCount = 0,
               LockoutEndUtc    = NULL,
               ProfileName      = @ProfileName
         WHERE UserId = @UserId;
    END
    ELSE
    BEGIN
        SET @UserId = NEWID();

        INSERT dbo.Users
        (
            UserId, Email, FirstName, LastName, PasswordHash,
            IsActive, FailedLoginCount, LockoutEndUtc, ProfileName
        )
        VALUES
        (
            @UserId, @Email, @FirstName, @LastName, @PasswordHash,
            1, 0, NULL, @ProfileName
        );
    END

    /* ---- Permissões (opcional): cria a tabela se não existir ---- */
    IF OBJECT_ID('dbo.UserPermissions', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.UserPermissions
        (
            UserId UNIQUEIDENTIFIER NOT NULL,
            PermissionCode NVARCHAR(100) NOT NULL,
            PermissionDescription NVARCHAR(200) NULL,
            CONSTRAINT PK_UserPermissions PRIMARY KEY (UserId, PermissionCode),
            CONSTRAINT FK_UserPermissions_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
        );
    END

    ;WITH P(Code, Description) AS (
        SELECT N'users.read',         N'Ler usuários'          UNION ALL
        SELECT N'users.write',        N'Gerenciar usuários'    UNION ALL
        SELECT N'auth.token.issue',   N'Emitir tokens'         UNION ALL
        SELECT N'auth.profile.read',  N'Ler perfil'            UNION ALL
        SELECT N'admin.panel',        N'Acessar painel admin'  UNION ALL
        SELECT N'admin.users',        N'Administrar usuários'  UNION ALL
        SELECT N'admin.roles',        N'Administrar perfis'
    )
    INSERT dbo.UserPermissions (UserId, PermissionCode, PermissionDescription)
    SELECT @UserId, P.Code, P.Description
    FROM P
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.UserPermissions UP
        WHERE UP.UserId = @UserId
          AND UP.PermissionCode = P.Code
    );

    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    THROW;
END CATCH;

-- Conferência
SELECT TOP (1) U.UserId, U.Email, U.ProfileName, U.IsActive
FROM dbo.Users U
WHERE U.Email = N'admin@empresa.local';

SELECT UP.PermissionCode
FROM dbo.UserPermissions UP
JOIN dbo.Users U ON U.UserId = UP.UserId
WHERE U.Email = N'admin@empresa.local'
ORDER BY UP.PermissionCode;

/* 1) (Opcional) Cria o banco se ainda n�o existir */
IF DB_ID(N'AuthDb') IS NULL
BEGIN
    PRINT 'Criando banco AuthDb...';
    CREATE DATABASE [AuthDb];
END
GO

/* 2) Cria ou atualiza o LOGIN a n�vel de SERVIDOR */
USE [master];
GO

IF SUSER_ID(N'app_fsi') IS NULL
BEGIN
    PRINT 'Criando login [app_fsi]...';
    CREATE LOGIN [app_fsi]
        WITH PASSWORD = N'SenhaForte!123',
             CHECK_POLICY = ON,       -- aplica pol�tica de complexidade do Windows
             CHECK_EXPIRATION = OFF;  -- n�o expira (ajuste se quiser expirar)
END
ELSE
BEGIN
    PRINT 'Login [app_fsi] j� existe. Atualizando senha/pol�ticas...';
    ALTER LOGIN [app_fsi]
        WITH PASSWORD = N'SenhaForte!123',
             CHECK_POLICY = ON,
             CHECK_EXPIRATION = OFF;
END
GO

/* 3) Cria o USER no banco e adiciona ao db_owner (permiss�o total no DB) */
USE [AuthDb];
GO

IF USER_ID(N'app_fsi') IS NULL
BEGIN
    PRINT 'Criando usu�rio [app_fsi] no banco AuthDb...';
    CREATE USER [app_fsi] FOR LOGIN [app_fsi] WITH DEFAULT_SCHEMA = [dbo];
END
ELSE
BEGIN
    PRINT 'Usu�rio [app_fsi] j� existe no banco AuthDb.';
END
GO

-- Garante que � membro de db_owner (full no banco)
IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    JOIN sys.database_principals r ON r.principal_id = drm.role_principal_id AND r.name = N'db_owner'
    JOIN sys.database_principals u ON u.principal_id = drm.member_principal_id AND u.name = N'app_fsi'
)
BEGIN
    PRINT 'Adicionando [app_fsi] ao papel db_owner...';
    ALTER ROLE [db_owner] ADD MEMBER [app_fsi];
END
ELSE
BEGIN
    PRINT '[app_fsi] j� � db_owner.';
END
GO

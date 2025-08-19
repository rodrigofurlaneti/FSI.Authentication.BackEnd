-- 1. Tabela de log de geolocalização do cliente
IF OBJECT_ID('dbo.GeoClientLog', 'U') IS NULL
BEGIN
  CREATE TABLE dbo.GeoClientLog (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),

    -- GEO
    Lat FLOAT NULL,
    Lon FLOAT NULL,
    AccuracyMeters FLOAT NULL,
    AltitudeMeters FLOAT NULL,
    AltitudeAccuracyMeters FLOAT NULL,
    SpeedMps FLOAT NULL,
    HeadingDegrees FLOAT NULL,
    TsEpochMs BIGINT NULL,
    City NVARCHAR(200) NULL,

    -- ENV
    UserAgent NVARCHAR(1024) NULL,
    Language NVARCHAR(32) NULL,
    Languages NVARCHAR(512) NULL,
    Platform NVARCHAR(128) NULL,
    IsOnline BIT NULL,
    TimeZone NVARCHAR(128) NULL,
    ScreenWidth INT NULL,
    ScreenHeight INT NULL,
    DevicePixelRatio FLOAT NULL,
    Referrer NVARCHAR(1000) NULL,
    PageUrl NVARCHAR(1000) NULL,

    ConnectionEffectiveType NVARCHAR(32) NULL,
    ConnectionDownlinkMbps FLOAT NULL,
    ConnectionRttMs INT NULL,
    ConnectionSaveData BIT NULL,

    -- Error (se client reportar)
    Error NVARCHAR(4000) NULL,

    CONSTRAINT PK_GeoClientLog PRIMARY KEY (Id)
  );
END;
GO;
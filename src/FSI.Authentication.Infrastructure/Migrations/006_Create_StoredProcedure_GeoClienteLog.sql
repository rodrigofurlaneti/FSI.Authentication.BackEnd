-- 2. Stored Procedure de insert
IF OBJECT_ID('dbo.usp_GeoLog_Insert', 'P') IS NOT NULL
  DROP PROCEDURE dbo.usp_GeoLog_Insert;
GO

CREATE PROCEDURE dbo.usp_GeoLog_Insert
  @Lat FLOAT = NULL,
  @Lon FLOAT = NULL,
  @AccuracyMeters FLOAT = NULL,
  @AltitudeMeters FLOAT = NULL,
  @AltitudeAccuracyMeters FLOAT = NULL,
  @SpeedMps FLOAT = NULL,
  @HeadingDegrees FLOAT = NULL,
  @TsEpochMs BIGINT = NULL,
  @City NVARCHAR(200) = NULL,

  @UserAgent NVARCHAR(1024) = NULL,
  @Language NVARCHAR(32) = NULL,
  @Languages NVARCHAR(512) = NULL,
  @Platform NVARCHAR(128) = NULL,
  @IsOnline BIT = NULL,
  @TimeZone NVARCHAR(128) = NULL,
  @ScreenWidth INT = NULL,
  @ScreenHeight INT = NULL,
  @DevicePixelRatio FLOAT = NULL,
  @Referrer NVARCHAR(1000) = NULL,
  @PageUrl NVARCHAR(1000) = NULL,

  @ConnectionEffectiveType NVARCHAR(32) = NULL,
  @ConnectionDownlinkMbps FLOAT = NULL,
  @ConnectionRttMs INT = NULL,
  @ConnectionSaveData BIT = NULL,

  @Error NVARCHAR(4000) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  INSERT INTO dbo.GeoClientLog (
    Lat, Lon, AccuracyMeters, AltitudeMeters, AltitudeAccuracyMeters, SpeedMps, HeadingDegrees, TsEpochMs, City,
    UserAgent, Language, Languages, Platform, IsOnline, TimeZone, ScreenWidth, ScreenHeight, DevicePixelRatio, Referrer, PageUrl,
    ConnectionEffectiveType, ConnectionDownlinkMbps, ConnectionRttMs, ConnectionSaveData,
    Error
  )
  VALUES (
    @Lat, @Lon, @AccuracyMeters, @AltitudeMeters, @AltitudeAccuracyMeters, @SpeedMps, @HeadingDegrees, @TsEpochMs, @City,
    @UserAgent, @Language, @Languages, @Platform, @IsOnline, @TimeZone, @ScreenWidth, @ScreenHeight, @DevicePixelRatio, @Referrer, @PageUrl,
    @ConnectionEffectiveType, @ConnectionDownlinkMbps, @ConnectionRttMs, @ConnectionSaveData,
    @Error
  );
END
GO
USE [PstBackup]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* Tables creation

CREATE TABLE [dbo].[tbClients]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Version] VARCHAR(15) NOT NULL, 
    [ComputerName] NVARCHAR(64) NOT NULL, 
    [UserName] NVARCHAR(32) NOT NULL, 
    [LastContactDate] DATETIME NOT NULL
)
CREATE TABLE [dbo].[tbPstFiles]
(
	[ClientId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [LocalPath] NVARCHAR(300) NOT NULL, 
    [FileId] UNIQUEIDENTIFIER NOT NULL, 
    [IsSetToBackup] BIT NOT NULL, 
    [Size] BIGINT NOT NULL, 
    [LastSuccessfulBackup] DATETIME NULL
)
CREATE TABLE [dbo].[tbBackupSessions]
(
	[FileId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [RemotePath] NVARCHAR(300) NOT NULL, 
    [IsCompressed] BIT NOT NULL, 
    [BackupMethod] INT NOT NULL, 
    [IsSchedule] BIT NOT NULL, 
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME NOT NULL, 
    [ChunkCount] INT NOT NULL, 
    [ErrorCode] INT NOT NULL, 
    [ErrorMessage] NVARCHAR(300) NOT NULL
)
*/

SELECT * FROM tbClients ORDER BY LastContactDate DESC;
SELECT * FROM tbPstFiles ORDER BY ClientID;
SELECT * FROM tbBackupSessions ORDER BY FileID, StartTime;



/* Delete tables content

DELETE FROM tbClients;
DELETE FROM tbPstFiles;
DELETE FROM tbBackupSessions;
*/
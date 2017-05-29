USE [PstBackup]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* 
DELETE FROM tbClients;
DELETE FROM tbPstFiles;
DELETE FROM tbBackupSessions;
*/

SELECT * FROM tbClients ORDER BY LastContactDate DESC;
SELECT * FROM tbPstFiles ORDER BY Size;
SELECT * FROM tbBackupSessions ORDER BY FileId,StartTime;
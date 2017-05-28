USE [Test-PstBackup]
GO

/****** Objet : Table [dbo].[tbClients] Date du script : 28/05/2017 16:30:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- DELETE FROM tbClients;
SELECT * FROM tbClients ORDER BY LastContactDate DESC;

-- DELETE FROM tbPstFiles;
SELECT * FROM tbPstFiles ORDER BY Size;

-- DELETE FROM tbBackupSessions;
SELECT * FROM tbBackupSessions ORDER BY FileId;
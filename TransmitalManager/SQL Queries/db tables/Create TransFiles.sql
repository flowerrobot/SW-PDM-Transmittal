CREATE TABLE [dbo].[TransFiles]
(
	[Id] INT NOT NULL PRIMARY KEY, 
	[TransId] INT,
    [FileName] NVARCHAR(MAX) NULL, 
	[FileId] INT NOT NULL,
    [Version] INT NULL, 
    [Revision] NCHAR(1) NULL

)

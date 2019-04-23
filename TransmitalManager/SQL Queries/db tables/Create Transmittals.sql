
    CREATE TABLE[dbo].[Transmittals]
    (

    [Id] INT NOT NULL PRIMARY KEY, 
    [ProjectNo] INT NOT NULL, 
    [Recipients] NVARCHAR(MAX) NULL, 
    [SentDate]
    DATE NULL,
    [IssueBy] NCHAR(10) NULL, 
    [CreatedBy] NCHAR(10) NULL, 
    [Comments] NVARCHAR(MAX) NULL, 
    [IssueType]
    INT NULL,
    [Status] INT NULL,
    [ToWorkShop] INT NULL
	)
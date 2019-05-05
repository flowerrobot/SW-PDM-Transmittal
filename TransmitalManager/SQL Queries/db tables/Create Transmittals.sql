
    CREATE TABLE[dbo].[Transmittals]
    (

    [Id] INT NOT NULL PRIMARY KEY Identity(1,1), 
    [ProjectNo] INT NOT NULL, 
    [Recipients] NVARCHAR(MAX) NULL, 
    [SentDate] DATE NULL,
    [IssueBy] int NULL, 
    [CreatedBy] int NULL, 
    [Comments] NVARCHAR(MAX) NULL, 
    [IssueType] INT NULL,
    [Status] INT NULL,
    [ToWorkShop] INT NULL
	)

    CREATE TABLE[dbo].[Recipients]
    (

    [Id] INT NOT NULL PRIMARY KEY, 
    [CompanyId] INT NOT NULL, 
    [Name] NVARCHAR(MAX) NULL, 
    [Email] NVARCHAR(MAX) NULL
    
	)
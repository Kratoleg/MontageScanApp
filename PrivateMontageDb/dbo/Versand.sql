CREATE TABLE [dbo].[Versand]
(
	[VersandId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [LieferscheinId] INT NOT NULL, 
    [VersandTS] DATETIME NOT NULL 
    foreign key ([LieferscheinId]) 
		references Lieferschein([LieferscheinId])
	
)

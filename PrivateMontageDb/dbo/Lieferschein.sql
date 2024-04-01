CREATE TABLE [dbo].[Lieferschein]
(
	[LieferscheinId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [lieferschein] NCHAR(7) NOT NULL, 
    [EingangsTS] DATETIME NOT NULL
)

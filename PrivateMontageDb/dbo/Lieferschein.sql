CREATE TABLE [dbo].[Lieferschein]
(
	[LieferscheinId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Lieferschein] NCHAR(7) NOT NULL, 
    [EingangsTS] DATETIME NOT NULL, 
    [Storniert] BIT NULL
)

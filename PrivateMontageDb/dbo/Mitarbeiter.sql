CREATE TABLE [dbo].[Mitarbeiter]
(
	[MitarbeiterId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ChipId] VARCHAR(20) NOT NULL, 
    [Vorname] VARCHAR(30) NOT NULL, 
    [Nachname] VARCHAR(30) NOT NULL
)

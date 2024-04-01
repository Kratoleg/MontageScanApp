CREATE TABLE [dbo].[Montage]
(
	[MontageId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [MitarbeiterId] INT NOT NULL, 
    [LieferscheinId] INT NOT NULL, 
    [MontageTS] DATETIME NOT NULL

    foreign key ([LieferscheinId]) 
		references Lieferschein([LieferscheinId])
        foreign key ([MitarbeiterId]) 
		references Mitarbeiter([MitarbeiterId])
)

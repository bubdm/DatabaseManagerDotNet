CREATE TABLE [_DatabaseSettings]
( 
   [Id]    INTEGER PRIMARY KEY ASC ON CONFLICT ROLLBACK AUTOINCREMENT,
   [Name]  TEXT    NOT NULL ON CONFLICT ROLLBACK UNIQUE ON CONFLICT ROLLBACK,
   [Value] TEXT    NULL
);

GO

INSERT INTO [_DatabaseSettings] ([Name], [Value]) VALUES ('Database.Version', '0');
INSERT INTO [_DatabaseSettings] ([Name], [Value]) VALUES ('TestValue', 'xyz');

GO
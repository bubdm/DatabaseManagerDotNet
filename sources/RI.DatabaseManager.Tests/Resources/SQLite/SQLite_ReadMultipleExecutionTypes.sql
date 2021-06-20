/* DBMANAGER:ExecutionType=Scalar */
SELECT [Value] FROM [_DatabaseSettings] WHERE [Name] = 'TestValue';

GO

/* DBMANAGER:ExecutionType=Scalar */
SELECT [Value] FROM [_DatabaseSettings] WHERE [Name] = 'Database.Version';

GO

/* DBMANAGER:ExecutionType=Scalar */
SELECT count(*) FROM [_DatabaseSettings];

GO

/* DBMANAGER:ExecutionType=NonQuery */
SELECT count(*) FROM [_DatabaseSettings];

GO

/* DBMANAGER:ExecutionType=NonQuery */
INSERT INTO [_DatabaseSettings] ([Name], [Value]) VALUES ('TestValue2', 'abc');

GO

/* DBMANAGER:ExecutionType=Reader */
SELECT * FROM [_DatabaseSettings];

GO

/* DBMANAGER:ExecutionType=Reader */
SELECT [Value] FROM [_DatabaseSettings];
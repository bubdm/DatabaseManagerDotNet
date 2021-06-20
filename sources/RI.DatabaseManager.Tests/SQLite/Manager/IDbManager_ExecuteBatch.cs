using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Manager
{
    public sealed class IDbManager_ExecuteBatch
    {
        [Fact]
        public static void ExecuteBatch_CreateTable ()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> dbManager;

            // Act

            dbManager = new DbManagerBuilder().UseSQLite(options =>
                                              {
                                                  options.ConnectionString.DataSource = tempFile.FullPath;
                                              }).UseAssemblyScriptBatches()
                                              .BuildDbManager();

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = dbManager.GetBatch("SQLite_CreateSettingsTable");

            dbManager.Initialize();

            int versionAfterInit = dbManager.Version;
            DbState stateAfterInit = dbManager.State;

            bool result = dbManager.ExecuteBatch(batch, false, true);

            int versionAfterExecution = dbManager.Version;
            DbState stateAfterExecution = dbManager.State;

            // Assert

            Assert.Equal(0, versionAfterInit);
            Assert.Equal(DbState.New, stateAfterInit);

            Assert.Equal(123, versionAfterExecution);
            Assert.Equal(DbState.TooNew, stateAfterExecution);

            Assert.True(result);

            Assert.NotNull(batch);
            Assert.Equal(2, batch.Commands.Count);

            Assert.Null(batch.Commands[0].Code);
            Assert.NotNull(batch.Commands[0].Script);
            Assert.NotEmpty(batch.Commands[0].Script);

            Assert.Null(batch.Commands[1].Code);
            Assert.NotNull(batch.Commands[1].Script);
            Assert.NotEmpty(batch.Commands[1].Script);
        }

        [Fact]
        public static void ExecuteBatch_ReadTableOne ()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> dbManager;

            // Act

            dbManager = new DbManagerBuilder().UseSQLite(options =>
                                              {
                                                  options.ConnectionString.DataSource = tempFile.FullPath;
                                              }).UseAssemblyScriptBatches()
                                              .BuildDbManager();

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> create = dbManager.GetBatch("SQLite_CreateSettingsTable");
            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> read = dbManager.GetBatch("SQLite_ReadSettingsTableOne");

            dbManager.Initialize();

            bool createResult = dbManager.ExecuteBatch(create, false, true);
            bool readResult = dbManager.ExecuteBatch(read, false, true);

            // Assert

            Assert.True(createResult);
            Assert.True(readResult);
            Assert.Equal("xyz", read.GetLastScalar<string>());
        }

        [Fact]
        public static void ExecuteBatch_ReadTableMultiple ()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> dbManager;

            // Act

            dbManager = new DbManagerBuilder().UseSQLite(options =>
                                              {
                                                  options.ConnectionString.DataSource = tempFile.FullPath;
                                              }).UseAssemblyScriptBatches()
                                              .BuildDbManager();

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> create = dbManager.GetBatch("SQLite_CreateSettingsTable");
            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> read = dbManager.GetBatch("SQLite_ReadSettingsTableMultiple");

            dbManager.Initialize();

            bool createResult = dbManager.ExecuteBatch(create, false, true);
            bool readResult = dbManager.ExecuteBatch(read, false, true);

            List<object> results = read.GetAllResults();

            // Assert

            Assert.True(createResult);
            Assert.True(readResult);
            
            Assert.Equal(4, results.Count);
            Assert.Equal("xyz", results[0]);
            Assert.Equal(123, Convert.ToInt32(results[1]));
            Assert.Equal(123, Convert.ToInt32(results[2]));
            Assert.Equal("xyz", results[3]);
        }

        [Fact]
        public static void ExecuteBatch_MultipleExecutionTypes ()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> dbManager;

            // Act

            dbManager = new DbManagerBuilder().UseSQLite(options =>
                                              {
                                                  options.ConnectionString.DataSource = tempFile.FullPath;
                                              }).UseAssemblyScriptBatches()
                                              .BuildDbManager();

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> create = dbManager.GetBatch("SQLite_CreateSettingsTable");
            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> read = dbManager.GetBatch("SQLite_ReadMultipleExecutionTypes");

            dbManager.Initialize();

            bool createResult = dbManager.ExecuteBatch(create, false, true);
            bool readResult = dbManager.ExecuteBatch(read, false, true);

            // Assert

            Assert.True(createResult);
            Assert.True(readResult);

            Assert.Equal("xyz", read.Commands[0].Results[0]);
            Assert.Equal(DbBatchExecutionType.Scalar, read.Commands[0].ExecutionType);
            
            Assert.Equal(123, Convert.ToInt32(read.Commands[1].Results[0]));
            Assert.Equal(DbBatchExecutionType.Scalar, read.Commands[1].ExecutionType);

            Assert.Equal(2, Convert.ToInt32(read.Commands[2].Results[0]));
            Assert.Equal(DbBatchExecutionType.Scalar, read.Commands[2].ExecutionType);

            Assert.Equal(-1, Convert.ToInt32(read.Commands[3].Results[0]));
            Assert.Equal(DbBatchExecutionType.NonQuery, read.Commands[3].ExecutionType);

            Assert.Equal(1, Convert.ToInt32(read.Commands[4].Results[0]));
            Assert.Equal(DbBatchExecutionType.NonQuery, read.Commands[4].ExecutionType);

            Assert.Equal(1, Convert.ToInt32(read.Commands[5].Results[0]));
            Assert.Equal("Database.Version", read.Commands[5].Results[1]);
            Assert.Equal(123, Convert.ToInt32(read.Commands[5].Results[2]));
            Assert.Equal(2, Convert.ToInt32(read.Commands[5].Results[3]));
            Assert.Equal("TestValue", read.Commands[5].Results[4]);
            Assert.Equal("xyz", read.Commands[5].Results[5]);
            Assert.Equal(3, Convert.ToInt32(read.Commands[5].Results[6]));
            Assert.Equal("TestValue2", read.Commands[5].Results[7]);
            Assert.Equal("abc", read.Commands[5].Results[8]);
            Assert.Equal(DbBatchExecutionType.Reader, read.Commands[5].ExecutionType);

            Assert.Equal("123", read.Commands[6].Results[0]);
            Assert.Equal("xyz", read.Commands[6].Results[1]);
            Assert.Equal("abc", read.Commands[6].Results[2]);
            Assert.Equal(DbBatchExecutionType.Reader, read.Commands[6].ExecutionType);
        }
    }
}

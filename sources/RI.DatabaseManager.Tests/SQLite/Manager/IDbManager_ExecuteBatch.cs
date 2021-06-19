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

            bool result = dbManager.ExecuteBatch(batch, false, true);

            // Assert

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
        }
    }
}

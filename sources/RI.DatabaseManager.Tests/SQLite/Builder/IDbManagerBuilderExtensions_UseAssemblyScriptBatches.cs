using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Builder
{
    public sealed class IDbManagerBuilderExtensions_UseAssemblyScriptBatches
    {
        [Fact]
        public static void UseAssemblyScriptBatches_FromTestAssembly_Success()
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

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = dbManager.GetBatch("TestScript1");

            // Assert

            Assert.NotNull(batch);
            Assert.Equal(2, batch.Commands.Count);

            Assert.Null(batch.Commands[0].Code);
            Assert.NotNull(batch.Commands[0].Script);
            Assert.NotEmpty(batch.Commands[0].Script);
            Assert.Equal("Command1", batch.Commands[0].Script);

            Assert.Null(batch.Commands[1].Code);
            Assert.NotNull(batch.Commands[1].Script);
            Assert.NotEmpty(batch.Commands[1].Script);
            Assert.Equal("Command2", batch.Commands[1].Script);
        }
    }
}

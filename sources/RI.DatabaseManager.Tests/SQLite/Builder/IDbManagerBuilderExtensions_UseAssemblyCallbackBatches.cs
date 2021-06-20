using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Builder
{
    public sealed class IDbManagerBuilderExtensions_UseAssemblyCallbackBatches
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
                                              }).UseAssemblyCallbackBatches()
                                              .BuildDbManager();

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch1 = dbManager.GetBatch("TestCallback1");
            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch2 = dbManager.GetBatch("CBx");

            // Assert

            Assert.NotNull(batch1);
            Assert.NotNull(batch2);

            Assert.Equal(1, batch1.Commands.Count);
            Assert.Equal(1, batch2.Commands.Count);

            Assert.Null(batch1.Commands[0].Script);
            Assert.Null(batch2.Commands[0].Script);
            
            
            Assert.NotNull(batch1.Commands[0].Code);
            Assert.Null(batch1.Commands[0].IsolationLevel);
            Assert.Equal(DbBatchTransactionRequirement.DontCare, batch1.Commands[0].TransactionRequirement);
            Assert.Equal(DbBatchExecutionType.Reader, batch1.Commands[0].ExecutionType);


            Assert.NotNull(batch2.Commands[0].Code);
            Assert.Equal(IsolationLevel.Chaos, batch2.Commands[0].IsolationLevel);
            Assert.Equal(DbBatchTransactionRequirement.Disallowed, batch2.Commands[0].TransactionRequirement);
            Assert.Equal(DbBatchExecutionType.Reader, batch2.Commands[0].ExecutionType);
        }
    }
}

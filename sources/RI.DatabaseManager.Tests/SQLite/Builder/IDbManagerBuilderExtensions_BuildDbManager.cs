using System.Data;
using System.Data.SQLite;
using System.Reflection;

using RI.Abstractions.Builder;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Builder
{
    public sealed class IDbManagerBuilderExtensions_BuildDbManager
    {
        [Fact]
        public static void BuildDbManager_NoBatchLocator_BuilderException()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> dbManager = null;

            // Act + Assert

            Assert.Throws<BuilderException>(() =>
            {


                dbManager = new DbManagerBuilder().UseSQLite(options =>
                                                  {
                                                      options.ConnectionString.DataSource = tempFile.FullPath;
                                                  })
                                                  .BuildDbManager();
            });
        }

        [Fact]
        public static void BuildDbManager_WithBatchLocator_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> dbManager = null;

            // Act

            dbManager = new DbManagerBuilder().UseSQLite(options =>
                                              {
                                                  options.ConnectionString.DataSource = tempFile.FullPath;
                                              }).UseAssemblyScriptBatches(assemblies: Assembly.GetExecutingAssembly())
                                              .BuildDbManager();

            // Assert

            Assert.Equal(DbState.Uninitialized, dbManager.State);
        }
    }
}

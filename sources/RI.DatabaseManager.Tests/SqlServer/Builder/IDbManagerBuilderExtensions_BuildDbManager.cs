using System.Data;
using System.Reflection;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Builder;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SqlServer.Builder
{
    public sealed class IDbManagerBuilderExtensions_BuildDbManager
    {
        [Fact]
        public static void SqlServer_NoBatchLocator_BuilderException()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            // Act + Assert

            Assert.Throws<BuilderException>(() =>
            {


                new DbManagerBuilder().UseSqlServer(options =>
                                                                                                       {
                                                                                                           options.ConnectionString.DataSource = tempFile.FullPath;
                                                                                                       })
                                                                                                       .BuildDbManager();
            });
        }

        [Fact]
        public static void SqlServer_WithBatchLocator_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SqlConnection, SqlTransaction, SqlDbType> dbManager;

            // Act

            dbManager = new DbManagerBuilder().UseSqlServer(options =>
                                              {
                                                  options.ConnectionString.DataSource = tempFile.FullPath;
                                              }).UseAssemblyScriptBatches(assemblies: Assembly.GetExecutingAssembly())
                                              .BuildDbManager();

            // Assert

            Assert.Equal(DbState.Uninitialized, dbManager.State);
        }
    }
}

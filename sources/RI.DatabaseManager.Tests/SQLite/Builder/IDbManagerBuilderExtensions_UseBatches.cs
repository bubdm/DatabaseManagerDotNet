using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Reflection;

using RI.Abstractions.Builder;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Builder
{
    public sealed class IDbManagerBuilderExtensions_UseBatches
    {
        [Fact]
        public static void UseBatches_Empty_SuccessNoBatchFound()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> dbManager = null;

            // Act

            dbManager = new DbManagerBuilder().UseSQLite(options =>
                                              {
                                                  options.ConnectionString.DataSource = tempFile.FullPath;
                                              }).UseBatches(dictionary => { })
                                              .BuildDbManager();

            // Assert

            Assert.Null(dbManager.GetBatch("Test"));
        }

        [Fact]
        public static void UseBatches_MultipleBatches_SuccessBatchesFound()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> dbManager = null;

            // Act

            dbManager = new DbManagerBuilder().UseSQLite(options =>
                                              {
                                                  options.ConnectionString.DataSource = tempFile.FullPath;
                                              }).UseBatches(dictionary =>
                                              {
                                                  dictionary.AddCode("Test1", (SQLiteConnection connection, SQLiteTransaction transaction, IDbBatchCommandParameterCollection<DbType> parameters, out string error, out Exception exception) =>
                                                  {
                                                      error = null;
                                                      exception = null;
                                                      return null;
                                                  });

                                                  dictionary.AddScript("Test2", "Script");

                                                  dictionary.AddScriptFromAssemblyResource("Test3", Assembly.GetExecutingAssembly(), "RI.DatabaseManager.Tests.Resources.TestScript1.sql");
                                              })
                                              .BuildDbManager();

            ISet<string> names = dbManager.GetBatchNames();
            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch1 = dbManager.GetBatch("Test1");
            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch2 = dbManager.GetBatch("Test2");
            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch3 = dbManager.GetBatch("Test3");

            // Assert

            Assert.Null(dbManager.GetBatch("Test"));

            Assert.NotNull(batch1);
            Assert.NotNull(batch2);
            Assert.NotNull(batch3);

            Assert.Equal(1, batch1.Commands.Count);
            Assert.Equal(1, batch2.Commands.Count);
            Assert.Equal(2, batch3.Commands.Count);

            Assert.Null(batch1.Commands[0].Script);
            Assert.NotNull(batch1.Commands[0].Code);

            Assert.Null(batch2.Commands[0].Code);
            Assert.NotNull(batch2.Commands[0].Script);
            Assert.NotEmpty(batch2.Commands[0].Script);
            Assert.Equal("Script", batch2.Commands[0].Script);

            Assert.Null(batch3.Commands[0].Code);
            Assert.NotNull(batch3.Commands[0].Script);
            Assert.NotEmpty(batch3.Commands[0].Script);
            Assert.Equal("Command1", batch3.Commands[0].Script);
            Assert.NotNull(batch3.Commands[1].Script);
            Assert.NotEmpty(batch3.Commands[1].Script);
            Assert.Equal("Command2", batch3.Commands[1].Script);
        }
    }
}

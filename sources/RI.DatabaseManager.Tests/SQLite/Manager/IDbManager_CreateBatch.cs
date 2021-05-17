using System;
using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Manager
{
    public sealed class IDbManager_CreateBatch
    {
        [Fact]
        public static void CreateBatch_EmptyBatch_CorrectState()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            // Assert

            Assert.NotNull(batch);

            Assert.Null(batch.GetError());
            Assert.Null(batch.GetException());
            Assert.Null(batch.GetResult());
            Assert.Null(batch.GetRequiredIsolationLevel());

            Assert.Empty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.Empty(batch.GetErrors());
            Assert.Empty(batch.GetExceptions());
            Assert.Empty(batch.GetResults());

            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.True(batch.IsEmpty());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_Clear()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test");
            batch.AddScript("Test");

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            batch.Clear();
            
            Assert.Empty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.True(batch.IsEmpty());

            Assert.Null(batch.GetRequiredIsolationLevel());
            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectTransactionRequirement1()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, null);
            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, null);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Null(batch.GetRequiredIsolationLevel());
            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectTransactionRequirement2()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, null);
            batch.AddScript("Test", DbBatchTransactionRequirement.Required, null);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Null(batch.GetRequiredIsolationLevel());
            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.True(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectTransactionRequirement3()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, null);
            batch.AddScript("Test", DbBatchTransactionRequirement.Disallowed, null);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Null(batch.GetRequiredIsolationLevel());
            Assert.True(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectTransactionRequirement4()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.Required, null);
            batch.AddScript("Test", DbBatchTransactionRequirement.Disallowed, null);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Null(batch.GetRequiredIsolationLevel());
            Assert.False(batch.HasFailed());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());

            Assert.Throws<InvalidOperationException>(() =>
            {
                var value = batch.RequiresTransaction();
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                var value = batch.DisallowsTransaction();
            });
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectTransactionRequirement5()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.Required, null);
            batch.AddScript("Test", DbBatchTransactionRequirement.Required, null);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Null(batch.GetRequiredIsolationLevel());
            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.True(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectIsolationLevel1()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, null);
            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, null);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Null(batch.GetRequiredIsolationLevel());
            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectIsolationLevel2()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, IsolationLevel.Unspecified);
            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, IsolationLevel.Unspecified);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Equal(IsolationLevel.Unspecified, batch.GetRequiredIsolationLevel());
            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectIsolationLevel3()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, null);
            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, IsolationLevel.ReadCommitted);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Equal(IsolationLevel.ReadCommitted, batch.GetRequiredIsolationLevel());
            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectIsolationLevel4()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, IsolationLevel.ReadUncommitted);
            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, IsolationLevel.ReadCommitted);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());

            Assert.Throws<InvalidOperationException>(() =>
            {
                var value = batch.GetRequiredIsolationLevel();
            });
        }

        [Fact]
        public static void CreateBatch_NonEmptyBatch_CorrectIsolationLevel5()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, IsolationLevel.ReadCommitted);
            batch.AddScript("Test", DbBatchTransactionRequirement.DontCare, IsolationLevel.ReadCommitted);

            // Assert

            Assert.NotEmpty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            Assert.Equal(IsolationLevel.ReadCommitted, batch.GetRequiredIsolationLevel());
            Assert.False(batch.DisallowsTransaction());
            Assert.False(batch.HasFailed());
            Assert.False(batch.RequiresTransaction());
            Assert.False(batch.WasFullyExecuted());
            Assert.False(batch.WasPartiallyExecuted());
        }

        [Fact]
        public static void CreateBatch_Parameters()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch = manager.CreateBatch();

            batch.AddScript("Test");
            batch.AddScript("Test");

            batch.Parameters.Add("Para1", DbType.String);
            batch.Parameters.Add("Para2", DbType.String, "Value1");
            batch.Parameters.Add("Para3", DbType.Int32, 123);

            Assert.NotEmpty(batch.Commands);
            Assert.NotEmpty(batch.Parameters);
            Assert.False(batch.IsEmpty());

            batch.Clear();

            Assert.Empty(batch.Commands);
            Assert.Empty(batch.Parameters);
            Assert.True(batch.IsEmpty());
        }
    }
}

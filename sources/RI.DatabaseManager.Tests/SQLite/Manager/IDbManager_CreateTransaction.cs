using System;
using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Manager
{
    public sealed class IDbManager_CreateTransaction
    {
        [Fact]
        public static void CreateTransaction_Uninitialized_InvalidOperationException()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            Assert.Throws<InvalidOperationException>(() =>
            {
                manager.CreateTransaction();
            });
        }

        [Fact]
        public static void CreateTransaction_InitializedNew_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            manager.Initialize();
            manager.CreateTransaction();
        }

        [Fact]
        public static void CreateTransaction_InitializedReadWriteDefault_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            manager.Initialize();

            Assert.True(manager.CanCreate());

            manager.Create();

            SQLiteTransaction transaction = manager.CreateTransaction();

            Assert.Equal(ConnectionState.Open, transaction.Connection.State);
            Assert.Equal(false, transaction.Connection.IsReadOnly(null));
            Assert.Equal(IsolationLevel.ReadCommitted, transaction.IsolationLevel);

            transaction.Commit();
        }

        [Fact]
        public static void CreateTransaction_InitializedReadWriteCustom_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            manager.Initialize();

            Assert.True(manager.CanCreate());

            manager.Create();

            SQLiteTransaction transaction = manager.CreateTransaction(false, IsolationLevel.Serializable);

            Assert.Equal(ConnectionState.Open, transaction.Connection.State);
            Assert.Equal(false, transaction.Connection.IsReadOnly(null));
            Assert.Equal(IsolationLevel.Serializable, transaction.IsolationLevel);

            transaction.Rollback();
        }
    }
}

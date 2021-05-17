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
    public sealed class IDbManager_CreateConnection
    {
        [Fact]
        public static void CreateConnection_Uninitialized_InvalidOperationException()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            Assert.Throws<InvalidOperationException>(() =>
            {
                manager.CreateConnection();
            });
        }

        [Fact]
        public static void CreateConnection_NewState_InvalidOperationException()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            manager.Initialize();

            Assert.Throws<InvalidOperationException>(() =>
            {
                manager.CreateConnection();
            });
        }

        [Fact]
        public static void CreateConnection_InitializedReadWrite_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            manager.Initialize();

            Assert.True(manager.CanUpgrade());

            manager.Upgrade();

            SQLiteConnection connection = manager.CreateConnection();

            Assert.Equal(ConnectionState.Open, connection.State);
            Assert.Equal(false, connection.IsReadOnly(null));

            connection.Close();

            Assert.Equal(ConnectionState.Closed, connection.State);
            Assert.Equal(false, connection.IsReadOnly(null));
        }
    }
}

using System;
using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Manager
{
    public sealed class IDbManager_Close
    {
        [Fact]
        public static void Initialize_NoDatabase_CorrectPostCloseState()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            manager.Initialize();
            manager.Close();

            // Assert

            Assert.Equal(DbState.Uninitialized, manager.InitialState);
            Assert.Equal(DbState.Uninitialized, manager.State);

            Assert.Equal(-1, manager.InitialVersion);
            Assert.Equal(-1, manager.Version);
            Assert.Equal(-1, manager.MinVersion);
            Assert.Equal(-1, manager.MaxVersion);

            Assert.Equal(true, manager.SupportsBackup);
            Assert.Equal(true, manager.SupportsCleanup);
            Assert.Equal(true, manager.SupportsReadOnlyConnections);
            Assert.Equal(false, manager.SupportsRestore);
            Assert.Equal(true, manager.SupportsUpgrade);
        }

        [Fact]
        public static void Initialize_CloseTwice_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            manager.Initialize();
            manager.Close();
            manager.Close();
        }

        [Fact]
        public static void Initialize_CloseBeforeInitialize_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act + Assert

            manager.Close();
        }
    }
}

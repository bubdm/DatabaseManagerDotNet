using System;
using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Manager
{
    public sealed class IDbManager_Initialize
    {
        [Fact]
        public static void Initialize_NoDatabase_CorrectPreInitializeState()
        {
            // Arrange + Act

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Assert

            Assert.Equal(DbState.Uninitialized, manager.InitialState);
            Assert.Equal(DbState.Uninitialized, manager.State);

            Assert.Equal(-1, manager.InitialVersion);
            Assert.Equal(-1, manager.Version);
            Assert.Equal(-1, manager.MinVersion);
            Assert.Equal(-1, manager.MaxVersion);

            Assert.Equal(true, manager.SupportsBackup);
            Assert.Equal(true, manager.SupportsCleanup);
            Assert.Equal(true, manager.SupportsReadOnly);
            Assert.Equal(false, manager.SupportsRestore);
            Assert.Equal(true, manager.SupportsUpgrade);
        }

        [Fact]
        public static void Initialize_NoDatabase_CorrectPostInitializeState()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();

            IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager = new DbManagerBuilder().UseSQLite($"Data Source={tempFile.FullPath}")
                                                                                                    .UseAssemblyScriptBatches()
                                                                                                    .BuildDbManager();

            // Act

            manager.Initialize();

            // Assert

            Assert.Equal(DbState.New, manager.InitialState);
            Assert.Equal(DbState.New, manager.State);

            Assert.Equal(0, manager.InitialVersion);
            Assert.Equal(0, manager.Version);
            Assert.Equal(-1, manager.MinVersion);
            Assert.Equal(-1, manager.MaxVersion);

            Assert.Equal(true, manager.SupportsBackup);
            Assert.Equal(true, manager.SupportsCleanup);
            Assert.Equal(true, manager.SupportsReadOnly);
            Assert.Equal(false, manager.SupportsRestore);
            Assert.Equal(true, manager.SupportsUpgrade);
        }

        [Fact]
        public static void Initialize_InitializeTwice_InvalidOperationException()
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
                manager.Initialize();
            });
        }
    }
}

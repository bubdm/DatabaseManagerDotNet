using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SQLite.Builder
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class SQLiteDbManagerBuilderExtensions_UseSQLite
    {
        [Fact]
        public static void UseSQLite_ConnectionStringNull_ArgumentNullException ()
        {
            // Arrange

            DbManagerBuilder builder = new DbManagerBuilder();

            // Act + Assert

            Assert.Throws<ArgumentNullException>(() => builder.UseSQLite((string)null));
            Assert.Throws<ArgumentNullException>(() => builder.UseSQLite((SQLiteConnectionStringBuilder)null));
        }

        [Fact]
        public static void UseSQLite_ConnectionStringEmpty_ArgumentException()
        {
            // Arrange

            DbManagerBuilder builder = new DbManagerBuilder();

            // Act + Assert

            Assert.Throws<ArgumentException>(() => builder.UseSQLite(string.Empty));
        }

        [Fact]
        public static void UseSQLite_ConnectionStringInvalid_ArgumentException()
        {
            // Arrange

            DbManagerBuilder builder = new DbManagerBuilder();

            // Act + Assert

            Assert.Throws<ArgumentException>(() => builder.UseSQLite("abc"));
        }

        [Fact]
        public static void UseSQLite_ConnectionStringValid_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            DbManagerBuilder builder = new DbManagerBuilder();

            // Act

            IDbManagerBuilder<SQLiteConnection, SQLiteTransaction, DbType, SQLiteDbManager> realBuilder = builder.UseSQLite($"Data Source={tempFile.FullPath}");

            // Assert

            Assert.NotNull(realBuilder);
        }

        [Fact]
        public static void UseSQLite_Options_Success()
        {
            // Arrange

            using TemporaryFile tempFile = new TemporaryFile();
            DbManagerBuilder builder = new DbManagerBuilder();

            // Act

            IDbManagerBuilder<SQLiteConnection, SQLiteTransaction, DbType, SQLiteDbManager> realBuilder = builder.UseSQLite(options =>
            {
                options.ConnectionString.DataSource = tempFile.FullPath;
            });

            //Assert

            Assert.NotNull(realBuilder);
        }
    }
}

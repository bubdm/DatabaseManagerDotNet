using System;
using System.Data;
using System.Data.SQLite;

using Microsoft.Data.SqlClient;

using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Tests.Utilities;

using Xunit;




namespace RI.DatabaseManager.Tests.SqlServer.Builder
{
    public sealed class SqlServerDbManagerBuilderExtensions_UseSqlServer
    {
        [Fact]
        public static void UseSqlServer_ConnectionStringNull_ArgumentNullException ()
        {
            // Arrange

            DbManagerBuilder builder = new DbManagerBuilder();

            // Act + Assert

            Assert.Throws<ArgumentNullException>(() => builder.UseSqlServer((string)null));
            Assert.Throws<ArgumentNullException>(() => builder.UseSqlServer((SqlConnectionStringBuilder)null));
        }

        [Fact]
        public static void UseSqlServer_ConnectionStringEmpty_ArgumentException()
        {
            //Arrange
            DbManagerBuilder builder = new DbManagerBuilder();

            //Act + Assert
            Assert.Throws<ArgumentException>(() => builder.UseSqlServer(string.Empty));
        }

        [Fact]
        public static void UseSqlServer_ConnectionStringInvalid_ArgumentException()
        {
            //Arrange
            DbManagerBuilder builder = new DbManagerBuilder();

            //Act + Assert
            Assert.Throws<ArgumentException>(() => builder.UseSqlServer("abc"));
        }

        [Fact]
        public static void UseSqlServer_ConnectionStringValid_Success()
        {
            //Arrange
            using TemporaryFile tempFile = new TemporaryFile();
            DbManagerBuilder builder = new DbManagerBuilder();

            //Act
            IDbManagerBuilder<SqlConnection, SqlTransaction, SqlDbType, SqlServerDbManager> realBuilder = builder.UseSqlServer($"Data Source={tempFile.FullPath}");

            //Assert
            Assert.NotNull(realBuilder);
        }

        [Fact]
        public static void UseSqlServer_Options_Success()
        {
            //Arrange
            using TemporaryFile tempFile = new TemporaryFile();
            DbManagerBuilder builder = new DbManagerBuilder();

            //Act
            IDbManagerBuilder<SqlConnection, SqlTransaction, SqlDbType, SqlServerDbManager> realBuilder = builder.UseSqlServer(options =>
            {
                options.ConnectionString.DataSource = tempFile.FullPath;
            });

            //Assert
            Assert.NotNull(realBuilder);
        }
    }
}

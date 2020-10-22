using System;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Builder;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Upgrading;
using RI.DatabaseManager.Versioning;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IDbManagerBuilder" /> and <see cref="IDbManagerBuilder{TConnection,TTransaction,TManager}" />type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class SqlServerDbManagerBuilderExtensions
    {
        /// <summary>
        /// Configures the database manager builder to use Microsoft SQL Server.
        /// </summary>
        /// <param name="builder">The database manager builder.</param>
        /// <param name="connectionString">The used connection string.</param>
        /// <returns>
        /// The database manager builder.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Except for the connection string, default options for the SQL Server are used (see <see cref="SqlServerDbManagerOptions"/>).
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="connectionString"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="connectionString"/> is an empty string.</exception>
        public static IDbManagerBuilder<SqlConnection, SqlTransaction, SqlServerDbManager> UseSqlServer (this IDbManagerBuilder builder, string connectionString)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("The string argument is empty.", nameof(connectionString));
            }

            return builder.UseSqlServer(x =>
            {
                x.ConnectionString = new SqlConnectionStringBuilder(connectionString);
            });
        }

        /// <summary>
        /// Configures the database manager builder to use Microsoft SQL Server.
        /// </summary>
        /// <param name="builder">The database manager builder.</param>
        /// <param name="connectionString">The used connection string.</param>
        /// <returns>
        /// The database manager builder.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Except for the connection string, default options for the SQL Server are used (see <see cref="SqlServerDbManagerOptions"/>).
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="connectionString"/> is null.</exception>
        public static IDbManagerBuilder<SqlConnection, SqlTransaction, SqlServerDbManager> UseSqlServer (this IDbManagerBuilder builder, SqlConnectionStringBuilder connectionString)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            return builder.UseSqlServer(x =>
            {
                x.ConnectionString = connectionString;
            });
        }

        /// <summary>
        /// Configures the database manager builder to use Microsoft SQL Server.
        /// </summary>
        /// <param name="builder">The database manager builder.</param>
        /// <param name="config">The callback used to configure the SQL Server options.</param>
        /// <returns>
        /// The database manager builder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="config"/> is null.</exception>
        public static IDbManagerBuilder<SqlConnection, SqlTransaction, SqlServerDbManager> UseSqlServer (this IDbManagerBuilder builder, Action<SqlServerDbManagerOptions> config)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            SqlServerDbManagerOptions options = new SqlServerDbManagerOptions();
            config(options);
            return builder.UseSqlServer(options);
        }

        /// <summary>
        /// Configures the database manager builder to use Microsoft SQL Server.
        /// </summary>
        /// <param name="builder">The database manager builder.</param>
        /// <param name="options">The SQL Server options.</param>
        /// <returns>
        /// The database manager builder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="options"/> is null.</exception>
        public static IDbManagerBuilder<SqlConnection, SqlTransaction, SqlServerDbManager> UseSqlServer (this IDbManagerBuilder builder, SqlServerDbManagerOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            builder.AddSingleton(typeof(SqlServerDbManagerOptions), options.Clone());
            builder.AddSingleton(typeof(IDbVersionDetector<SqlConnection, SqlTransaction>), typeof(SqlServerDatabaseVersionDetector));
            builder.AddSingleton(typeof(IDbCleanupProcessor<SqlConnection, SqlTransaction>), typeof(SqlServerDatabaseCleanupProcessor));
            builder.AddSingleton(typeof(IDbVersionUpgrader<SqlConnection, SqlTransaction>), typeof(SqlServerDatabaseVersionUpgrader));

            return new DbManagerBuilder<SqlConnection, SqlTransaction, SqlServerDbManager>(builder);
        }
    }
}

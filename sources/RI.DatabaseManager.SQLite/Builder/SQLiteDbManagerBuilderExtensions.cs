using System;
using System.Data.SQLite;

using RI.Abstractions.Builder;
using RI.DatabaseManager.Backup;
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
    public static class SQLiteDbManagerBuilderExtensions
    {
        /// <summary>
        /// Configures the database manager builder to use SQLite.
        /// </summary>
        /// <param name="builder">The database manager builder.</param>
        /// <param name="connectionString">The used connection string.</param>
        /// <returns>
        /// The database manager builder.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Except for the connection string, default options for SQLite are used (see <see cref="SQLiteDbManagerOptions"/>).
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="connectionString"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="connectionString"/> is an empty string.</exception>
        public static IDbManagerBuilder<SQLiteConnection, SQLiteTransaction, SQLiteDbManager> UseSqlServer (this IDbManagerBuilder builder, string connectionString)
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
                x.ConnectionString = new SQLiteConnectionStringBuilder(connectionString);
            });
        }

        /// <summary>
        /// Configures the database manager builder to use SQLite.
        /// </summary>
        /// <param name="builder">The database manager builder.</param>
        /// <param name="connectionString">The used connection string.</param>
        /// <returns>
        /// The database manager builder.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Except for the connection string, default options for SQLite are used (see <see cref="SQLiteDbManagerOptions"/>).
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="connectionString"/> is null.</exception>
        public static IDbManagerBuilder<SQLiteConnection, SQLiteTransaction, SQLiteDbManager> UseSqlServer (this IDbManagerBuilder builder, SQLiteConnectionStringBuilder connectionString)
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
        /// Configures the database manager builder to use SQLite.
        /// </summary>
        /// <param name="builder">The database manager builder.</param>
        /// <param name="config">The callback used to configure the SQLite options.</param>
        /// <returns>
        /// The database manager builder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="config"/> is null.</exception>
        public static IDbManagerBuilder<SQLiteConnection, SQLiteTransaction, SQLiteDbManager> UseSqlServer (this IDbManagerBuilder builder, Action<SQLiteDbManagerOptions> config)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            SQLiteDbManagerOptions options = new SQLiteDbManagerOptions();
            config(options);
            return builder.UseSqlServer(options);
        }

        /// <summary>
        /// Configures the database manager builder to use SQLite.
        /// </summary>
        /// <param name="builder">The database manager builder.</param>
        /// <param name="options">The SQLite options.</param>
        /// <returns>
        /// The database manager builder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="options"/> is null.</exception>
        public static IDbManagerBuilder<SQLiteConnection, SQLiteTransaction, SQLiteDbManager> UseSqlServer (this IDbManagerBuilder builder, SQLiteDbManagerOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            builder.AddSingleton(typeof(SQLiteDbManagerOptions), options.Clone());
            builder.AddSingleton(typeof(IDbVersionDetector<SQLiteConnection, SQLiteTransaction>), typeof(SQLiteDbVersionDetector));
            builder.AddSingleton(typeof(IDbBackupCreator<SQLiteConnection, SQLiteTransaction>), typeof(SQLiteDatabaseBackupCreator));
            builder.AddSingleton(typeof(IDbCleanupProcessor<SQLiteConnection, SQLiteTransaction>), typeof(SQLiteDbCleanupProcessor));
            builder.AddSingleton(typeof(IDbVersionUpgrader<SQLiteConnection, SQLiteTransaction>), typeof(SQLiteDatabaseVersionUpgrader));

            return new DbManagerBuilder<SQLiteConnection, SQLiteTransaction, SQLiteDbManager>(builder);
        }
    }
}

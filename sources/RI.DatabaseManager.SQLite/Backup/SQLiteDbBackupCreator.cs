using System;
using System.Data;
using System.Data.SQLite;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Backup
{
    /// <summary>
    ///     Implements a database backup creator for SQLite databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDbBackupCreator" /> performs a backup using the SQLite online backup API.
    ///     </para>
    ///     <para>
    ///         <see cref="SQLiteDbBackupCreator" /> supports the following types for backup targets:
    ///         <see cref="string" /> (backup to SQLite database file using the string as the file path),
    ///         <see cref="SQLiteConnectionStringBuilder" /> (backup to SQLite database using this connection string),
    ///         <see cref="SQLiteConnection" /> (backup to SQLite database using this database connection).
    ///     </para>
    ///     <note type="note">
    ///         <see cref="SQLiteDbBackupCreator" /> does not support restore.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDbBackupCreator : DbBackupCreatorBase<SQLiteConnection, SQLiteTransaction, DbType>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbBackupCreator" />.
        /// </summary>
        /// <param name="options"> The used SQLite database manager options.</param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SQLiteDbBackupCreator (SQLiteDbManagerOptions options, ILogger logger) : base(options, logger)
        {
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool SupportsBackup => true;

        /// <inheritdoc />
        public override bool SupportsRestore => false;

        /// <inheritdoc />
        public override bool Backup (IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager, object backupTarget)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (backupTarget == null)
            {
                throw new ArgumentNullException(nameof(backupTarget));
            }

            string file = backupTarget as string;
            SQLiteConnectionStringBuilder connectionStringBuilder = backupTarget as SQLiteConnectionStringBuilder;
            SQLiteConnection connection = backupTarget as SQLiteConnection;

            if ((file == null) && (connectionStringBuilder == null) && (connection == null))
            {
                throw new ArgumentException($"Type {backupTarget.GetType().Name} is not supported by {this.GetType().Name}.", nameof(backupTarget));
            }

            try
            {
                this.Log(LogLevel.Information, "Beginning SQLite database backup");

                SQLiteConnectionStringBuilder targetConnectionString = connectionStringBuilder ?? new SQLiteConnectionStringBuilder(this.Options.GetConnectionString());
                if (file != null)
                {
                    targetConnectionString.DataSource = file;
                }

                using (SQLiteConnection source = manager.CreateConnection(true))
                {
                    using (SQLiteConnection target = connection ?? ((SQLiteDbManager)manager).CreateInternalConnection(targetConnectionString.ConnectionString, false))
                    {
                        if (target.State != ConnectionState.Open)
                        {
                            target.Open();
                        }

                        string sourceDatabaseName = "main";
                        string targetDatabaseName = "main";

                        source.BackupDatabase(target, targetDatabaseName, sourceDatabaseName, -1, null, 10);
                    }
                }

                this.Log(LogLevel.Information, "Finished SQLite database backup");
                return true;
            }
            catch (Exception exception)
            {
                this.Log(LogLevel.Error, "SQLite database backup failed:{0}{1}", Environment.NewLine, exception.ToString());
                return false;
            }
        }

        /// <inheritdoc />
        public override bool Restore (IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager, object backupSource)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not support restore.");
        }

        #endregion
    }
}

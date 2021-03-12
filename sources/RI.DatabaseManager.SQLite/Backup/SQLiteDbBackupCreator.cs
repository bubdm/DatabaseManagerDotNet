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
    ///         It also supports additional pre-processing and post-processing batches which are executed on the source database.
    ///         See <see cref="SQLiteDbManagerOptions"/> for more information.
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

        private SQLiteDbManagerOptions Options { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbBackupCreator" />.
        /// </summary>
        /// <param name="options"> The used SQLite database manager options.</param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SQLiteDbBackupCreator (SQLiteDbManagerOptions options, ILogger logger) : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
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

                IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> preprocessingBatch = null;
                IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> postprocessingBatch = null;

                if (!this.Options.BackupPreprocessingBatch.IsEmpty())
                {
                    preprocessingBatch = this.Options.BackupPreprocessingBatch;
                }
                else if (!string.IsNullOrWhiteSpace(this.Options.BackupPreprocessingBatchName))
                {
                    preprocessingBatch = manager.GetBatch(this.Options.BackupPreprocessingBatchName);
                }

                if (!this.Options.BackupPostprocessingBatch.IsEmpty())
                {
                    postprocessingBatch = this.Options.BackupPostprocessingBatch;
                }
                else if (!string.IsNullOrWhiteSpace(this.Options.BackupPostprocessingBatchName))
                {
                    postprocessingBatch = manager.GetBatch(this.Options.BackupPostprocessingBatchName);
                }

                SQLiteConnectionStringBuilder targetConnectionString = connectionStringBuilder ?? new SQLiteConnectionStringBuilder(this.Options.GetConnectionString());
                if (file != null)
                {
                    targetConnectionString.DataSource = file;
                }

                if (preprocessingBatch != null)
                {
                    bool result = manager.ExecuteBatch(preprocessingBatch, false, false);

                    if (!result)
                    {
                        this.Log(LogLevel.Error, "SQLite database backup preprocessing failed.");
                        return false;
                    }
                }

                using (SQLiteConnection source = manager.CreateConnection(false))
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

                if (postprocessingBatch != null)
                {
                    bool result = manager.ExecuteBatch(postprocessingBatch, false, false);

                    if (!result)
                    {
                        this.Log(LogLevel.Error, "SQLite database backup postprocessing failed.");
                        return false;
                    }
                }

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

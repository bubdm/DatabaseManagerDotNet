using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Backup
{
    /// <summary>
    ///     Implements a database backup creator for SQLite databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDatabaseBackupCreator" /> performs a backup using the SQLite online backup API.
    ///         It also supports additional pre-processing and post-processing steps which are executed on the source database.
    ///     </para>
    ///     <para>
    ///         <see cref="SQLiteDatabaseBackupCreator" /> supports the following types for backup targets:
    ///         <see cref="FilePath" /> (backup to SQLite database file using this file path),
    ///         <see cref="string" /> or <see cref="SQLiteConnectionStringBuilder" /> (backup to SQLite database using this connection string),
    ///         <see cref="SQLiteConnection" /> (backup to SQLite database using this database connection),
    ///         <see cref="Stream" /> (backup to a stream as a SQLite database).
    ///     </para>
    ///     <para>
    ///         <see cref="SQLiteDatabaseBackupCreator" /> does not support restore.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDatabaseBackupCreator : DbBackupCreatorBase<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDbManager, SQLiteDatabaseManagerConfiguration>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDatabaseBackupCreator" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         No pre-processing or post-processing is used.
        ///     </para>
        /// </remarks>
        public SQLiteDatabaseBackupCreator ()
            : this(null, (SQLiteDbProcessingStep)null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDatabaseBackupCreator" />.
        /// </summary>
        /// <param name="preprocessingStep"> The pre-processing step executed before the backup is created or null if no pre-processing step is used. </param>
        /// <param name="postprocessingStep"> The post-processing step executed after the backup was created or null if no post-processing step is used. </param>
        public SQLiteDatabaseBackupCreator (SQLiteDbProcessingStep preprocessingStep, SQLiteDbProcessingStep postprocessingStep)
        {
            this.PreprocessingStep = preprocessingStep;
            this.PostprocessingStep = postprocessingStep;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDatabaseBackupCreator" />.
        /// </summary>
        /// <param name="preprocessingScriptName"> The script name of the pre-processing step executed before the backup is created or null if no pre-processing step is used. </param>
        /// <param name="postprocessingScriptName"> The script name of the post-processing step executed after the backup was created or null if no post-processing step is used. </param>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="postprocessingScriptName" /> or <paramref name="postprocessingScriptName"/> is an empty string. </exception>
        public SQLiteDatabaseBackupCreator(string preprocessingScriptName, string postprocessingScriptName)
        {
            if (preprocessingScriptName != null)
            {
                if (preprocessingScriptName.IsNullOrEmptyOrWhitespace())
                {
                    throw new EmptyStringArgumentException(nameof(preprocessingScriptName));
                }
            }

            if (postprocessingScriptName != null)
            {
                if (postprocessingScriptName.IsNullOrEmptyOrWhitespace())
                {
                    throw new EmptyStringArgumentException(nameof(postprocessingScriptName));
                }
            }

            if (preprocessingScriptName != null)
            {
                SQLiteDbProcessingStep step = new SQLiteDbProcessingStep();
                step.AddScript(preprocessingScriptName);
                this.PreprocessingStep = step;
            }
            else
            {
                this.PreprocessingStep = null;
            }

            if (postprocessingScriptName != null)
            {
                SQLiteDbProcessingStep step = new SQLiteDbProcessingStep();
                step.AddScript(postprocessingScriptName);
                this.PostprocessingStep = step;
            }
            else
            {
                this.PostprocessingStep = null;
            }
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used post-processing step.
        /// </summary>
        /// <value>
        ///     The used post-processing step or null if no post-processing is used.
        /// </value>
        public SQLiteDbProcessingStep PostprocessingStep { get; }

        /// <summary>
        ///     Gets the used pre-processing step.
        /// </summary>
        /// <value>
        ///     The used pre-processing step or null if no pre-processing is used.
        /// </value>
        public SQLiteDbProcessingStep PreprocessingStep { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool RequiresScriptLocator => (this.PreprocessingStep?.RequiresScriptLocator ?? false) || (this.PostprocessingStep?.RequiresScriptLocator ?? false);

        /// <inheritdoc />
        public override bool SupportsRestore => false;

        /// <inheritdoc />
        public override bool Backup (SQLiteDbManager manager, object backupTarget)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (backupTarget == null)
            {
                throw new ArgumentNullException(nameof(backupTarget));
            }

            FilePath backupFile = backupTarget as FilePath;
            string connectionString = backupTarget as string;
            SQLiteConnectionStringBuilder connectionStringBuilder = backupTarget as SQLiteConnectionStringBuilder;
            SQLiteConnection connection = backupTarget as SQLiteConnection;
            Stream stream = backupTarget as Stream;

            if ((backupFile == null) && (connectionString == null) && (connectionStringBuilder == null) && (connection == null) && (stream == null))
            {
                throw new InvalidTypeArgumentException(nameof(backupTarget), nameof(SQLiteDatabaseBackupCreator) + " does not support " + backupTarget.GetType().Name + " as backup target.");
            }

            try
            {
                TemporaryFile tempFile = null;

                try
                {
                    SQLiteConnectionStringBuilder targetConnectionString = connectionStringBuilder ?? new SQLiteConnectionStringBuilder(connectionString ?? manager.Configuration.ConnectionString.ConnectionString);
                    if (backupFile != null)
                    {
                        targetConnectionString.DataSource = backupFile.PathResolved;
                    }
                    if (stream != null)
                    {
                        tempFile = new TemporaryFile();
                        targetConnectionString.DataSource = tempFile.File.PathResolved;
                    }

                    //TODO: Log: this.Log(LogLevel.Debug, "Beginning SQLite database backup: Source=[{0}]; Target=[{1}]", manager.Configuration.ConnectionString, backupTarget);

                    using (SQLiteConnection source = manager.CreateInternalConnection(null, false))
                    {
                        if (this.PreprocessingStep != null)
                        {
                            using (SQLiteTransaction transaction = this.PreprocessingStep.RequiresTransaction ? source.BeginTransaction(IsolationLevel.Serializable) : null)
                            {
                                this.PreprocessingStep.Execute(manager, source, transaction);
                                transaction?.Commit();
                            }
                        }

                        using (SQLiteConnection target = connection ?? manager.CreateInternalConnection(targetConnectionString.ConnectionString, false))
                        {
                            if (target.State != ConnectionState.Open)
                            {
                                target.Open();
                            }

                            string sourceDatabaseName = "main";
                            string targetDatabaseName = "main";

                            //TODO: Log: this.Log(LogLevel.Debug, "Executing SQLite database backup: {0} -> {1}", sourceDatabaseName, targetDatabaseName);
                            source.BackupDatabase(target, targetDatabaseName, sourceDatabaseName, -1, null, 10);
                        }

                        if (this.PostprocessingStep != null)
                        {
                            using (SQLiteTransaction transaction = this.PostprocessingStep.RequiresTransaction ? source.BeginTransaction(IsolationLevel.Serializable) : null)
                            {
                                this.PostprocessingStep.Execute(manager, source, transaction);
                                transaction?.Commit();
                            }
                        }
                    }

                    if (stream != null)
                    {
                        //TODO: This looks hacky...!
                        Thread.Sleep(100);
                        using (TemporaryFile tempFile2 = new TemporaryFile())
                        {
                            tempFile.File.Copy(tempFile2.File, true);
                            using (FileStream fs = new FileStream(tempFile2.File, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                fs.CopyTo(stream);
                            }
                        }
                    }

                    //TODO: Log: this.Log(LogLevel.Debug, "Finished SQLite database backup: Source=[{0}]; Target=[{1}]", manager.Configuration.ConnectionString, backupTarget);

                    return true;
                }
                finally
                {
                    tempFile?.Delete();
                }
            }
            catch (Exception exception)
            {
                //TODO: Log: this.Log(LogLevel.Error, "SQLite database backup failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
                return false;
            }
        }

        /// <inheritdoc />
        public override bool Restore (SQLiteDbManager manager, object backupSource)
        {
            throw new NotSupportedException(nameof(SQLiteDatabaseBackupCreator) + "does not support restore.");
        }

        #endregion
    }
}

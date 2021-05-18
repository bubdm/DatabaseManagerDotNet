using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder.Options;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Stores configuration and options for the SQLite database manager (<see cref="SQLiteDbManager" />).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If commands are added to <see cref="CustomCleanupBatch" />, <see cref="CustomCleanupBatch" /> is used for cleanup instead of the batch named by <see cref="CustomCleanupBatchName" /> or the default script.
    ///     </para>
    ///     <para>
    ///         If <see cref="CustomCleanupBatch" /> is empty (has no commands) and <see cref="CustomCleanupBatchName" /> is not null, <see cref="CustomCleanupBatchName" /> is used for cleanup instead of the default script.
    ///     </para>
    ///     <para>
    ///         The default cleanup script is (each line executed as a separate command):
    ///     </para>
    /// <code language="sql">
    ///  <![CDATA[
    /// VACUUM;
    /// ANALYZE;
    /// REINDEX;
    ///  ]]>
    ///  </code>
    ///     <para>
    ///         If commands are added to <see cref="CustomVersionDetectionBatch" />, <see cref="CustomVersionDetectionBatch" /> is used for version detection instead of the batch named by <see cref="CustomVersionDetectionBatchName" /> or the default script.
    ///     </para>
    ///     <para>
    ///         If <see cref="CustomVersionDetectionBatch" /> is empty (has no commands) and <see cref="CustomVersionDetectionBatchName" /> is not null, <see cref="CustomVersionDetectionBatchName" /> is used for version detection instead of the default script.
    ///     </para>
    ///     <para>
    ///         The default version detection script is (each line executed as a separate command):
    ///     </para>
    /// <code language="sql">
    ///  <![CDATA[
    /// SELECT (SELECT count(*) FROM [sqlite_master] WHERE [type] = 'table' AND [name] = '__@TableName') - 1;
    /// SELECT (SELECT count(*) FROM [__@TableName] WHERE [__@NameColumnName] = '__@KeyName') - 1;
    /// SELECT [__@ValueColumnName] FROM [__@TableName] WHERE [__@NameColumnName] = '__@KeyName'; 
    ///  ]]>
    ///  </code>
    /// <para>
    /// The following replacements will be done in the default version detection script:
    /// <c>__@TableName</c> with <see cref="DefaultVersionDetectionTable"/> (default: <c>_DatabaseSettings</c>),
    /// <c>__@NameColumnName</c> with <see cref="DefaultVersionDetectionNameColumn"/> (default: <c>Name</c>),
    /// <c>__@ValueColumnName</c> with <see cref="DefaultVersionDetectionValueColumn"/> (default: <c>Value</c>),
    /// <c>__@KeyName</c> with <see cref="DefaultVersionDetectionKey"/> (default: <c>Database.Version</c>).
    /// </para>
    /// <para>
    /// Therefore, the default script requires the following table setup:
    /// </para>
    /// <code language="sql">
    ///  <![CDATA[
    /// CREATE TABLE [_DatabaseSettings]
    /// ( 
    ///    [Id]    INTEGER PRIMARY KEY ASC ON CONFLICT ROLLBACK AUTOINCREMENT,
    ///    [Name]  TEXT    NOT NULL ON CONFLICT ROLLBACK UNIQUE ON CONFLICT ROLLBACK,
    ///    [Value] TEXT    NULL
    /// );
    ///
    /// INSERT INTO [_DatabaseSettings] ([Name], [Value]) VALUES ('Database.Version', '0');
    ///  ]]>
    ///  </code>
    ///     <para>
    ///         The default setup script (as shown above) can be obtained using <see cref="GetDefaultCreationScript"/>.
    ///     </para>
    ///     <para>
    ///         If commands are added to <see cref="BackupPreprocessingBatch" />/<see cref="BackupPostprocessingBatch"/>, <see cref="BackupPreprocessingBatch" />/<see cref="BackupPostprocessingBatch"/> is used for preprocessing/postprocessing instead of the batch named by <see cref="BackupPreprocessingBatchName" />/<see cref="BackupPostprocessingBatchName" />.
    ///     </para>
    ///     <para>
    ///         If <see cref="BackupPreprocessingBatch" />/<see cref="BackupPostprocessingBatch"/> is empty (has no commands) and <see cref="BackupPreprocessingBatchName" />/<see cref="BackupPostprocessingBatchName" /> is not null, <see cref="BackupPreprocessingBatchName" />/<see cref="BackupPostprocessingBatchName" /> is used for preprocessing/postprocessing.
    ///     </para>
    ///     <para>
    ///         If neither <see cref="BackupPreprocessingBatch" />/<see cref="BackupPostprocessingBatch"/> nor <see cref="BackupPreprocessingBatchName" />/<see cref="BackupPostprocessingBatchName" /> is configured, no preprocessing/postprocessing will be done before/after backup.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDbManagerOptions : IDbManagerOptions, ISupportDefaultDatabaseUpgrading, ISupportDefaultDatabaseCreation, ICloneable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbManagerOptions" />.
        /// </summary>
        public SQLiteDbManagerOptions ()
        {
            this.ConnectionString = new SQLiteConnectionStringBuilder();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbManagerOptions" />.
        /// </summary>
        /// <param name="connectionString"> The used connection string. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
        public SQLiteDbManagerOptions (string connectionString)
            : this()
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("The string argument is empty.", nameof(connectionString));
            }

            this.ConnectionString = new SQLiteConnectionStringBuilder(connectionString);
        }

        #endregion




        #region Instance Fields

        private SQLiteConnectionStringBuilder _connectionString;

        private string _customCleanupBatchName;

        private string _customVersionDetectionBatchName;

        private string _backupPreprocessingBatchName;

        private string _backupPostprocessingBatchName;

        private string _versionUpgradeNameFormat = @".+?(?<sourceVersion>\d{4}).*";

        private string _defaultVersionDetectionKey = "Database.Version";

        private string _defaultVersionDetectionNameColumn = "Name";

        private string _defaultVersionDetectionTable = "_DatabaseSettings";

        private string _defaultVersionDetectionValueColumn = "Value";

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets the connection string builder.
        /// </summary>
        /// <value>
        ///     The connection string builder. Cannot be null.
        /// </value>
        /// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
        public SQLiteConnectionStringBuilder ConnectionString
        {
            get => this._connectionString;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._connectionString = value;
            }
        }

        /// <summary>
        ///     Gets the used custom cleanup batch.
        /// </summary>
        /// <value>
        ///     The used custom cleanup batch.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="CustomCleanupBatch" /> is empty (has no commands).
        ///     </para>
        /// </remarks>
        public DbBatch<SQLiteConnection, SQLiteTransaction, DbType> CustomCleanupBatch { get; private set; } = new DbBatch<SQLiteConnection, SQLiteTransaction, DbType>();

        /// <summary>
        ///     Gets or sets the used custom cleanup batch name.
        /// </summary>
        /// <value>
        ///     The used custom cleanup batch name.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="CustomCleanupBatchName" /> is null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string CustomCleanupBatchName
        {
            get => this._customCleanupBatchName;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The string argument is empty.", nameof(value));
                    }
                }

                this._customCleanupBatchName = value;
            }
        }

        /// <summary>
        ///     Gets the list of custom collations to be registered with the connections.
        /// </summary>
        /// <value>
        ///     The list of custom collations to be registered with the connections.
        /// </value>
        public List<SQLiteFunction> CustomCollations { get; } = new List<SQLiteFunction>();

        /// <summary>
        ///     Gets the list of custom functions to be registered with the connections.
        /// </summary>
        /// <value>
        ///     The list of custom functions to be registered with the connections.
        /// </value>
        public List<SQLiteFunction> CustomFunctions { get; } = new List<SQLiteFunction>();

        /// <summary>
        ///     Gets the used custom version detection batch.
        /// </summary>
        /// <value>
        ///     The used custom version detection batch.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="CustomVersionDetectionBatch" /> is empty (has no commands).
        ///     </para>
        /// </remarks>
        public DbBatch<SQLiteConnection, SQLiteTransaction, DbType> CustomVersionDetectionBatch { get; private set; } = new DbBatch<SQLiteConnection, SQLiteTransaction, DbType>();

        /// <summary>
        ///     Gets or sets the used custom version detection batch name.
        /// </summary>
        /// <value>
        ///     The used custom version detection batch name.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="CustomVersionDetectionBatchName" /> is null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string CustomVersionDetectionBatchName
        {
            get => this._customVersionDetectionBatchName;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The string argument is empty.", nameof(value));
                    }
                }

                this._customVersionDetectionBatchName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the path to the used database file.
        /// </summary>
        /// <value>
        ///     The path to the used database file. Cannot be null or an empty string.
        /// </value>
        /// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string DatabaseFile
        {
            get => this.ConnectionString.DataSource;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("The string argument is empty.", nameof(value));
                }

                this.ConnectionString.DataSource = value;
            }
        }

        /// <summary>
        ///     Gets or sets the used key name for the database version entry when the default version detection script is used.
        /// </summary>
        /// <value>
        ///     The used key name for the database version entry when the default version detection script is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="DefaultVersionDetectionKey" /> is <c> Database.Version </c>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string DefaultVersionDetectionKey
        {
            get => this._defaultVersionDetectionKey;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The string argument is empty.", nameof(value));
                    }
                }

                this._defaultVersionDetectionKey = value;
            }
        }

        /// <summary>
        ///     Gets or sets the used column name for the key names when the default version detection script is used.
        /// </summary>
        /// <value>
        ///     The used column name for the key names when the default version detection script is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="DefaultVersionDetectionNameColumn" /> is <c> Name </c>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string DefaultVersionDetectionNameColumn
        {
            get => this._defaultVersionDetectionNameColumn;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The string argument is empty.", nameof(value));
                    }
                }

                this._defaultVersionDetectionNameColumn = value;
            }
        }

        /// <summary>
        ///     Gets or sets the used table name when the default version detection script is used.
        /// </summary>
        /// <value>
        ///     The used table name when the default version detection script is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="DefaultVersionDetectionTable" /> is <c> _DatabaseSettings </c>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string DefaultVersionDetectionTable
        {
            get => this._defaultVersionDetectionTable;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The string argument is empty.", nameof(value));
                    }
                }

                this._defaultVersionDetectionTable = value;
            }
        }

        /// <summary>
        ///     Gets or sets the used column name for the values when the default version detection script is used.
        /// </summary>
        /// <value>
        ///     The used column name for the values when the default version detection script is used.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="DefaultVersionDetectionValueColumn" /> is <c> Value </c>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string DefaultVersionDetectionValueColumn
        {
            get => this._defaultVersionDetectionValueColumn;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The string argument is empty.", nameof(value));
                    }
                }

                this._defaultVersionDetectionValueColumn = value;
            }
        }

        /// <summary>
        ///     Gets the used backup preprocessing batch.
        /// </summary>
        /// <value>
        ///     The used backup preprocessing batch.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="BackupPreprocessingBatch" /> is empty (has no commands).
        ///     </para>
        /// </remarks>
        public DbBatch<SQLiteConnection, SQLiteTransaction, DbType> BackupPreprocessingBatch { get; private set; } = new DbBatch<SQLiteConnection, SQLiteTransaction, DbType>();

        /// <summary>
        ///     Gets the used backup preprocessing batch name.
        /// </summary>
        /// <value>
        ///     The used backup preprocessing batch name.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="BackupPreprocessingBatchName" /> is null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string BackupPreprocessingBatchName
        {
            get => this._backupPreprocessingBatchName;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The string argument is empty.", nameof(value));
                    }
                }

                this._backupPreprocessingBatchName = value;
            }
        }

        /// <summary>
        ///     Gets the used backup preprocessing batch.
        /// </summary>
        /// <value>
        ///     The used backup preprocessing batch.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="BackupPostprocessingBatch" /> is empty (has no commands).
        ///     </para>
        /// </remarks>
        public DbBatch<SQLiteConnection, SQLiteTransaction, DbType> BackupPostprocessingBatch { get; private set; } = new DbBatch<SQLiteConnection, SQLiteTransaction, DbType>();

        /// <summary>
        ///     Gets the used backup postprocessing batch name.
        /// </summary>
        /// <value>
        ///     The used backup postprocessing batch name.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         By default, <see cref="BackupPostprocessingBatchName" /> is null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string BackupPostprocessingBatchName
        {
            get => this._backupPostprocessingBatchName;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The string argument is empty.", nameof(value));
                    }
                }

                this._backupPostprocessingBatchName = value;
            }
        }

        /// <summary>
        /// Gets the default cleanup script.
        /// </summary>
        /// <param name="transactionRequirement">The transaction requirement.</param>
        /// <param name="isolationLevel">The isolation level requirement.</param>
        /// <returns>
        /// The array with the commands of the default cleanup script.
        /// </returns>
        public string[] GetDefaultCleanupScript (out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.Disallowed;
            isolationLevel = IsolationLevel.Serializable;

            return (string[])this.DefaultCleanupScript.Clone();
        }

        /// <summary>
        /// Gets the default version detection script.
        /// </summary>
        /// <param name="transactionRequirement">The transaction requirement.</param>
        /// <param name="isolationLevel">The isolation level requirement.</param>
        /// <returns>
        /// The array with the commands of the default version detection script.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The placeholders in the default script are replaced as follows: <c>__@TableName</c> = <see cref="DefaultVersionDetectionTable"/>, <c>__@NameColumnName</c> = <see cref="DefaultVersionDetectionNameColumn"/>, <c>__@ValueColumnName</c> = <see cref="DefaultVersionDetectionValueColumn"/>, <c>__@KeyName</c> = <see cref="DefaultVersionDetectionKey"/>.
        /// </para>
        /// </remarks>
        public string[] GetDefaultVersionDetectionScript (out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.Required;
            isolationLevel = IsolationLevel.Serializable;

            List<string> commands = new List<string>();

            foreach (string command in this.DefaultVersionDetectionScript)
            {
                commands.Add(command.Replace("__@TableName", this.DefaultVersionDetectionTable)
                                    .Replace("__@NameColumnName", this.DefaultVersionDetectionNameColumn)
                                    .Replace("__@ValueColumnName", this.DefaultVersionDetectionValueColumn)
                                    .Replace("__@KeyName", this.DefaultVersionDetectionKey));
            }

            return commands.ToArray();
        }

        /// <summary>
        /// Gets the default setup script.
        /// </summary>
        /// <param name="transactionRequirement">The transaction requirement.</param>
        /// <param name="isolationLevel">The isolation level requirement.</param>
        /// <returns>
        /// The array with the commands of the default setup script or null or an empty array if a default setup script is not available.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The placeholders in the default script are replaced as follows: <c>__@TableName</c> = <see cref="DefaultVersionDetectionTable"/>, <c>__@NameColumnName</c> = <see cref="DefaultVersionDetectionNameColumn"/>, <c>__@ValueColumnName</c> = <see cref="DefaultVersionDetectionValueColumn"/>, <c>__@KeyName</c> = <see cref="DefaultVersionDetectionKey"/>.
        /// </para>
        /// </remarks>
        public string[] GetDefaultCreationScript(out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.Disallowed;
            isolationLevel = IsolationLevel.Serializable;

            List<string> commands = new List<string>();

            foreach (string command in this.DefaultSetupScript)
            {
                commands.Add(command.Replace("__@TableName", this.DefaultVersionDetectionTable)
                                    .Replace("__@NameColumnName", this.DefaultVersionDetectionNameColumn)
                                    .Replace("__@ValueColumnName", this.DefaultVersionDetectionValueColumn)
                                    .Replace("__@KeyName", this.DefaultVersionDetectionKey));
            }

            return commands.ToArray();
        }

        private string[] DefaultCleanupScript { get; } =
        {
            "VACUUM;",
            "ANALYZE;",
            "REINDEX;",
        };

        private string[] DefaultVersionDetectionScript
        {
            get
            {
                return new[]
                {
                    "SELECT (SELECT count(*) FROM [sqlite_master] WHERE [type] = 'table' AND [name] = '__@TableName') - 1;",
                    "SELECT (SELECT count(*) FROM [__@TableName] WHERE [__@NameColumnName] = '__@KeyName') - 1;",
                    "SELECT [__@ValueColumnName] FROM [__@TableName] WHERE [__@NameColumnName] = '__@KeyName';",
                };
            }
        }

        private string[] DefaultSetupScript
        {
            get
            {
                return new[]
                {
                    "CREATE TABLE [_DatabaseSettings] ([Id] INTEGER PRIMARY KEY ASC ON CONFLICT ROLLBACK AUTOINCREMENT, [Name] TEXT NOT NULL ON CONFLICT ROLLBACK UNIQUE ON CONFLICT ROLLBACK, [Value] TEXT NULL);",
                    "INSERT INTO [_DatabaseSettings] ([Name], [Value]) VALUES ('Database.Version', '0');",
                };
            }
        }

        #endregion




        #region Instance Methods

        /// <inheritdoc cref="ICloneable.Clone" />
        public SQLiteDbManagerOptions Clone ()
        {
            SQLiteDbManagerOptions clone = new SQLiteDbManagerOptions();
            clone.ConnectionString = new SQLiteConnectionStringBuilder(this.ConnectionString.ToString());
            clone.CustomFunctions.AddRange(this.CustomFunctions);
            clone.CustomCollations.AddRange(this.CustomCollations);
            clone.CustomCleanupBatch = this.CustomCleanupBatch.Clone();
            clone.CustomCleanupBatchName = this.CustomCleanupBatchName;
            clone.CustomVersionDetectionBatch = this.CustomVersionDetectionBatch.Clone();
            clone.CustomVersionDetectionBatchName = this.CustomVersionDetectionBatchName;
            clone.BackupPreprocessingBatch = this.BackupPreprocessingBatch.Clone();
            clone.BackupPreprocessingBatchName = this.BackupPreprocessingBatchName;
            clone.BackupPostprocessingBatch = this.BackupPostprocessingBatch.Clone();
            clone.BackupPostprocessingBatchName = this.BackupPostprocessingBatchName;
            clone.DefaultVersionDetectionTable = this.DefaultVersionDetectionTable;
            clone.DefaultVersionDetectionNameColumn = this.DefaultVersionDetectionNameColumn;
            clone.DefaultVersionDetectionValueColumn = this.DefaultVersionDetectionValueColumn;
            clone.DefaultVersionDetectionKey = this.DefaultVersionDetectionKey;
            return clone;
        }

        #endregion




        #region Interface: ICloneable

        /// <inheritdoc />
        object ICloneable.Clone ()
        {
            return this.Clone();
        }

        #endregion




        /// <inheritdoc />
        public string GetConnectionString () => this.ConnectionString?.ToString();

        /// <inheritdoc />
        public string BatchNameFormat
        {
            get => this._versionUpgradeNameFormat;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("The string argument is empty.", nameof(value));
                }

                this._versionUpgradeNameFormat = value;
            }
        }
    }
}

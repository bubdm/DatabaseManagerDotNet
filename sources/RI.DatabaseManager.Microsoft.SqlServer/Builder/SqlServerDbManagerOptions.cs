using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.Data.SqlClient;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder.Options;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Stores configuration and options for the Microsoft SQL Server database manager (<see cref="SqlServerDbManager" />).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If commands are added to <see cref="CustomCleanupBatch" />, <see cref="CustomCleanupBatch" /> is used for cleanup instead of the batch named by <see cref="CustomCleanupBatchName" /> or the default script.
    ///     </para>
    ///     <para>
    ///         If <see cref="CustomCleanupBatch" /> is empty (has no commands) and <see cref="CustomCleanupBatchName" /> is not null, <see cref="CustomCleanupBatchName" /> is used for cleanup instead of the default script.
    ///     </para>
    ///     <para>
    ///         The default cleanup script is:
    ///     </para>
    /// <code language="sql">
    ///  <![CDATA[
    /// DBCC SHRINKDATABASE 0
    ///  ]]>
    ///  </code>
    ///     <para>
    ///         If commands are added to <see cref="CustomVersionDetectionBatch" />, <see cref="CustomVersionDetectionBatch" /> is used for version detection instead of the batch named by <see cref="CustomVersionDetectionBatchName" /> or the default script.
    ///     </para>
    ///     <para>
    ///         If <see cref="CustomVersionDetectionBatch" /> is empty (has no commands) and <see cref="CustomVersionDetectionBatchName" /> is not null, <see cref="CustomVersionDetectionBatchName" /> is used for version detection instead of the default script.
    ///     </para>
    /// <code language="cs">
    ///  <![CDATA[
    /// // create a list of integers
    /// var numbers = new List<int> { 0, 1, 4, 5, 12 };
    ///
    /// // create the comparer, including a hash value calculation function
    /// // what we do: two integers are considered the same if they have the same remainder when divided by 5
    /// var comparer = new EqualityComparison<int>(
    ///     (x,y) => (x % 5) == (y % 5),
    ///     (obj) => obj % 5 );
    /// 
    /// // lets use the comparer to check whether we have some numbers in the list or not
    /// numbers.Contains(0, comparer); //returns "true" because of 0 in the list
    /// numbers.Contains(1, comparer); //returns "true" because of 1 in the list
    /// numbers.Contains(2, comparer); //returns "true" because of 12 in the list
    /// numbers.Contains(3, comparer); //returns "false"
    /// numbers.Contains(4, comparer); //returns "true" because of 4 in the list
    /// numbers.Contains(5, comparer); //returns "true" because of 0 in the list
    ///  ]]>
    ///  </code>
    ///     <para>
    ///         The default version detection script is (each line executed as a separate command):
    ///     </para>
    /// <code language="sql">
    ///  <![CDATA[
    /// IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='__@TableName') SELECT 1 ELSE SELECT 0;
    /// IF (SELECT count(*) FROM [__@TableName] WHERE [__@NameColumnName] = '__@KeyName') = 0 SELECT -1 ELSE SELECT 1;
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
    ///     [Id]    INT            PRIMARY KEY IDENTITY(1,1),
    ///     [Name]  NVARCHAR(1024) NOT NULL,
    ///     [Value] NVARCHAR(MAX)  NULL
    /// );
    ///
    /// INSERT INTO [_DatabaseSettings] ([Name], [Value]) VALUES ('Database.Version', '0');
    ///  ]]>
    ///  </code>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDbManagerOptions : IDbManagerOptions, ISupportVersionUpgradeNameFormat, ICloneable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbManagerOptions" />.
        /// </summary>
        public SqlServerDbManagerOptions ()
        {
            this.ConnectionString = new SqlConnectionStringBuilder();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbManagerOptions" />.
        /// </summary>
        /// <param name="connectionString"> The used connection string. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
        public SqlServerDbManagerOptions (string connectionString)
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

            this.ConnectionString = new SqlConnectionStringBuilder(connectionString);
        }

        #endregion




        #region Instance Fields

        private SqlConnectionStringBuilder _connectionString;

        private string _customCleanupBatchName;

        private string _customVersionDetectionBatchName;

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
        public SqlConnectionStringBuilder ConnectionString
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
        public DbBatch<SqlConnection, SqlTransaction, SqlDbType> CustomCleanupBatch { get; private set; } = new DbBatch<SqlConnection, SqlTransaction, SqlDbType>();

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
        public DbBatch<SqlConnection, SqlTransaction, SqlDbType> CustomVersionDetectionBatch { get; private set; } = new DbBatch<SqlConnection, SqlTransaction, SqlDbType>();

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
        /// Gets the default cleanup script.
        /// </summary>
        /// <returns>
        /// The array with the commands of the default cleanup script.
        /// </returns>
        public string[] GetDefaultCleanupScript() => (string[])this.DefaultCleanupScript.Clone();

        /// <summary>
        /// Gets the default version detection script.
        /// </summary>
        /// <returns>
        /// The array with the commands of the default version detection script.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The placeholders in the default script are replaced as follows: <c>__@TableName</c> = <see cref="DefaultVersionDetectionTable"/>, <c>__@NameColumnName</c> = <see cref="DefaultVersionDetectionNameColumn"/>, <c>__@ValueColumnName</c> = <see cref="DefaultVersionDetectionValueColumn"/>, <c>__@KeyName</c> = <see cref="DefaultVersionDetectionKey"/>.
        /// </para>
        /// </remarks>
        public string[] GetDefaultVersionDetectionScript()
        {
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

        private string[] DefaultCleanupScript { get; } =
        {
            "DBCC SHRINKDATABASE (0);",
        };

        private string[] DefaultVersionDetectionScript
        {
            get
            {
                return new[]
                {
                    "IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='__@TableName') SELECT 1 ELSE SELECT 0;",
                    "IF (SELECT count(*) FROM [__@TableName] WHERE [__@NameColumnName] = '__@KeyName') = 0 SELECT -1 ELSE SELECT 1;",
                    "SELECT [__@ValueColumnName] FROM [__@TableName] WHERE [__@NameColumnName] = '__@KeyName';",
                };
            }
        }

        #endregion




        #region Instance Methods

        /// <inheritdoc cref="ICloneable.Clone" />
        public SqlServerDbManagerOptions Clone ()
        {
            SqlServerDbManagerOptions clone = new SqlServerDbManagerOptions();
            clone.ConnectionString = new SqlConnectionStringBuilder(this.ConnectionString.ToString());
            clone.CustomCleanupBatch = this.CustomCleanupBatch.Clone();
            clone.CustomCleanupBatchName = this.CustomCleanupBatchName;
            clone.CustomVersionDetectionBatch = this.CustomVersionDetectionBatch.Clone();
            clone.CustomVersionDetectionBatchName = this.CustomVersionDetectionBatchName;
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
        public string GetConnectionString() => this.ConnectionString?.ToString();

        /// <inheritdoc />
        public string VersionUpgradeNameFormat
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

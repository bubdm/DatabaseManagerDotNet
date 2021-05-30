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
    ///         The default cleanup script is (each line executed as a separate command):
    ///     </para>
    ///     <code language="sql">
    ///  <![CDATA[
    /// VACUUM;
    /// ANALYZE;
    /// REINDEX;
    ///  ]]>
    ///  </code>
    ///     <para>
    ///         The default version detection script is (each line executed as a separate command):
    ///     </para>
    ///     <code language="sql">
    ///  <![CDATA[
    /// SELECT (SELECT count(*) FROM [sqlite_master] WHERE [type] = 'table' AND [name] = '_DatabaseSettings') - 1;
    /// SELECT (SELECT count(*) FROM [_DatabaseSettings] WHERE [Name] = 'Database.Version') - 1;
    /// SELECT [Value] FROM [_DatabaseSettings] WHERE [Name] = 'Database.Version'; 
    ///  ]]>
    ///  </code>
    ///     <para>
    ///         The default create script is (each line executed as a separate command):
    ///     </para>
    ///     <code language="sql">
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
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class
        SQLiteDbManagerOptions : DbManagerOptionsBase<SQLiteDbManagerOptions, SQLiteConnectionStringBuilder>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbManagerOptions" />.
        /// </summary>
        public SQLiteDbManagerOptions () { }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbManagerOptions" />.
        /// </summary>
        /// <param name="connectionString"> The used connection string. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
        public SQLiteDbManagerOptions (string connectionString)
            : base(connectionString) { }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbManagerOptions" />.
        /// </summary>
        /// <param name="connectionString"> The used connection string. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
        public SQLiteDbManagerOptions (SQLiteConnectionStringBuilder connectionString)
            : base(connectionString) { }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     The list of custom collations used by SQLite.
        /// </summary>
        /// <value>
        ///     The list of custom collations used by SQLite.
        ///     If no custom collations are used, an empty list is returned.
        /// </value>
        public List<SQLiteFunction> CustomCollations { get; } = new List<SQLiteFunction>();

        /// <summary>
        ///     The list of custom functions used by SQLite.
        /// </summary>
        /// <value>
        ///     The list of custom functions used by SQLite.
        ///     If no custom functions are used, an empty list is returned.
        /// </value>
        public List<SQLiteFunction> CustomFunctions { get; } = new List<SQLiteFunction>();

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override string[] GetDefaultCleanupScript (out DbBatchTransactionRequirement transactionRequirement,
                                                          out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.Disallowed;
            isolationLevel = null;

            return new[]
            {
                "VACUUM;",
                "ANALYZE;",
                "REINDEX;",
            };
        }

        /// <inheritdoc />
        public override string[] GetDefaultCreationScript (out DbBatchTransactionRequirement transactionRequirement,
                                                           out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.Disallowed;
            isolationLevel = null;

            return new[]
            {
                "CREATE TABLE [_DatabaseSettings] ([Id] INTEGER PRIMARY KEY ASC ON CONFLICT ROLLBACK AUTOINCREMENT, [Name] TEXT NOT NULL ON CONFLICT ROLLBACK UNIQUE ON CONFLICT ROLLBACK, [Value] TEXT NULL);",
                "INSERT INTO [_DatabaseSettings] ([Name], [Value]) VALUES ('Database.Version', '0');",
            };
        }

        /// <inheritdoc />
        public override string[] GetDefaultVersioningScript (out DbBatchTransactionRequirement transactionRequirement,
                                                             out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.Disallowed;
            isolationLevel = null;

            return new[]
            {
                "SELECT (SELECT count(*) FROM [sqlite_master] WHERE [type] = 'table' AND [name] = '_DatabaseSettings') - 1;",
                "SELECT (SELECT count(*) FROM [_DatabaseSettings] WHERE [Name] = 'Database.Version') - 1;",
                "SELECT [Value] FROM [_DatabaseSettings] WHERE [Name] = 'Database.Version';",
            };
        }

        #endregion
    }
}

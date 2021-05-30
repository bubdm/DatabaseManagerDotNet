using System;
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
    ///         The default cleanup script is (each line executed as a separate command):
    ///     </para>
    ///     <code language="sql">
    ///  <![CDATA[
    /// DBCC SHRINKDATABASE 0
    ///  ]]>
    ///  </code>
    ///     <para>
    ///         The default version detection script is (each line executed as a separate command):
    ///     </para>
    ///     <code language="sql">
    ///  <![CDATA[
    /// IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='_DatabaseSettings') SELECT 1 ELSE SELECT 0;
    /// IF (SELECT count(*) FROM [_DatabaseSettings] WHERE [Name] = 'Database.Version') = 0 SELECT -1 ELSE SELECT 1;
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
    public sealed class
        SqlServerDbManagerOptions : DbManagerOptionsBase<SqlServerDbManagerOptions, SqlConnectionStringBuilder>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbManagerOptions" />.
        /// </summary>
        public SqlServerDbManagerOptions () { }

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbManagerOptions" />.
        /// </summary>
        /// <param name="connectionString"> The used connection string. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
        public SqlServerDbManagerOptions (string connectionString)
            : base(connectionString) { }

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbManagerOptions" />.
        /// </summary>
        /// <param name="connectionString"> The used connection string. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
        public SqlServerDbManagerOptions (SqlConnectionStringBuilder connectionString)
            : base(connectionString) { }

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
                "DBCC SHRINKDATABASE (0);",
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
                "CREATE TABLE [_DatabaseSettings] ([Id] INT PRIMARY KEY IDENTITY(1,1), [Name] NVARCHAR(1024) NOT NULL, [Value] NVARCHAR(MAX) NULL);",
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
                "IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='_DatabaseSettings') SELECT 1 ELSE SELECT 0;",
                "IF (SELECT count(*) FROM [_DatabaseSettings] WHERE [Name] = 'Database.Version') = 0 SELECT -1 ELSE SELECT 1;",
                "SELECT [Value] FROM [_DatabaseSettings] WHERE [Name] = 'Database.Version';",
            };
        }

        #endregion
    }
}

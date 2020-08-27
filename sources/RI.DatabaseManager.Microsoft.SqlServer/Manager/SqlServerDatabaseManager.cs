using Microsoft.Data.SqlClient;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Implements a database manager for SQL Server databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IDbManager" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDatabaseManager : DatabaseManager<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
    {
        #region Instance Methods

        internal SqlConnection CreateInternalConnection (string connectionStringOverride)
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(connectionStringOverride ?? this.Configuration.ConnectionString.ConnectionString);

            SqlConnection connection = new SqlConnection(connectionString.ConnectionString);
            connection.Open();

            return connection;
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override bool SupportsBackupImpl => false;

        /// <inheritdoc />
        protected override bool SupportsCleanupImpl => true;

        /// <inheritdoc />
        protected override bool SupportsConnectionTrackingImpl => true;

        /// <inheritdoc />
        protected override bool SupportsReadOnlyImpl => false;

        /// <inheritdoc />
        protected override bool SupportsRestoreImpl => false;

        /// <inheritdoc />
        protected override bool SupportsScriptsImpl => true;

        /// <inheritdoc />
        protected override bool SupportsUpgradeImpl => true;

        /// <inheritdoc />
        protected override SqlConnection CreateConnectionImpl (bool readOnly) => this.CreateInternalConnection(null);

        /// <inheritdoc />
        protected override IDatabaseProcessingStep<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration> CreateProcessingStepImpl () => new SqlServerDbProcessingStep();

        #endregion
    }
}

using System.Data.SQLite;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Implements a database manager for SQLite databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IDatabaseManager" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDatabaseManager : DatabaseManager<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
    {
        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the path to the database file.
        /// </summary>
        /// <value>
        ///     The path to the database file.
        /// </value>
        public FilePath DatabaseFile => this.Configuration.DatabaseFile;

        #endregion




        #region Instance Methods

        internal SQLiteConnection CreateInternalConnection (string connectionStringOverride, bool readOnly)
        {
            SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder(connectionStringOverride ?? this.Configuration.ConnectionString.ConnectionString);
            connectionString.ReadOnly = readOnly;

            SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString);
            connection.Open();

            if (this.Configuration.RegisterDefaultCollations)
            {
                this.RegisterCollations(connection);
            }

            if (this.Configuration.RegisterDefaultFunctions)
            {
                this.RegisterFunctions(connection);
            }

            return connection;
        }

        private void RegisterCollations (SQLiteConnection connection)
        {
            connection.BindFrameworkCollations();
        }

        private void RegisterFunctions (SQLiteConnection connection)
        {
            connection.BindFrameworkFunctions();
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override bool SupportsBackupImpl => true;

        /// <inheritdoc />
        protected override bool SupportsCleanupImpl => true;

        /// <inheritdoc />
        protected override bool SupportsConnectionTrackingImpl => true;

        /// <inheritdoc />
        protected override bool SupportsReadOnlyImpl => true;

        /// <inheritdoc />
        protected override bool SupportsRestoreImpl => true;

        /// <inheritdoc />
        protected override bool SupportsScriptsImpl => true;

        /// <inheritdoc />
        protected override bool SupportsUpgradeImpl => true;

        /// <inheritdoc />
        protected override SQLiteConnection CreateConnectionImpl (bool readOnly) => this.CreateInternalConnection(null, readOnly);

        /// <inheritdoc />
        protected override IDatabaseProcessingStep<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration> CreateProcessingStepImpl () => new SQLiteDatabaseProcessingStep();

        /// <inheritdoc />
        protected override bool DetectStateAndVersionImpl (out DatabaseState? state, out int version)
        {
            if (!this.DatabaseFile.Exists)
            {
                state = null;
                version = 0;
                return true;
            }

            if (this.DatabaseFile.Size.GetValueOrDefault(0) == 0)
            {
                state = null;
                version = 0;
                return true;
            }

            return base.DetectStateAndVersionImpl(out state, out version);
        }

        #endregion
    }
}

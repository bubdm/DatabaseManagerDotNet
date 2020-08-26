using System;
using System.Data.SQLite;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Stores the database configuration for an SQLite database manager.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IDatabaseManagerConfiguration" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDatabaseManagerConfiguration : DatabaseManagerConfiguration<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDatabaseManagerConfiguration" />.
        /// </summary>
        public SQLiteDatabaseManagerConfiguration ()
        {
            this.ConnectionString = new SQLiteConnectionStringBuilder();
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets the path to the database file.
        /// </summary>
        /// <value>
        ///     The path to the database file or null if no database file is specified.
        /// </value>
        public FilePath DatabaseFile
        {
            get
            {
                return this.ConnectionString?.DataSource?.ToNullIfNullOrEmptyOrWhitespace();
            }
            set
            {
                if (this.ConnectionString != null)
                {
                    this.ConnectionString.DataSource = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets whether default collations are automatically registered with newly created connections.
        /// </summary>
        /// <value>
        ///     true if default collations are automatically registered with newly created connections, false otherwise.
        /// </value>
        /// <remarks>
        /// <para>
        /// The default value is true. 
        /// </para>
        /// </remarks>
        public bool RegisterDefaultCollations { get; set; } = true;

        /// <summary>
        ///     Gets or sets whether default functions are automatically registered with newly created connections.
        /// </summary>
        /// <value>
        ///     true if default functions are automatically registered with newly created connections, false otherwise.
        /// </value>
        /// <remarks>
        /// <para>
        /// The default value is true. 
        /// </para>
        /// </remarks>
        public bool RegisterDefaultFunctions { get; set; } = true;

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void VerifyConfiguration (SQLiteDatabaseManager manager)
        {
            base.VerifyConfiguration(manager);

            try
            {
                FilePath filePath = new FilePath(this.ConnectionString.DataSource);
                filePath.VerifyRealFile();
            }
            catch (Exception exception)
            {
                throw new InvalidDatabaseConfigurationException("Invalid database file path: " + (this.ConnectionString.DataSource ?? "[null]") + ".", exception);
            }
        }

        #endregion
    }
}

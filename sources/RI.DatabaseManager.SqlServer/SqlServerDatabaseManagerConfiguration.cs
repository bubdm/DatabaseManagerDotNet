using Microsoft.Data.SqlClient;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Stores the database configuration for an SQL Server database manager.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IDatabaseManagerConfiguration" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDatabaseManagerConfiguration : DatabaseManagerConfiguration<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseManagerConfiguration" />.
        /// </summary>
        public SqlServerDatabaseManagerConfiguration ()
        {
            this.ConnectionString = new SqlConnectionStringBuilder();
        }

        #endregion
    }
}

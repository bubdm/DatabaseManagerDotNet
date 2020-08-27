using System.Data.Common;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Cleanup
{
    /// <summary>
    ///     Implements a base class for database cleanup processors.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <remarks>
    ///     <para>
    ///         It is recommended that database cleanup processor implementations use this base class as it already implements most of the logic which is database-independent.
    ///     </para>
    ///     <para>
    ///         See <see cref="IDatabaseCleanupProcessor" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DatabaseCleanupProcessor <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseCleanupProcessor<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {




        #region Interface: IDatabaseCleanupProcessor<TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration>

        /// <inheritdoc />
        public abstract bool RequiresScriptLocator { get; }

        /// <inheritdoc />
        public abstract bool Cleanup (TManager manager);

        /// <inheritdoc />
        bool IDatabaseCleanupProcessor.Cleanup (IDbManager manager)
        {
            return this.Cleanup((TManager)manager);
        }

        #endregion
    }
}

using System.Data.Common;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Cleanup
{
    /// <summary>
    ///     Defines the interface for database cleanup processors.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database cleanup processors are used to cleanup a database.
    ///         What the cleanup does in detail depends on the database type and the implementation of <see cref="IDatabaseCleanupProcessor" /> but is usually something like &quot;vacuum&quot;, recreate indices, etc.
    ///     </para>
    ///     <para>
    ///         Database cleanup processors are used by database managers (<see cref="IDbManager" /> implementations).
    ///         Do not use database cleanup processors directly but rather configure to use them through configuration (<see cref="IDatabaseManagerConfiguration.CleanupProcessor" />).
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDatabaseCleanupProcessor" /> are always specific for a particular type of database (or particular implementation of <see cref="IDbManager" /> respectively).
    ///     </para>
    ///     <para>
    ///         Database cleanup processors are optional.
    ///         If not configured, cleanup is not available / not supported.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseCleanupProcessor
    {
        /// <summary>
        ///     Gets whether this database cleanup processor requires a script locator.
        /// </summary>
        /// <value>
        ///     true if a script locator is required, false otherwise.
        /// </value>
        bool RequiresScriptLocator { get; }

        /// <summary>
        ///     Performs a cleanup of a database.
        /// </summary>
        /// <param name="manager"> The used database manager representing the database. </param>
        /// <returns>
        ///     true if the cleanup was successful, false otherwise.
        ///     Details must be written to the log.
        /// </returns>
        bool Cleanup (IDbManager manager);
    }

    /// <inheritdoc cref="IDatabaseCleanupProcessor" />
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseCleanupProcessor <TConnection, TTransaction, in TManager> : IDatabaseCleanupProcessor
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction, TManager>
    {
        /// <inheritdoc cref="IDatabaseCleanupProcessor.Cleanup" />
        bool Cleanup (TManager manager);
    }
}

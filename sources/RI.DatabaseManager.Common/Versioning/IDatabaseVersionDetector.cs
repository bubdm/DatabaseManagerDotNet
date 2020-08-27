using System.Data.Common;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Versioning
{
    /// <summary>
    ///     Defines the interface for database version detectors.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database version detectors are used to detect the state and version of a database.
    ///         What the version detection does in detail depends on the database type and the implementation of <see cref="IDatabaseVersionDetector" />.
    ///     </para>
    ///     <para>
    ///         Database version detectors are used by database managers (<see cref="IDbManager" /> implementations).
    ///         Do not use database version detectors directly but rather configure to use them through configuration (<see cref="IDatabaseManagerConfiguration" />.<see cref="IDatabaseManagerConfiguration.VersionDetector" />).
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDatabaseVersionDetector" /> are always specific for a particular type of database (or particular implementation of <see cref="IDbManager" /> respectively).
    ///     </para>
    ///     <para>
    ///         Database version detectors are mandatory.
    ///         A database version detector must be configured before a database manager can be used.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseVersionDetector
    {
        /// <summary>
        ///     Gets whether this database version detector requires a script locator.
        /// </summary>
        /// <value>
        ///     true if a script locator is required, false otherwise.
        /// </value>
        bool RequiresScriptLocator { get; }

        /// <summary>
        ///     Detects the state and version of a database.
        /// </summary>
        /// <param name="manager"> The used database manager representing the database. </param>
        /// <param name="state"> Returns the state of the database. Can return null to let <paramref name="manager" /> (or the implementation of <see cref="IDbManager" /> respectively) determine the state based on <paramref name="version" />. </param>
        /// <param name="version"> Returns the version of the database. Return -1 to indicate when the database is damaged or in an invalid state. Return 0 to indicate that the database does not yet exist and needs to be created. </param>
        /// <returns>
        ///     true if the state and version of the database could be successfully determined.
        /// </returns>
        bool Detect (IDbManager manager, out DbState? state, out int version);
    }

    /// <inheritdoc cref="IDatabaseVersionDetector" />
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseVersionDetector <TConnection, TTransaction, in TManager> : IDatabaseVersionDetector
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction, TManager>
    {
        /// <inheritdoc cref="IDatabaseVersionDetector.Detect" />
        bool Detect (TManager manager, out DbState? state, out int version);
    }
}

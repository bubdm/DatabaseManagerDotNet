using System.Data.Common;




namespace RI.Framework.Data.Database.Upgrading
{
    /// <summary>
    ///     Defines the interface for database version upgraders.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database version upgraders are used to upgrade a databases version from version n to version n+1.
    ///         What the upgrade does in detail depends on the database type and the implementation of <see cref="IDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         Database version upgraders are used by database managers (<see cref="IDatabaseManager" /> implementations).
    ///         Do not use database version upgraders directly but rather configure to use them through configuration (<see cref="IDatabaseManagerConfiguration.VersionUpgrader" />).
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDatabaseVersionUpgrader" /> are always specific for a particular type of database (or particular implementation of <see cref="IDatabaseManager" /> respectively).
    ///     </para>
    ///     <para>
    ///         Database version upgraders are optional.
    ///         If not configured, upgrading is not available / not supported and some versioning information about the database will not be available.
    ///     </para>
    ///     <para>
    ///         <see cref="IDatabaseVersionUpgrader" /> performs upgrades incrementally through multiple calls to <see cref="Upgrade" />.
    ///         <see cref="Upgrade" /> is always called for the current/source version and then upgrades to the current/source version + 1.
    ///         Therefore, <see cref="IDatabaseManager" /> implementations must call <see cref="Upgrade" /> as many times as necessary to upgrade incrementally from the current version to the desired version.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseVersionUpgrader
    {
        /// <summary>
        ///     Gets whether this database version upgrader requires a script locator.
        /// </summary>
        /// <value>
        ///     true if a script locator is required, false otherwise.
        /// </value>
        bool RequiresScriptLocator { get; }

        /// <summary>
        ///     Gets the highest supported/known version of this database version upgrader.
        /// </summary>
        /// <param name="manager"> The used database manager representing the database. </param>
        /// <returns>
        ///     The highest supported/known version of this database version upgrader.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetMaxVersion" /> always represents a target version, not a source version, meaning that <see cref="GetMaxVersion" /> is the highest version to which can be upgraded.
        ///     </note>
        /// </remarks>
        int GetMaxVersion (IDatabaseManager manager);

        /// <summary>
        ///     Gets the lowest supported/known version of this database version upgrader.
        /// </summary>
        /// <param name="manager"> The used database manager representing the database. </param>
        /// <returns>
        ///     The lowest supported/known version of this database version upgrader.
        ///     Zero means that the database version upgrader supports creating a new database if it does not exist.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetMinVersion" /> always represents a source version, not a target version, meaning that <see cref="GetMinVersion" /> is the lowest version from which can be upgraded.
        ///     </note>
        /// </remarks>
        int GetMinVersion (IDatabaseManager manager);

        /// <summary>
        ///     Upgrades a database version from a specified version to the next version.
        /// </summary>
        /// <param name="manager"> The used database manager representing the database. </param>
        /// <param name="sourceVersion"> The source version to upgrade from. </param>
        /// <returns>
        ///     true if the upgrade was successful, false otherwise.
        ///     Details must be written to the log.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         The upgrade must be exactly from <paramref name="sourceVersion" /> to <paramref name="sourceVersion" /> + 1.
        ///     </note>
        /// </remarks>
        bool Upgrade (IDatabaseManager manager, int sourceVersion);
    }

    /// <inheritdoc cref="IDatabaseVersionUpgrader" />
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    public interface IDatabaseVersionUpgrader <TConnection, TTransaction, TConnectionStringBuilder, in TManager, TConfiguration> : IDatabaseVersionUpgrader
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        /// <inheritdoc cref="IDatabaseVersionUpgrader.GetMaxVersion" />
        int GetMaxVersion (TManager manager);

        /// <inheritdoc cref="IDatabaseVersionUpgrader.GetMinVersion" />
        int GetMinVersion (TManager manager);

        /// <inheritdoc cref="IDatabaseVersionUpgrader.Upgrade" />
        bool Upgrade (TManager manager, int sourceVersion);
    }
}

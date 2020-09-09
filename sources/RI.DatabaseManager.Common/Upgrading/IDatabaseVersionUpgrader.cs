using System;
using System.Data.Common;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Database version upgrader to upgrade database schemas to newer versions.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database version upgraders are used to upgrade a databases version from version n to version n+1.
    ///         What the upgrade does in detail depends on the database type and the implementation of <see cref="IDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDatabaseVersionUpgrader" /> are always specific for a particular type of database (or particular implementation of <see cref="IDbManager" /> respectively).
    ///     </para>
    ///     <para>
    ///         <see cref="IDatabaseVersionUpgrader" /> performs upgrades incrementally through multiple calls to <see cref="Upgrade" />.
    ///         <see cref="Upgrade" /> is always called for the current/source version and then upgrades to the current/source version + 1.
    ///         Therefore, database managers must call <see cref="Upgrade" /> as many times as necessary to upgrade incrementally from the current version to the desired version.
    ///     </para>
    ///     <note type="note">
    ///         Database version upgraders are optional.
    ///         If not configured, upgrading is not available / not supported and some versioning information about the database will not be available.
    ///     </note>
    ///     <note type="note">
    /// <see cref="IDatabaseVersionUpgrader"/> implementations are used by database managers.
    ///         Do not use <see cref="IDatabaseVersionUpgrader"/> implementations directly.
    /// Use <see cref="IDbManager.Upgrade"/> instead.
    ///     </note>
    /// </remarks>
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
        ///     Gets the highest supported/known version this database version upgrader can upgrade to.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <returns>
        ///     The highest supported/known version of this database version upgrader.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetMaxVersion" /> always represents a target version, not a source version, meaning that <see cref="GetMaxVersion" /> is the highest version to which can be upgraded.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="manager"/> is null.</exception>
        int GetMaxVersion (IDbManager manager);

        /// <summary>
        ///     Gets the lowest supported/known version this database version upgrader can upgrade from.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <returns>
        ///     The lowest supported/known version of this database version upgrader.
        ///     Zero means that the database version upgrader supports creating a new database if it does not exist.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetMinVersion" /> always represents a source version, not a target version, meaning that <see cref="GetMinVersion" /> is the lowest version from which can be upgraded.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="manager"/> is null.</exception>
        int GetMinVersion (IDbManager manager);

        /// <summary>
        ///     Upgrades a database version from a specified version to the next version.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="sourceVersion"> The source version to upgrade from. </param>
        /// <returns>
        ///     true if the upgrade was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         The upgrade must be exactly from <paramref name="sourceVersion" /> to <paramref name="sourceVersion" /> + 1.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="manager"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sourceVersion"/> is less than zero, less than <see cref="GetMinVersion"/>, or equal or greater than <see cref="GetMaxVersion"/>.</exception>
        bool Upgrade (IDbManager manager, int sourceVersion);
    }

    /// <inheritdoc cref="IDatabaseVersionUpgrader" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    public interface IDatabaseVersionUpgrader <TConnection, TTransaction> : IDatabaseVersionUpgrader
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        /// <inheritdoc cref="IDatabaseVersionUpgrader.GetMaxVersion" />
        int GetMaxVersion (IDbManager<TConnection, TTransaction> manager);

        /// <inheritdoc cref="IDatabaseVersionUpgrader.GetMinVersion" />
        int GetMinVersion (IDbManager<TConnection, TTransaction> manager);

        /// <inheritdoc cref="IDatabaseVersionUpgrader.Upgrade" />
        bool Upgrade (IDbManager<TConnection, TTransaction> manager, int sourceVersion);
    }
}

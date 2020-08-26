using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Scripts;
using RI.Framework.Data.Database.Upgrading;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Defines the interface for a database manager.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A database manager encapsulates low-level database functionality so that high-level database functionality (such as <see cref="IRepositoryContext" />) can be implemented on top without having to worry about low-level and/or database-specific details.
    ///         A database manager is intended to handle all low-level database stuff, such as connection management, versioning, migration, backup, etc., so that the high-level functionality can focus on the actual task of working with the data or accessing the database respectively.
    ///     </para>
    ///     <para>
    ///         The link from the database manager to higher-level functionality are the database connections which can be created using <see cref="CreateConnection" />.
    ///     </para>
    ///     <para>
    ///         The database cannot be used if it is in any other state than <see cref="DatabaseState.ReadyUnknown" />, <see cref="DatabaseState.ReadyNew" />, or <see cref="DatabaseState.ReadyOld" />.
    ///         However, there are three exceptions:
    ///         Cleanups (<see cref="Cleanup"/>) and Upgrades (<see cref="Upgrade(int)" />, <see cref="Upgrade()" />) are also possible in the <see cref="DatabaseState.New" /> state.
    ///         Backups (<see cref="Backup" />) and Restores (<see cref="Restore" />) are possible in any state except <see cref="DatabaseState.Uninitialized" />.
    ///     </para>
    ///     <para>
    ///         If the database supports connection tracking (see <see cref="SupportsConnectionTracking" />), <see cref="Close" /> or <see cref="CloseTrackedConnections" /> will close all currently non-closed connections which were created using <see cref="CreateConnection" />.
    ///     </para>
    ///     <note type="implement">
    ///         While a database is initialized, its configuration (<see cref="Configuration" />) can be changed.
    ///         However, it is recommend to change configuration only when the database is in the uninitialized state.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseManager : IDisposable
    {
        /// <summary>
        ///     Gets whether the database is in a state where it can be upgraded to a newer version.
        /// </summary>
        /// <value>
        ///     true if the database supports upgrading, is in a ready or the new state, and the current version is less than the maximum supported version, false otherwise.
        /// </value>
        bool CanUpgrade { get; }

        /// <summary>
        ///     Gets the configuration used by this database.
        /// </summary>
        /// <value>
        ///     The configuration used by this database.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         This property must never be null.
        ///     </note>
        /// </remarks>
        IDatabaseManagerConfiguration Configuration { get; }

        /// <summary>
        ///     Gets the state of the database after initialization.
        /// </summary>
        /// <value>
        ///     The state of the database after initialization.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         This property is set during <see cref="Initialize" /> and reset during <see cref="Close" />.
        ///     </para>
        ///     <para>
        ///         See <see cref="State" /> for more details.
        ///     </para>
        /// </remarks>
        DatabaseState InitialState { get; }

        /// <summary>
        ///     Gets the version of the database after initialization.
        /// </summary>
        /// <value>
        ///     The version of the database after initialization.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         This property is set during <see cref="Initialize" /> and reset during <see cref="Close" />.
        ///     </para>
        ///     <para>
        ///         See <see cref="Version" /> for more details.
        ///     </para>
        /// </remarks>
        int InitialVersion { get; }

        /// <summary>
        ///     Gets whether the database is ready for use and connections can be created or backups and cleanups can be performed.
        /// </summary>
        /// <value>
        ///     true if the database is in <see cref="DatabaseState.ReadyUnknown" />, <see cref="DatabaseState.ReadyNew" />, or <see cref="DatabaseState.ReadyOld" /> state, false otherwise.
        /// </value>
        bool IsReady { get; }

        /// <summary>
        ///     Gets the highest version supported for upgrading.
        /// </summary>
        /// <value>
        ///     The highest version supported for upgrading (as a target version) or -1 if upgrading is not supported.
        /// </value>
        int MaxVersion { get; }

        /// <summary>
        ///     Gets the lowest version supported for upgrading.
        /// </summary>
        /// <value>
        ///     The lowest version supported for upgrading (as a source version) or -1 if upgrading is not supported.
        /// </value>
        int MinVersion { get; }

        /// <summary>
        ///     Gets the current state of the database.
        /// </summary>
        /// <value>
        ///     The current state of the database.
        /// </value>
        DatabaseState State { get; }

        /// <summary>
        ///     Gets whether the database supports the backup functionality.
        /// </summary>
        /// <value>
        ///     true if the database supports backup, false otherwise.
        /// </value>
        bool SupportsBackup { get; }

        /// <summary>
        ///     Gets whether the database supports the cleanup functionality.
        /// </summary>
        /// <value>
        ///     true if the database supports cleanup, false otherwise.
        /// </value>
        bool SupportsCleanup { get; }

        /// <summary>
        ///     Gets whether the database supports connection tracking.
        /// </summary>
        /// <value>
        ///     true if the database supports connection tracking, false otherwise.
        /// </value>
        bool SupportsConnectionTracking { get; }

        /// <summary>
        ///     Gets whether the database supports read-only connections.
        /// </summary>
        /// <value>
        ///     true if the database supports read-only connections, false otherwise.
        /// </value>
        bool SupportsReadOnly { get; }

        /// <summary>
        ///     Gets whether the database supports the restore functionality.
        /// </summary>
        /// <value>
        ///     true if the database supports restore, false otherwise.
        /// </value>
        bool SupportsRestore { get; }

        /// <summary>
        ///     Gets whether the database supports script retrieval.
        /// </summary>
        /// <value>
        ///     true if the database supports script retrieval, false otherwise.
        /// </value>
        bool SupportsScripts { get; }

        /// <summary>
        ///     Gets whether the database supports the upgrade functionality.
        /// </summary>
        /// <value>
        ///     true if the database supports upgrading, false otherwise.
        /// </value>
        bool SupportsUpgrade { get; }

        /// <summary>
        ///     Gets the current version of the database.
        /// </summary>
        /// <value>
        ///     The current version of the database.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         Any version below zero (usually -1) indicates an invalid version or a damaged database respectively.
        ///     </para>
        ///     <para>
        ///         A version of zero indicates that the database does not yet exist and must be created before it can be used.
        ///     </para>
        /// </remarks>
        int Version { get; }

        /// <summary>
        ///     Raised when the state of a tracked connection has changed.
        /// </summary>
        event EventHandler<DatabaseConnectionChangedEventArgs> ConnectionChanged;

        /// <summary>
        ///     Raised when a connection has been created using <see cref="CreateConnection" />.
        /// </summary>
        event EventHandler<DatabaseConnectionCreatedEventArgs> ConnectionCreated;

        /// <summary>
        ///     Raised when a script has been retrieved using <see cref="GetScriptBatch" />.
        /// </summary>
        event EventHandler<DatabaseScriptRetrievedEventArgs> ScriptRetrieved;

        /// <summary>
        ///     Raised when the state of this database has changed (<see cref="State" />).
        /// </summary>
        event EventHandler<DatabaseStateChangedEventArgs> StateChanged;

        /// <summary>
        ///     Raised when the version of this database has changed (<see cref="Version" />).
        /// </summary>
        event EventHandler<DatabaseVersionChangedEventArgs> VersionChanged;

        /// <summary>
        ///     Performs a backup using the configured <see cref="IDatabaseBackupCreator" />.
        /// </summary>
        /// <param name="backupTarget"> The backup creator specific object which describes the backup target. </param>
        /// <returns>
        ///     true if the backup was successful, false otherwise.
        ///     Details can be obtained from the log.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the current state and version of the database after backup.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="backupTarget" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="backupTarget" /> is of a type which is not supported by the used backup creator. </exception>
        /// <exception cref="InvalidOperationException"> The database is not initialized. </exception>
        /// <exception cref="NotSupportedException"> Backup is not supported by the database or no <see cref="IDatabaseBackupCreator" /> is configured. </exception>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        bool Backup (object backupTarget);

        /// <summary>
        ///     Performs a database cleanup using the configured <see cref="IDatabaseCleanupProcessor" />.
        /// </summary>
        /// <returns>
        ///     true if the cleanup was successful, false otherwise.
        ///     Details can be obtained from the log.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the current state and version of the database after cleanup.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The database is not in a ready or the new state. </exception>
        /// <exception cref="NotSupportedException"> Cleanup is not supported by the database or no <see cref="IDatabaseCleanupProcessor" /> is configured. </exception>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        bool Cleanup ();

        /// <summary>
        ///     Closes the database.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="State" /> and <see cref="InitialState" /> are set to <see cref="DatabaseState.Uninitialized" /> and <see cref="Version" /> and <see cref="InitialVersion" /> are set to -1.
        ///     </para>
        ///     <note type="note">
        ///         All tracked connections will be closed.
        ///         Untracked connections remain unchanged.
        ///     </note>
        /// </remarks>
        void Close ();

        /// <summary>
        ///     Closes all currently tracked connections.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Untracked connections remain unchanged.
        ///     </para>
        /// </remarks>
        void CloseTrackedConnections ();

        /// <summary>
        ///     Creates a new connection which can be used to work with the database.
        /// </summary>
        /// <param name="readOnly"> Specifies whether the connection should be read-only. </param>
        /// <param name="track"> Specifies whether the connection should be tracked. </param>
        /// <returns>
        ///     The newly created and already opened connection or null if the connection could not be created.
        ///     Details can be obtained from the log.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The database is not in a ready state. </exception>
        /// <exception cref="NotSupportedException"> <paramref name="readOnly" /> is true but read-only connections are not supported or <paramref name="track" /> is true but connection tracking is not supported. </exception>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        DbConnection CreateConnection (bool readOnly, bool track);

        /// <summary>
        ///     Creates a new processing step which can be used to perform database actions.
        /// </summary>
        /// <returns>
        ///     The newly created processing step.
        /// </returns>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        IDatabaseProcessingStep CreateProcessingStep ();

        /// <summary>
        ///     Retrieves a script batch using the configured <see cref="IDatabaseScriptLocator" />.
        /// </summary>
        /// <param name="name"> The name of the script. </param>
        /// <param name="preprocess"> Specifies whether the script is to be preprocessed. </param>
        /// <returns>
        ///     The list of batches in the script.
        ///     If the script is empty or does not contain any batches respectively, an empty list is returned.
        ///     If the script could not be found, null is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidOperationException"> The database is not initialized. </exception>
        /// <exception cref="NotSupportedException"> Retrieving scripts is not supported by the database or no <see cref="IDatabaseScriptLocator" /> is configured. </exception>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        List<string> GetScriptBatch (string name, bool preprocess);

        /// <summary>
        ///     Gets a list of all tracked connections.
        /// </summary>
        /// <returns>
        ///     The list of tracked connections or null if connection tracking is not supported.
        ///     If connection tracking is supported but no connections are currently being tracked, an empty list is returned.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         Connections which are in a <see cref="ConnectionState.Closed" /> or <see cref="ConnectionState.Broken" /> state must be removed from the list of tracked connections.
        ///     </note>
        /// </remarks>
        List<DbConnection> GetTrackedConnections ();

        /// <summary>
        ///     Initializes the database.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="State" /> and <see cref="Version" /> as well as <see cref="InitialState" /> and <see cref="InitialVersion" /> are updated to reflect the current state and version of the database after initialization.
        ///     </para>
        ///     <note type="note">
        ///         If the database is already initialized, it will be closed first (implicitly calling <see cref="Close" />).
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        void Initialize ();

        /// <summary>
        ///     Performs a backup using the configured <see cref="IDatabaseBackupCreator" />.
        /// </summary>
        /// <param name="backupSource"> The backup creator specific object which describes the backup source. </param>
        /// <returns>
        ///     true if the restore was successful, false otherwise.
        ///     Details can be obtained from the log.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the current state and version of the database after restore.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="backupSource" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> <paramref name="backupSource" /> is of a type which is not supported by the used backup creator. </exception>
        /// <exception cref="InvalidOperationException"> The database is not initialized. </exception>
        /// <exception cref="NotSupportedException"> Restore is not supported by the database or no <see cref="IDatabaseBackupCreator" /> is configured. </exception>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        bool Restore (object backupSource);

        /// <summary>
        ///     Performs an upgrade to a specific database version using the configured <see cref="IDatabaseVersionUpgrader" />.
        /// </summary>
        /// <param name="version"> The version to upgrade the database to. </param>
        /// <returns>
        ///     true if the upgrade was successful, false otherwise.
        ///     Details can be obtained from the log.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the current state and version of the database after upgrade.
        ///     </para>
        ///     <para>
        ///         If <paramref name="version" /> is the same as <see cref="Version" />, nothing is done.
        ///     </para>
        ///     <note type="implement">
        ///         Upgrading is to be performed incrementally, upgrading from n to n+1 until the desired version, as specified by <paramref name="version" />, is reached.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The database is not in a ready or the new state. </exception>
        /// <exception cref="NotSupportedException"> Upgrading is not supported by the database or no <see cref="IDatabaseVersionUpgrader" /> is configured. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="version" /> is less than <see cref="MinVersion" />, greater than <see cref="MaxVersion" />, or less than <see cref="Version" />. </exception>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        bool Upgrade (int version);

        /// <summary>
        ///     Performs an upgrade to highest supported version using the configured <see cref="IDatabaseVersionUpgrader" />.
        /// </summary>
        /// <returns>
        ///     true if the upgrade was successful, false otherwise.
        ///     Details can be obtained from the log.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the current state and version of the database after upgrade.
        ///     </para>
        ///     <para>
        ///         If <see cref="MaxVersion" /> is the same as <see cref="Version" />, nothing is done.
        ///     </para>
        ///     <note type="implement">
        ///         Upgrading is to be performed incrementally, upgrading from n to n+1 until the desired version, <see cref="MaxVersion" />, is reached.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The database is not in a ready or the new state. </exception>
        /// <exception cref="NotSupportedException"> Upgrading is not supported by the database or no <see cref="IDatabaseVersionUpgrader" /> is configured. </exception>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration specified by <see cref="Configuration" /> is not valid. </exception>
        bool Upgrade ();
    }

    /// <inheritdoc cref="IDatabaseManager" />
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseManager <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseManager
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        /// <inheritdoc cref="IDatabaseManager.Configuration" />
        new IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> Configuration { get; }

        /// <inheritdoc cref="IDatabaseManager.ConnectionChanged" />
        new event EventHandler<DatabaseConnectionChangedEventArgs<TConnection>> ConnectionChanged;

        /// <inheritdoc cref="IDatabaseManager.ConnectionCreated" />
        new event EventHandler<DatabaseConnectionCreatedEventArgs<TConnection>> ConnectionCreated;

        /// <inheritdoc cref="IDatabaseManager.CreateConnection" />
        new TConnection CreateConnection (bool readOnly, bool track);

        /// <inheritdoc cref="IDatabaseManager.CreateProcessingStep" />
        new IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> CreateProcessingStep ();

        /// <inheritdoc cref="IDatabaseManager.GetTrackedConnections" />
        new List<TConnection> GetTrackedConnections ();
    }
}

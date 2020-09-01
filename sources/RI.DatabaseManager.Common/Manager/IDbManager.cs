using System;
using System.Collections.Generic;
using System.Data.Common;

using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Scripts;
using RI.DatabaseManager.Upgrading;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     The database manager.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A database manager encapsulates low-level database management functionality so that high-level database functionality (such as repositories, Entity Framework contexts, Dapper, etc.) can be used on top without having to worry about database management.
    ///         A database manager is intended to handle all low-level database management, such as connection and database creation, versioning, migration, backup, etc., so that the high-level functionality can focus on the actual task of working with the data or accessing the database respectively.
    ///     </para>
    ///     <para>
    ///         The link from the database manager to higher-level functionality are the database connections which can be created using <see cref="CreateConnection" />.
    ///     </para>
    ///     <para>
    ///         The database cannot be used if it is in any other state than <see cref="DbState.ReadyUnknown" />, <see cref="DbState.ReadyNew" />, or <see cref="DbState.ReadyOld" />.
    ///         However, there are three exceptions:
    ///         Cleanups (<see cref="Cleanup"/>) and Upgrades (<see cref="Upgrade(int)" />) are also possible in the <see cref="DbState.New" /> state.
    ///         Backups (<see cref="Backup" />) and Restores (<see cref="Restore" />) are possible in any state except <see cref="DbState.Uninitialized" />.
    ///     </para>
    ///     <para>
    ///         A database manager, and all its dependencies, makes no assumptions or requirements regarding the used threading model. It passes through to the underlying database provider ina threading-agnostic way.
    ///     </para>
    /// </remarks>
    public interface IDbManager : IDisposable
    {
        /// <summary>
        ///     Gets whether the database is in a state where it can be upgraded to a newer version.
        /// </summary>
        /// <value>
        ///     true if the database supports upgrading, is in a ready or the new state, and the current version is less than the maximum supported version, false otherwise.
        /// </value>
        bool CanUpgrade { get; }

        /// <summary>
        ///     Gets the state of the database after initialization.
        /// </summary>
        /// <value>
        ///     The state of the database after initialization.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         This property is set during <see cref="Initialize" /> and reset during <see cref="Close" />.
        ///     </note>
        /// </remarks>
        DbState InitialState { get; }

        /// <summary>
        ///     Gets the version of the database after initialization.
        /// </summary>
        /// <value>
        ///     The version of the database after initialization.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         This property is set during <see cref="Initialize" /> and reset during <see cref="Close" />.
        ///     </note>
        /// </remarks>
        int InitialVersion { get; }

        /// <summary>
        ///     Gets whether the database is ready for use and connections can be created and backups, restores, and cleanups can be performed (depending on what is actually supported).
        /// </summary>
        /// <value>
        ///     true if the database is in <see cref="DbState.ReadyUnknown" />, <see cref="DbState.ReadyNew" />, or <see cref="DbState.ReadyOld" /> state, false otherwise.
        /// </value>
        bool IsReady { get; }

        /// <summary>
        ///     Gets the highest version supported for upgrading.
        /// </summary>
        /// <value>
        ///     The highest version supported for upgrading (as a target version) or -1 if upgrading is not supported.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="MaxVersion"/> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        int MaxVersion { get; }

        /// <summary>
        ///     Gets the lowest version supported for upgrading.
        /// </summary>
        /// <value>
        ///     The lowest version supported for upgrading (as a source version) or -1 if upgrading is not supported.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="MinVersion"/> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        int MinVersion { get; }

        /// <summary>
        ///     Gets the current state of the database.
        /// </summary>
        /// <value>
        ///     The current state of the database.
        /// </value>
        DbState State { get; }

        /// <summary>
        ///     Gets whether the database manager supports the backup functionality.
        /// </summary>
        /// <value>
        ///     true if the database manager supports backup, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsBackup"/> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsBackup { get; }

        /// <summary>
        ///     Gets whether the database manager supports the cleanup functionality.
        /// </summary>
        /// <value>
        ///     true if the database manager supports cleanup, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsCleanup"/> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsCleanup { get; }

        /// <summary>
        ///     Gets whether the database manager supports read-only connections.
        /// </summary>
        /// <value>
        ///     true if the database manager supports read-only connections, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsReadOnly"/> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsReadOnly { get; }

        /// <summary>
        ///     Gets whether the database manager supports the restore functionality.
        /// </summary>
        /// <value>
        ///     true if the database manager supports restore, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsRestore"/> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsRestore { get; }

        /// <summary>
        ///     Gets whether the database manager supports script retrieval.
        /// </summary>
        /// <value>
        ///     true if the database manager supports script retrieval, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsScripts"/> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsScripts { get; }

        /// <summary>
        ///     Gets whether the database manager supports the upgrade functionality.
        /// </summary>
        /// <value>
        ///     true if the database manager supports upgrading, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsUpgrade"/> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsUpgrade { get; }

        /// <summary>
        ///     Gets the current version of the database.
        /// </summary>
        /// <value>
        ///     The current version of the database.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         Any version below zero (usually -1) indicates an invalid version or a damaged database respectively.
        ///     </note>
        ///     <note type="implement">
        ///         A version of zero indicates that the database does not yet exist and must be created before it can be used.
        ///     </note>
        /// </remarks>
        int Version { get; }

        /// <summary>
        ///     Performs a backup using the configured <see cref="IDatabaseBackupCreator" />.
        /// </summary>
        /// <param name="backupTarget"> The backup creator specific object which abstracts the backup target. </param>
        /// <returns>
        ///     true if the backup was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" />, <see cref="Version" />, <see cref="IsReady"/>, <see cref="CanUpgrade"/> are updated to reflect the current state and version of the database after restore.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="backupTarget" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="backupTarget" /> is of a type which is not supported by the used <see cref="IDatabaseBackupCreator"/>. </exception>
        /// <exception cref="InvalidOperationException"> The database is not initialized. </exception>
        /// <exception cref="NotSupportedException"> Backup is not supported by the database manager or no <see cref="IDatabaseBackupCreator" /> is configured. </exception>
        bool Backup (object backupTarget);

        /// <summary>
        ///     Performs a database cleanup using the configured <see cref="IDatabaseCleanupProcessor" />.
        /// </summary>
        /// <returns>
        ///     true if the cleanup was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" />, <see cref="Version" />, <see cref="IsReady"/>, <see cref="CanUpgrade"/> are updated to reflect the current state and version of the database after cleanup.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The database is not in a ready or the new state. </exception>
        /// <exception cref="NotSupportedException"> Cleanup is not supported by the database manager or no <see cref="IDatabaseCleanupProcessor" /> is configured. </exception>
        bool Cleanup ();

        /// <summary>
        ///     Closes the database manager.
        /// </summary>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" /> and <see cref="InitialState" /> are set to <see cref="DbState.Uninitialized" />, <see cref="Version" /> and <see cref="InitialVersion" /> are set to -1, <see cref="IsReady"/> and <see cref="CanUpgrade"/> are set to false.
        ///     </note>
        ///     <note type="implement">
        ///         <see cref="Close"/> should be callable multiple times and independent of the current state and version.
        ///     </note>
        /// </remarks>
        void Close ();

        /// <summary>
        ///     Creates a new connection which can be used to work with the database.
        /// </summary>
        /// <param name="readOnly"> Specifies whether the connection should be read-only. </param>
        /// <returns>
        ///     The newly created and already opened connection or null if the connection could not be created.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The database is not in a ready state. </exception>
        /// <exception cref="NotSupportedException"> <paramref name="readOnly" /> is true but read-only connections are not supported. </exception>
        DbConnection CreateConnection (bool readOnly);

        /// <summary>
        ///     Creates a new processing step which can be used to perform database actions.
        /// </summary>
        /// <returns>
        ///     The newly created processing step.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="CreateProcessingStep"/> should be callable at any time as the created processing step is just created, not executed.
        ///     </note>
        /// </remarks>
        IDbProcessingStep CreateProcessingStep ();

        /// <summary>
        ///     Retrieves a script and all its batches using the configured <see cref="IDbScriptLocator" />.
        /// </summary>
        /// <param name="name"> The name of the script. </param>
        /// <param name="batchSeparator"> The string which is used as the separator to separate individual batches in the script or null if the script locators default separator is to be used. </param>
        /// <param name="preprocess"> Specifies whether the script is to be preprocessed, if applicable. </param>
        /// <returns>
        ///     The batches in the script (list of independently executed commands).
        ///     If the script is empty or does not contain any commands respectively, an empty list is returned.
        ///     If the script could not be found, null is returned.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetScriptBatches"/> should be callable at any time as the retrieved script is just retrieved, not executed.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> or <paramref name="batchSeparator"/> is an empty string. </exception>
        /// <exception cref="NotSupportedException"> Retrieving scripts is not supported by the database manager or no <see cref="IDbScriptLocator" /> is configured. </exception>
        List<string> GetScriptBatches (string name, string batchSeparator, bool preprocess);

        /// <summary>
        ///     Initializes the database manager.
        /// </summary>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" />, <see cref="Version" />, <see cref="IsReady"/>, <see cref="CanUpgrade"/>, <see cref="InitialState"/>, <see cref="InitialVersion"/> are updated to reflect the current state and version of the database after initialization.
        ///     </note>
        ///     <note type="implement">
        ///         If the database is already initialized, it will be closed first (implicitly calling <see cref="Close" />).
        ///     </note>
        ///     <note type="implement">
        ///         <see cref="Initialize"/> should be callable multiple times and independent of the current state and version.
        ///     </note>
        /// </remarks>
        void Initialize ();

        /// <summary>
        ///     Performs a restore using the configured <see cref="IDatabaseBackupCreator" />.
        /// </summary>
        /// <param name="backupSource"> The backup creator specific object which abstracts the backup source. </param>
        /// <returns>
        ///     true if the restore was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" />, <see cref="Version" />, <see cref="IsReady"/>, <see cref="CanUpgrade"/> are updated to reflect the current state and version of the database after restore.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="backupSource" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="backupSource" /> is of a type which is not supported by the used <see cref="IDatabaseBackupCreator"/>. </exception>
        /// <exception cref="InvalidOperationException"> The database is not initialized. </exception>
        /// <exception cref="NotSupportedException"> Restore is not supported by the database manager or no <see cref="IDatabaseBackupCreator" /> is configured. </exception>
        bool Restore (object backupSource);

        /// <summary>
        ///     Performs an upgrade to a specific database version using the configured <see cref="IDatabaseVersionUpgrader" />.
        /// </summary>
        /// <param name="version"> The version to upgrade the database to. </param>
        /// <returns>
        ///     true if the upgrade was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" />, <see cref="Version" />, <see cref="IsReady"/>, <see cref="CanUpgrade"/> are updated to reflect the current state and version of the database after upgrade.
        ///     </note>
        ///     <note type="implement">
        ///         If <paramref name="version" /> is the same as <see cref="Version" />, nothing should be done.
        ///     </note>
        ///     <note type="implement">
        ///         Upgrading is to be performed incrementally, upgrading from n to n+1 until the desired version, as specified by <paramref name="version" />, is reached.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The database is not in a ready or the new state. </exception>
        /// <exception cref="NotSupportedException"> Upgrading is not supported by the database manager or no <see cref="IDatabaseVersionUpgrader" /> is configured. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="version" /> is less than <see cref="MinVersion" />, greater than <see cref="MaxVersion" />, or less than <see cref="Version" />. </exception>
        bool Upgrade (int version);
    }

    /// <inheritdoc cref="IDbManager" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    public interface IDbManager <TConnection, TTransaction, TManager> : IDbManager
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction, TManager>
    {
        /// <inheritdoc cref="IDbManager.CreateConnection" />
        new TConnection CreateConnection (bool readOnly);

        /// <inheritdoc cref="IDbManager.CreateProcessingStep" />
        new IDbProcessingStep<TConnection, TTransaction, TManager> CreateProcessingStep ();
    }
}

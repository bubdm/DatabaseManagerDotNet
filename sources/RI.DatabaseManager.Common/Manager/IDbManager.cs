using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder.Options;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Creation;
using RI.DatabaseManager.Upgrading;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     The database manager.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A database manager encapsulates low-level database management functionality so that high-level database functionality (such as repositories, Entity Framework contexts, Dapper, etc.) can be used on top without having to worry about database management (e.g. versioning).
    ///         A database manager is intended to handle all low-level database management, such as connection and database creation, versioning, migration, backup, etc., so that the high-level functionality can focus on the actual task of working with the data.
    ///     </para>
    ///     <para>
    ///         The link from the database manager to higher-level functionality are the database connections and/or transactions which can be created using <see cref="CreateConnection" /> and <see cref="CreateTransaction" />.
    ///     </para>
    ///     <para>
    ///         Batches (<see cref="IDbBatch" />) can be used to group multiple commands (<see cref="IDbBatchCommand" />) into one unit.
    ///         Some dependencies (e.g. version detectors) might use batches to retrieve the commands required to execute their functionality.
    ///     </para>
    ///     <para>
    ///         The database cannot be used if it is in any other state than <see cref="DbState.ReadyUnknown" />, <see cref="DbState.ReadyNew" />, or <see cref="DbState.ReadyOld" />.
    ///         However, there are a few exceptions:
    ///         Cleanups (<see cref="Cleanup" />) and Upgrades (<see cref="Upgrade(int)" />, <see cref="IDbManagerExtensions.Upgrade" />) are also possible in the <see cref="DbState.New" /> state.
    ///         Backups (<see cref="Backup" />) and Restores (<see cref="Restore" />) are possible in any state except <see cref="DbState.Uninitialized" />.
    ///     </para>
    ///     <note type="note">
    ///         A database manager, and all its dependencies, makes no assumptions or requirements regarding the used threading model.
    ///         It passes through to the underlying database provider in a threading-agnostic way.
    ///     </note>
    /// </remarks>
    public interface IDbManager : IDisposable
    {
        /// <summary>
        /// Gets a copy of the used database options.
        /// </summary>
        /// <returns>
        /// The copy of the used database options or null if no options are used.
        /// </returns>
        /// <remarks>
        ///<note type="note">
        /// The returned options is a copy.
        /// Changing its values will not have any effect to the database managers operations.
        /// </note>
        /// </remarks>
        IDbManagerOptions GetOptions ();

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
        ///     Gets the highest version supported for upgrading.
        /// </summary>
        /// <value>
        ///     The highest version supported for upgrading (as a target version) or -1 if upgrading is not supported.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         This property is set during <see cref="Initialize" /> and reset during <see cref="Close" />.
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
        ///         This property is set during <see cref="Initialize" /> and reset during <see cref="Close" />.
        ///     </note>
        /// </remarks>
        int MinVersion { get; }

        /// <summary>
        ///     Gets the current version of the database.
        /// </summary>
        /// <value>
        ///     The current version of the database.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         A version of 0 indicates that the database does not yet exist and must be created before it can be used.
        ///     </note>
        ///     <note type="implement">
        ///         A version of -1 indicates an invalid version or a damaged database respectively.
        ///     </note>
        /// </remarks>
        int Version { get; }

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
        ///         <see cref="SupportsBackup" /> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsBackup { get; }

        /// <summary>
        ///     Gets whether the database manager supports the restore functionality.
        /// </summary>
        /// <value>
        ///     true if the database manager supports restore, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsRestore" /> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsRestore { get; }

        /// <summary>
        ///     Gets whether the database manager supports the cleanup functionality.
        /// </summary>
        /// <value>
        ///     true if the database manager supports cleanup, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsCleanup" /> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsCleanup { get; }

        /// <summary>
        ///     Gets whether the database manager supports creating the database.
        /// </summary>
        /// <value>
        ///     true if the database manager supports database creation, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsCreate" /> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsCreate { get; }

        /// <summary>
        ///     Gets whether the database manager supports the upgrade functionality.
        /// </summary>
        /// <value>
        ///     true if the database manager supports upgrading, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsUpgrade" /> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsUpgrade { get; }

        /// <summary>
        ///     Gets whether the database manager supports read-only connections.
        /// </summary>
        /// <value>
        ///     true if the database manager supports read-only connections, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="SupportsReadOnlyConnections" /> should be set during the construction of the database manager and then remain unchanged and independent of the current database state or version (only depending on the database manager configuration).
        ///     </note>
        /// </remarks>
        bool SupportsReadOnlyConnections { get; }

        /// <summary>
        ///     Performs a backup using the configured <see cref="IDbBackupCreator" />.
        /// </summary>
        /// <param name="backupTarget"> The backup creator specific object which abstracts the backup target (e.g. a stream or a path to a file). </param>
        /// <returns>
        ///     true if the backup was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the state and version of the database after backup.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="backupTarget" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="backupTarget" /> is of a type which is not supported by the configured <see cref="IDbBackupCreator" />. </exception>
        /// <exception cref="InvalidOperationException"> The database is not initialized. </exception>
        /// <exception cref="NotSupportedException"> Backup is not supported by the database manager or no <see cref="IDbBackupCreator" /> is configured. </exception>
        bool Backup (object backupTarget);

        /// <summary>
        ///     Performs a restore using the configured <see cref="IDbBackupCreator" />.
        /// </summary>
        /// <param name="backupSource"> The backup creator specific object which abstracts the backup source (e.g. a stream or a path to a file). </param>
        /// <returns>
        ///     true if the restore was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the state and version of the database after restore.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="backupSource" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="backupSource" /> is of a type which is not supported by the configured <see cref="IDbBackupCreator" />. </exception>
        /// <exception cref="InvalidOperationException"> The database is not initialized. </exception>
        /// <exception cref="NotSupportedException"> Restore is not supported by the database manager or no <see cref="IDbBackupCreator" /> is configured. </exception>
        bool Restore (object backupSource);

        /// <summary>
        ///     Performs a database cleanup using the configured <see cref="IDbCleanupProcessor" />.
        /// </summary>
        /// <returns>
        ///     true if the cleanup was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the state and version of the database after cleanup.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The database is not in a ready state. </exception>
        /// <exception cref="NotSupportedException"> Cleanup is not supported by the database manager or no <see cref="IDbCleanupProcessor" /> is configured. </exception>
        bool Cleanup ();

        /// <summary>
        ///     Creates the database if it does not already exist.
        /// </summary>
        /// <returns>
        ///     true if the creation was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the state and version of the database after creation.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The database is not in the new state. </exception>
        /// <exception cref="NotSupportedException"> Creation is not supported by the database manager or no <see cref="IDbCreator" /> is configured. </exception>
        bool Create ();

        /// <summary>
        ///     Performs an upgrade to a specific database target version using the configured <see cref="IDbVersionUpgrader" />.
        /// </summary>
        /// <param name="version"> The target version to upgrade the database to. </param>
        /// <returns>
        ///     true if the upgrade was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" /> and <see cref="Version" /> are updated to reflect the state and version of the database after upgrade.
        ///     </note>
        ///     <note type="implement">
        ///         If <paramref name="version" /> is the same as <see cref="Version" />, nothing should be done.
        ///     </note>
        ///     <note type="implement">
        ///         Upgrading is to be performed incrementally, upgrading from n to n+1 until the desired target version, as specified by <paramref name="version" />, is reached.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="version" /> is less than <see cref="MinVersion" />, greater than <see cref="MaxVersion" />, or less than <see cref="Version" />. </exception>
        /// <exception cref="InvalidOperationException"> The database is not in a ready state. </exception>
        /// <exception cref="NotSupportedException"> Upgrading is not supported by the database manager or no <see cref="IDbVersionUpgrader" /> is configured. </exception>
        bool Upgrade (int version);

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
        ///     Creates a new transaction which can be used to work with the database.
        /// </summary>
        /// <param name="readOnly"> Specifies whether the underlying connection should be read-only. </param>
        /// <param name="isolationLevel"> Specifies the used isolation level for the transaction.</param>
        /// <returns>
        ///     The newly created transaction with its underlying connection already opened or null if the transaction or connection could not be created.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="InvalidOperationException"> The database is not in a ready state. </exception>
        /// <exception cref="NotSupportedException"> <paramref name="readOnly" /> is true but read-only connections are not supported or the specified <paramref name="isolationLevel"/> is not supported. </exception>
        DbTransaction CreateTransaction (bool readOnly, IsolationLevel isolationLevel);

        /// <summary>
        ///     Creates a new empty batch which can be used to perform database actions.
        /// </summary>
        /// <returns>
        ///     The newly created empty batch.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="CreateBatch" /> should be callable at any time.
        ///     </note>
        /// </remarks>
        IDbBatch CreateBatch ();

        /// <summary>
        ///     Gets the names of all available batches using the configured <see cref="IDbBatchLocator" />.
        /// </summary>
        /// <returns>
        ///     The set with the names of all available batches.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetBatchNames" /> should be callable at any time.
        ///     </note>
        /// </remarks>
        ISet<string> GetBatchNames ();

        /// <summary>
        ///     Gets a batch (for later execution) of a specified name using the configured <see cref="IDbBatchLocator" />.
        /// </summary>
        /// <param name="name"> The name of the batch. </param>
        /// <param name="commandSeparator"> The string which is used as the separator to separate commands within the batch or null if the batch locators default separators are to be used. </param>
        /// <returns>
        ///     The batch or null if the batch of the specified name could not be found.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetBatch" /> should be callable at any time.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> or <paramref name="commandSeparator" /> is an empty string. </exception>
        IDbBatch GetBatch(string name, string commandSeparator);

        /// <summary>
        ///     Executes a batch.
        /// </summary>
        /// <param name="batch"> The batch to execute. </param>
        /// <param name="readOnly"> Specifies whether the connection, used to execute the batch, should be read-only. </param>
        /// <param name="detectVersionAndStateAfterExecution"> Specifies whether the databases version and state (<see cref="Version" /> and <see cref="State" />) should be updated after execution of the batch. </param>
        /// <returns>
        ///     true if the batch was executed successfully, false otherwise.
        ///     Details about failures should be written to logs and/or into properties of the executed batch.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null </exception>
        /// <exception cref="InvalidOperationException"> The database is not in a ready state or the batch has conflicting transaction requirements or isolation levels (e.g. one command uses <see cref="DbBatchTransactionRequirement.Required" /> while another uses <see cref="DbBatchTransactionRequirement.Disallowed" />). </exception>
        /// <exception cref="NotSupportedException"> <paramref name="readOnly" /> is true but read-only connections are not supported. </exception>
        bool ExecuteBatch (IDbBatch batch, bool readOnly, bool detectVersionAndStateAfterExecution);

        /// <summary>
        ///     Initializes the database manager.
        /// </summary>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" />, <see cref="Version" />, <see cref="InitialState" />, <see cref="InitialVersion" /> are updated to reflect the current state and version of the database after initialization.
        ///     </note>
        ///     <note type="implement">
        ///         Details about failures should be written to logs.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The database manager is already initialized.</exception>
        void Initialize ();

        /// <summary>
        ///     Closes the database manager.
        /// </summary>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="State" /> and <see cref="InitialState" /> are set to <see cref="DbState.Uninitialized" />, <see cref="Version" /> and <see cref="InitialVersion" /> are set to -1.
        ///     </note>
        ///     <note type="implement">
        ///         <see cref="Close" /> should be callable multiple times and independent of the current state and version.
        ///     </note>
        ///     <note type="implement">
        ///         Details about failures should be written to logs.
        ///     </note>
        /// </remarks>
        void Close ();
    }

    /// <inheritdoc cref="IDbManager" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbManager <TConnection, TTransaction, TParameterTypes> : IDbManager
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbManager.CreateConnection" />
        new TConnection CreateConnection (bool readOnly);

        /// <inheritdoc cref="IDbManager.CreateTransaction" />
        new TTransaction CreateTransaction (bool readOnly, IsolationLevel isolationLevel);

        /// <inheritdoc cref="IDbManager.CreateBatch" />
        new IDbBatch<TConnection, TTransaction, TParameterTypes> CreateBatch();

        /// <inheritdoc cref="IDbManager.GetBatch" />
        new IDbBatch<TConnection, TTransaction, TParameterTypes> GetBatch(string name, string commandSeparator);

        /// <inheritdoc cref="IDbManager.ExecuteBatch" />
        bool ExecuteBatch(IDbBatch<TConnection, TTransaction, TParameterTypes> batch, bool readOnly, bool detectVersionAndStateAfterExecution);
    }
}

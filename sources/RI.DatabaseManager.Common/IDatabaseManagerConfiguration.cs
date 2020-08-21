using System.Data.Common;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Scripts;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Data.Database.Versioning;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Holds the database configuration used by a <see cref="IDatabaseManager" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IDatabaseManager" /> for more details about database managers.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseManagerConfiguration
    {
        /// <summary>
        ///     Gets or sets the backup creator to use.
        /// </summary>
        /// <value>
        ///     The backup creator to use or null if backup and restore are not required.
        /// </value>
        IDatabaseBackupCreator BackupCreator { get; }

        /// <summary>
        ///     Gets or sets the cleanup processor to use.
        /// </summary>
        /// <value>
        ///     The cleanup processor to use or null if cleanup is not required.
        /// </value>
        IDatabaseCleanupProcessor CleanupProcessor { get; }

        /// <summary>
        ///     Gets or sets the connection string builder which is used to build the connection string to access the database.
        /// </summary>
        /// <value>
        ///     The connection string builder which is used to build the connection string to access the database.
        /// </value>
        /// <remarks>
        ///     <note type="important">
        ///         This property is required and must be set before the database manager can be used.
        ///     </note>
        /// </remarks>
        DbConnectionStringBuilder ConnectionString { get; }

        /// <summary>
        ///     Gets or sets the script locator to use.
        /// </summary>
        /// <value>
        ///     The script locator to use or null if script retrieval is not required.
        /// </value>
        IDatabaseScriptLocator ScriptLocator { get; }

        /// <summary>
        ///     Gets or sets the version detector which is used with the database.
        /// </summary>
        /// <value>
        ///     The version detector which is used with the database.
        /// </value>
        /// <remarks>
        ///     <note type="important">
        ///         This property is required and must be set before the database manager can be used.
        ///     </note>
        /// </remarks>
        IDatabaseVersionDetector VersionDetector { get; }

        /// <summary>
        ///     Gets or sets the version upgrader to use.
        /// </summary>
        /// <value>
        ///     The version upgrader to use or null if upgrading is not required.
        /// </value>
        IDatabaseVersionUpgrader VersionUpgrader { get; }

        /// <summary>
        ///     Verifies the configuration.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <exception cref="InvalidDatabaseConfigurationException"> The database configuration is not valid. </exception>
        void VerifyConfiguration (IDatabaseManager manager);
    }

    /// <inheritdoc cref="IDatabaseManagerConfiguration" />
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseManagerConfiguration <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseManagerConfiguration
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        /// <inheritdoc cref="IDatabaseManagerConfiguration.BackupCreator" />
        new IDatabaseBackupCreator<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> BackupCreator { get; set; }

        /// <inheritdoc cref="IDatabaseManagerConfiguration.CleanupProcessor" />
        new IDatabaseCleanupProcessor<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> CleanupProcessor { get; set; }

        /// <inheritdoc cref="IDatabaseManagerConfiguration.ConnectionString" />
        new TConnectionStringBuilder ConnectionString { get; set; }

        /// <inheritdoc cref="IDatabaseManagerConfiguration.VersionDetector" />
        new IDatabaseVersionDetector<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> VersionDetector { get; set; }

        /// <inheritdoc cref="IDatabaseManagerConfiguration.VersionUpgrader" />
        new IDatabaseVersionUpgrader<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> VersionUpgrader { get; set; }

        /// <inheritdoc cref="IDatabaseManagerConfiguration.VerifyConfiguration" />
        void VerifyConfiguration (TManager manager);
    }
}

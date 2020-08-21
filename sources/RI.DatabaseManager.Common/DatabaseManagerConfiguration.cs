using System.Data.Common;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Scripts;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Data.Database.Versioning;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Implements a base class for database manager configurations.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <remarks>
    ///     <para>
    ///         It is recommended that database manager configuration implementations use this base class as it already implements most of the logic which is database-independent.
    ///     </para>
    ///     <para>
    ///         See <see cref="IDatabaseManagerConfiguration" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DatabaseManagerConfiguration <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        #region Instance Fields

        private IDatabaseBackupCreator<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> _backupCreator;

        private IDatabaseCleanupProcessor<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> _cleanupProcessor;

        private TConnectionStringBuilder _connectionString;

        private IDatabaseScriptLocator _scriptLocator;

        private IDatabaseVersionDetector<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> _versionDetector;

        private IDatabaseVersionUpgrader<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> _versionUpgrader;

        #endregion




        #region Virtuals

        /// <inheritdoc cref="IDatabaseManagerConfiguration.VerifyConfiguration" />
        protected virtual void VerifyConfiguration (TManager manager)
        {
            if (this.ConnectionString == null)
            {
                throw new InvalidDatabaseConfigurationException("No connection string configured.");
            }

            if (this.VersionDetector == null)
            {
                throw new InvalidDatabaseConfigurationException("No version detector configured.");
            }

            if (this.ScriptLocator == null)
            {
                if (this.VersionDetector?.RequiresScriptLocator ?? false)
                {
                    throw new InvalidDatabaseConfigurationException("Version detector (" + this.VersionDetector.GetType().Name + ") requires script locator but none is configured.");
                }

                if (this.VersionUpgrader?.RequiresScriptLocator ?? false)
                {
                    throw new InvalidDatabaseConfigurationException("Version upgrader (" + this.VersionUpgrader.GetType().Name + ") requires script locator but none is configured.");
                }

                if (this.BackupCreator?.RequiresScriptLocator ?? false)
                {
                    throw new InvalidDatabaseConfigurationException("Backup creator (" + this.BackupCreator.GetType().Name + ") requires script locator but none is configured.");
                }

                if (this.CleanupProcessor?.RequiresScriptLocator ?? false)
                {
                    throw new InvalidDatabaseConfigurationException("Cleanup processor (" + this.CleanupProcessor.GetType().Name + ") requires script locator but none is configured.");
                }
            }

            if (this.VersionUpgrader != null)
            {
                if (this.VersionUpgrader.GetMinVersion(manager) < 0)
                {
                    throw new InvalidDatabaseConfigurationException("Invalid version upgrader minimum version.");
                }

                if (this.VersionUpgrader.GetMaxVersion(manager) < this.VersionUpgrader.GetMinVersion(manager))
                {
                    throw new InvalidDatabaseConfigurationException("Invalid version upgrader maximum version.");
                }
            }
        }

        #endregion




        #region Interface: IDatabaseManagerConfiguration<TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration>

        /// <inheritdoc />
        public IDatabaseBackupCreator<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> BackupCreator
        {
            get
            {
                return this._backupCreator;
            }
            set
            {
                this._backupCreator = value;
            }
        }

        /// <inheritdoc />
        IDatabaseBackupCreator IDatabaseManagerConfiguration.BackupCreator => this.BackupCreator;

        /// <inheritdoc />
        public IDatabaseCleanupProcessor<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> CleanupProcessor
        {
            get
            {
                return this._cleanupProcessor;
            }
            set
            {
                this._cleanupProcessor = value;
            }
        }

        /// <inheritdoc />
        IDatabaseCleanupProcessor IDatabaseManagerConfiguration.CleanupProcessor => this.CleanupProcessor;

        /// <inheritdoc />
        public TConnectionStringBuilder ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                this._connectionString = value;
            }
        }

        /// <inheritdoc />
        DbConnectionStringBuilder IDatabaseManagerConfiguration.ConnectionString => this.ConnectionString;

        /// <inheritdoc />
        public IDatabaseScriptLocator ScriptLocator
        {
            get
            {
                return this._scriptLocator;
            }
            set
            {
                this._scriptLocator = value;
            }
        }

        /// <inheritdoc />
        public IDatabaseVersionDetector<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> VersionDetector
        {
            get
            {
                return this._versionDetector;
            }
            set
            {
                this._versionDetector = value;
            }
        }

        /// <inheritdoc />
        IDatabaseVersionDetector IDatabaseManagerConfiguration.VersionDetector => this.VersionDetector;

        /// <inheritdoc />
        public IDatabaseVersionUpgrader<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> VersionUpgrader
        {
            get
            {
                return this._versionUpgrader;
            }
            set
            {
                this._versionUpgrader = value;
            }
        }

        /// <inheritdoc />
        IDatabaseVersionUpgrader IDatabaseManagerConfiguration.VersionUpgrader => this.VersionUpgrader;

        /// <inheritdoc />
        void IDatabaseManagerConfiguration.VerifyConfiguration (IDatabaseManager manager)
        {
            this.VerifyConfiguration((TManager)manager);
        }

        /// <inheritdoc cref="IDatabaseManagerConfiguration.VerifyConfiguration" />
        void IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>.VerifyConfiguration (TManager manager)
        {
            this.VerifyConfiguration(manager);
        }

        #endregion
    }
}

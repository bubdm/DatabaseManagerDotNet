using System.Data.Common;




namespace RI.Framework.Data.Database.Backup
{
    /// <summary>
    ///     Implements a base class for database backup creators.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <remarks>
    ///     <para>
    ///         It is recommended that database backup creator implementations use this base class as it already implements most of the logic which is database-independent.
    ///     </para>
    ///     <para>
    ///         See <see cref="IDatabaseBackupCreator" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DatabaseBackupCreator <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseBackupCreator<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        #region Interface: IDatabaseBackupCreator<TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration>

        /// <inheritdoc />
        public abstract bool RequiresScriptLocator { get; }

        /// <inheritdoc />
        public abstract bool SupportsRestore { get; }

        /// <inheritdoc />
        bool IDatabaseBackupCreator.Backup (IDatabaseManager manager, object backupTarget)
        {
            return this.Backup((TManager)manager, backupTarget);
        }

        /// <inheritdoc />
        public abstract bool Backup (TManager manager, object backupTarget);

        /// <inheritdoc />
        bool IDatabaseBackupCreator.Restore (IDatabaseManager manager, object backupSource)
        {
            return this.Restore((TManager)manager, backupSource);
        }

        /// <inheritdoc />
        public abstract bool Restore (TManager manager, object backupSource);

        #endregion
    }
}

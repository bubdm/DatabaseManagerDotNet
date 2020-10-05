using System;
using System.Collections.Generic;
using System.Data.Common;

using RI.Abstractions.Composition;
using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Upgrading;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Default implementation of <see cref="IDbManagerBuilder" /> suitable for most scenarios.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbManagerBuilder : IDbManagerBuilder
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbManagerBuilder" />.
        /// </summary>
        public DbManagerBuilder ()
        {
            this.Builder = new DbManagerBuilderCore();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DbManagerBuilder" />.
        /// </summary>
        /// <param name="builder"> The database manager builder to be wrapped. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        public DbManagerBuilder (IDbManagerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            this.Builder = builder;
        }

        #endregion




        #region Instance Properties/Indexer

        private IDbManagerBuilder Builder { get; }

        #endregion




        #region Interface: IDbManagerBuilder

        /// <inheritdoc />
        public bool AlreadyBuilt => this.Builder.AlreadyBuilt;

        /// <inheritdoc />
        public List<CompositionRegistration> Registrations => this.Builder.Registrations;

        /// <inheritdoc />
        public void Build ()
        {
            this.Builder.Build();
        }

        #endregion




        #region Type: NullInstance

        /// <summary>
        ///     Null object used if no registration for <see cref="IDbBackupCreator{TConnection,TTransaction}" />, <see cref="IDbCleanupProcessor{TConnection,TTransaction}" />, <see cref="IDbVersionUpgrader{TConnection,TTransaction}" />, or <see cref="IDbBatchLocator" /> is provided.
        /// </summary>
        /// <threadsafety static="false" instance="false" />
        public sealed class NullInstance <TConnection, TTransaction> : IDbBackupCreator<TConnection, TTransaction>, IDbCleanupProcessor<TConnection, TTransaction>, IDbVersionUpgrader<TConnection, TTransaction>, IDbBatchLocator<TConnection, TTransaction>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
        {
            #region Instance Constructor/Destructor

            internal NullInstance () { }

            #endregion




            #region Interface: IDbBackupCreator<TConnection,TTransaction>

            /// <inheritdoc />
            bool IDbBackupCreator.SupportsBackup => false;

            /// <inheritdoc />
            bool IDbBackupCreator.SupportsRestore => false;

            /// <inheritdoc />
            bool IDbBackupCreator.Backup (IDbManager manager, object backupTarget) => false;

            /// <inheritdoc />
            bool IDbBackupCreator<TConnection, TTransaction>.Backup (IDbManager<TConnection, TTransaction> manager, object backupTarget) => false;

            /// <inheritdoc />
            bool IDbBackupCreator.Restore (IDbManager manager, object backupSource) => false;

            /// <inheritdoc />
            bool IDbBackupCreator<TConnection, TTransaction>.Restore (IDbManager<TConnection, TTransaction> manager, object backupTarget) => false;

            #endregion




            #region Interface: IDbCleanupProcessor<TConnection,TTransaction>

            /// <inheritdoc />
            bool IDbCleanupProcessor<TConnection, TTransaction>.Cleanup (IDbManager<TConnection, TTransaction> manager) => false;

            /// <inheritdoc />
            bool IDbCleanupProcessor.Cleanup (IDbManager manager) => false;

            #endregion




            #region Interface: IDbVersionUpgrader<TConnection,TTransaction>

            /// <inheritdoc />
            int IDbVersionUpgrader.GetMaxVersion (IDbManager manager) => -1;

            /// <inheritdoc />
            int IDbVersionUpgrader<TConnection, TTransaction>.GetMaxVersion (IDbManager<TConnection, TTransaction> manager) => -1;

            /// <inheritdoc />
            int IDbVersionUpgrader.GetMinVersion (IDbManager manager) => -1;

            /// <inheritdoc />
            int IDbVersionUpgrader<TConnection, TTransaction>.GetMinVersion (IDbManager<TConnection, TTransaction> manager) => -1;

            /// <inheritdoc />
            bool IDbVersionUpgrader.Upgrade (IDbManager manager, int sourceVersion) => false;

            /// <inheritdoc />
            bool IDbVersionUpgrader<TConnection, TTransaction>.Upgrade (IDbManager<TConnection, TTransaction> manager, int sourceVersion) => false;

            #endregion




            /// <inheritdoc />
            IDbBatch<TConnection, TTransaction> IDbBatchLocator<TConnection, TTransaction>.GetBatch (string name, string commandSeparator, Func<IDbBatch<TConnection, TTransaction>> batchCreator) => null;

            /// <inheritdoc />
            IDbBatch IDbBatchLocator.GetBatch(string name, string commandSeparator, Func<IDbBatch> batchCreator) => null;

            /// <inheritdoc />
            ISet<string> IDbBatchLocator.GetNames () => null;
        }

        #endregion
    }

    /// <summary>
    ///     Default implementation of <see cref="IDbManagerBuilder" /> and <see cref="IDbManagerBuilder{TConnection,TTransaction,TManager}" /> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbManagerBuilder <TConnection, TTransaction, TManager> : IDbManagerBuilder<TConnection, TTransaction, TManager>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbManagerBuilder" />.
        /// </summary>
        public DbManagerBuilder ()
        {
            this.Builder = new DbManagerBuilderCore();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DbManagerBuilder" />.
        /// </summary>
        /// <param name="builder"> The database manager builder to be wrapped. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        public DbManagerBuilder (IDbManagerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            this.Builder = builder;
        }

        #endregion




        #region Instance Properties/Indexer

        private IDbManagerBuilder Builder { get; }

        #endregion




        #region Interface: IDbManagerBuilder<TConnection,TTransaction,TManager>

        /// <inheritdoc />
        public bool AlreadyBuilt => this.Builder.AlreadyBuilt;

        /// <inheritdoc />
        public List<CompositionRegistration> Registrations => this.Builder.Registrations;

        /// <inheritdoc />
        public void Build ()
        {
            this.Builder.Build();
        }

        #endregion
    }
}

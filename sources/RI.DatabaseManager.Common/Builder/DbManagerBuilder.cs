using System;
using System.Collections.Generic;
using System.Data.Common;

using RI.Abstractions.Composition;
using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Creation;
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
        ///     Null object used if no registration for <see cref="IDbBackupCreator{TConnection,TTransaction,TParameterTypes}" />, <see cref="IDbCleanupProcessor{TConnection,TTransaction,TParameterTypes}" />, <see cref="IDbVersionUpgrader{TConnection,TTransaction,TParameterTypes}" />, or <see cref="IDbBatchLocator" /> is provided.
        /// </summary>
        /// <threadsafety static="false" instance="false" />
        public sealed class NullInstance <TConnection, TTransaction, TParameterTypes> : IDbBackupCreator<TConnection, TTransaction, TParameterTypes>, IDbCleanupProcessor<TConnection, TTransaction, TParameterTypes>, IDbVersionUpgrader<TConnection, TTransaction, TParameterTypes>, IDbBatchLocator<TConnection, TTransaction, TParameterTypes>, IDbCreator<TConnection, TTransaction, TParameterTypes>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
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
            bool IDbBackupCreator<TConnection, TTransaction, TParameterTypes>.Backup (IDbManager<TConnection, TTransaction, TParameterTypes> manager, object backupTarget) => false;

            /// <inheritdoc />
            bool IDbBackupCreator.Restore (IDbManager manager, object backupSource) => false;

            /// <inheritdoc />
            bool IDbBackupCreator<TConnection, TTransaction, TParameterTypes>.Restore (IDbManager<TConnection, TTransaction, TParameterTypes> manager, object backupTarget) => false;

            #endregion




            #region Interface: IDbCleanupProcessor<TConnection,TTransaction>

            /// <inheritdoc />
            bool IDbCleanupProcessor<TConnection, TTransaction, TParameterTypes>.Cleanup (IDbManager<TConnection, TTransaction, TParameterTypes> manager) => false;

            /// <inheritdoc />
            bool IDbCleanupProcessor.Cleanup (IDbManager manager) => false;

            #endregion




            #region Interface: IDbVersionUpgrader<TConnection,TTransaction>

            /// <inheritdoc />
            int IDbVersionUpgrader.GetMaxVersion (IDbManager manager) => -1;

            /// <inheritdoc />
            int IDbVersionUpgrader<TConnection, TTransaction, TParameterTypes>.GetMaxVersion (IDbManager<TConnection, TTransaction, TParameterTypes> manager) => -1;

            /// <inheritdoc />
            int IDbVersionUpgrader.GetMinVersion (IDbManager manager) => -1;

            /// <inheritdoc />
            int IDbVersionUpgrader<TConnection, TTransaction, TParameterTypes>.GetMinVersion (IDbManager<TConnection, TTransaction, TParameterTypes> manager) => -1;

            /// <inheritdoc />
            bool IDbVersionUpgrader.Upgrade (IDbManager manager, int sourceVersion) => false;

            /// <inheritdoc />
            bool IDbVersionUpgrader<TConnection, TTransaction, TParameterTypes>.Upgrade (IDbManager<TConnection, TTransaction, TParameterTypes> manager, int sourceVersion) => false;

            #endregion




            /// <inheritdoc />
            IDbBatch<TConnection, TTransaction, TParameterTypes> IDbBatchLocator<TConnection, TTransaction, TParameterTypes>.GetBatch (string name, string commandSeparator, Func<IDbBatch<TConnection, TTransaction, TParameterTypes>> batchCreator) => null;

            /// <inheritdoc />
            IDbBatch IDbBatchLocator.GetBatch(string name, string commandSeparator, Func<IDbBatch> batchCreator) => null;

            /// <inheritdoc />
            ISet<string> IDbBatchLocator.GetNames () => null;

            /// <inheritdoc />
            public bool Create (IDbManager<TConnection, TTransaction, TParameterTypes> manager) => false;

            /// <inheritdoc />
            public bool Create (IDbManager manager) => false;
        }

        #endregion
    }

    /// <summary>
    ///     Default implementation of <see cref="IDbManagerBuilder" /> and <see cref="IDbManagerBuilder{TConnection,TTransaction,TParameterTypes,TManager}" /> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbManagerBuilder <TConnection, TTransaction, TParameterTypes, TManager> : IDbManagerBuilder<TConnection, TTransaction, TParameterTypes, TManager>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
        where TManager : class, IDbManager<TConnection, TTransaction, TParameterTypes>
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

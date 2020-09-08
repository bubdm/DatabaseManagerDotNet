using System;
using System.Collections.Generic;
using System.Data.Common;

using RI.Abstractions.Composition;
using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Scripts;
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
        ///     Null object used if no registration for <see cref="IDatabaseBackupCreator{TConnection,TTransaction,TManager}" />, <see cref="IDatabaseCleanupProcessor{TConnection,TTransaction,TManager}" />, <see cref="IDatabaseVersionUpgrader{TConnection,TTransaction,TManager}" />, or <see cref="IDbScriptLocator" /> is provided.
        /// </summary>
        /// <threadsafety static="false" instance="false" />
        public sealed class NullInstance <TConnection, TTransaction, TManager> : IDatabaseBackupCreator<TConnection, TTransaction, TManager>, IDatabaseCleanupProcessor<TConnection, TTransaction, TManager>, IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>, IDbScriptLocator
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TManager : class, IDbManager<TConnection, TTransaction, TManager>
        {
            #region Instance Constructor/Destructor

            internal NullInstance () { }

            #endregion




            #region Interface: IDatabaseBackupCreator<TConnection,TTransaction,TManager>

            /// <inheritdoc />
            bool IDatabaseBackupCreator.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.SupportsRestore => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.Backup (IDbManager manager, object backupTarget)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseBackupCreator<TConnection, TTransaction, TManager>.Backup (TManager manager, object backupTarget)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseBackupCreator.Restore (IDbManager manager, object backupSource)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseBackupCreator<TConnection, TTransaction, TManager>.Restore (TManager manager, object backupTarget)
            {
                throw new NotImplementedException();
            }

            #endregion




            #region Interface: IDatabaseCleanupProcessor<TConnection,TTransaction,TManager>

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor<TConnection, TTransaction, TManager>.Cleanup (TManager manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor.Cleanup (IDbManager manager)
            {
                throw new NotImplementedException();
            }

            #endregion




            #region Interface: IDatabaseVersionUpgrader<TConnection,TTransaction,TManager>

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            int IDatabaseVersionUpgrader.GetMaxVersion (IDbManager manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            int IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>.GetMaxVersion (TManager manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            int IDatabaseVersionUpgrader.GetMinVersion (IDbManager manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            int IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>.GetMinVersion (TManager manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader.Upgrade (IDbManager manager, int sourceVersion)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>.Upgrade (TManager manager, int sourceVersion)
            {
                throw new NotImplementedException();
            }

            #endregion




            #region Interface: IDbScriptLocator

            /// <inheritdoc />
            string IDbScriptLocator.DefaultBatchSeparator
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            /// <inheritdoc />
            List<string> IDbScriptLocator.GetScriptBatches (IDbManager manager, string name, string batchSeparator, bool preprocess)
            {
                throw new NotImplementedException();
            }

            #endregion
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
        where TManager : class, IDbManager<TConnection, TTransaction, TManager>
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

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
        ///     Null object used if no registration for <see cref="IDatabaseBackupCreator{TConnection,TTransaction}" />, <see cref="IDatabaseCleanupProcessor{TConnection,TTransaction}" />, <see cref="IDatabaseVersionUpgrader{TConnection,TTransaction}" />, or <see cref="IDbScriptLocator" /> is provided.
        /// </summary>
        /// <threadsafety static="false" instance="false" />
        public sealed class NullInstance <TConnection, TTransaction> : IDatabaseBackupCreator<TConnection, TTransaction>, IDatabaseCleanupProcessor<TConnection, TTransaction>, IDatabaseVersionUpgrader<TConnection, TTransaction>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
        {
            #region Instance Constructor/Destructor

            internal NullInstance () { }

            #endregion




            #region Interface: IDatabaseBackupCreator<TConnection,TTransaction>

            /// <inheritdoc />
            bool IDatabaseBackupCreator.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.SupportsBackup => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.SupportsRestore => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.Backup (IDbManager manager, object backupTarget)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseBackupCreator<TConnection, TTransaction>.Backup (IDbManager<TConnection, TTransaction> manager, object backupTarget)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseBackupCreator.Restore (IDbManager manager, object backupSource)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseBackupCreator<TConnection, TTransaction>.Restore (IDbManager<TConnection, TTransaction> manager, object backupTarget)
            {
                throw new NotImplementedException();
            }

            #endregion




            #region Interface: IDatabaseCleanupProcessor<TConnection,TTransaction>

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor<TConnection, TTransaction>.Cleanup (IDbManager<TConnection, TTransaction> manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor.Cleanup (IDbManager manager)
            {
                throw new NotImplementedException();
            }

            #endregion




            #region Interface: IDatabaseVersionUpgrader<TConnection,TTransaction>

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            int IDatabaseVersionUpgrader.GetMaxVersion (IDbManager manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            int IDatabaseVersionUpgrader<TConnection, TTransaction>.GetMaxVersion (IDbManager<TConnection, TTransaction> manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            int IDatabaseVersionUpgrader.GetMinVersion (IDbManager manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            int IDatabaseVersionUpgrader<TConnection, TTransaction>.GetMinVersion (IDbManager<TConnection, TTransaction> manager)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader.Upgrade (IDbManager manager, int sourceVersion)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader<TConnection, TTransaction>.Upgrade (IDbManager<TConnection, TTransaction> manager, int sourceVersion)
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

using System;
using System.Data.Common;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbVersionUpgrader" /> and <see cref="IDbVersionUpgrader{TConnection,TTransaction}" />.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database version upgrader implementations use this base class as it already implements most of the database-independent logic defined by <see cref="IDbVersionUpgrader" /> and <see cref="IDbVersionUpgrader{TConnection,TTransaction}" />.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DbVersionUpgraderBase <TConnection, TTransaction> : IDbVersionUpgrader<TConnection, TTransaction>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbVersionUpgraderBase{TConnection,TTransaction}" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        protected DbVersionUpgraderBase (ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Logger = logger;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used logger.
        /// </summary>
        /// <value>
        ///     The used logger.
        /// </value>
        protected ILogger Logger { get; }

        #endregion




        #region Interface: IDbVersionUpgrader<TConnection,TTransaction>

        /// <inheritdoc />
        int IDbVersionUpgrader.GetMaxVersion (IDbManager manager)
        {
            return this.GetMaxVersion((IDbManager<TConnection, TTransaction>)manager);
        }

        /// <inheritdoc />
        public abstract int GetMaxVersion (IDbManager<TConnection, TTransaction> manager);

        /// <inheritdoc />
        int IDbVersionUpgrader.GetMinVersion (IDbManager manager)
        {
            return this.GetMinVersion((IDbManager<TConnection, TTransaction>)manager);
        }

        /// <inheritdoc />
        public abstract int GetMinVersion (IDbManager<TConnection, TTransaction> manager);

        /// <inheritdoc />
        public abstract bool Upgrade (IDbManager<TConnection, TTransaction> manager, int sourceVersion);

        /// <inheritdoc />
        bool IDbVersionUpgrader.Upgrade (IDbManager manager, int sourceVersion)
        {
            return this.Upgrade((IDbManager<TConnection, TTransaction>)manager, sourceVersion);
        }

        #endregion
    }
}

using System;
using System.Data.Common;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDatabaseVersionUpgrader"/> and <see cref="IDatabaseVersionUpgrader{TConnection,TTransaction,TManager}"/>.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that version upgrader implementations use this base class as it already implements most of the database-independent logic defined by <see cref="IDatabaseVersionUpgrader"/>.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DatabaseVersionUpgrader <TConnection, TTransaction, TManager> : IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction, TManager>
    {
        private ILogger Logger { get; }




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseVersionUpgrader{TConnection,TTransaction,TManager}" />.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        protected DatabaseVersionUpgrader(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Logger = logger;
        }

        #endregion

        #region Interface: IDatabaseVersionUpgrader<TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration>

        /// <inheritdoc />
        public abstract bool RequiresScriptLocator { get; }

        /// <inheritdoc />
        int IDatabaseVersionUpgrader.GetMaxVersion (IDbManager manager) => this.GetMaxVersion((TManager)manager);

        /// <inheritdoc />
        public abstract int GetMaxVersion (TManager manager);

        /// <inheritdoc />
        int IDatabaseVersionUpgrader.GetMinVersion (IDbManager manager) => this.GetMinVersion((TManager)manager);

        /// <inheritdoc />
        public abstract int GetMinVersion (TManager manager);

        /// <inheritdoc />
        public abstract bool Upgrade (TManager manager, int sourceVersion);

        /// <inheritdoc />
        bool IDatabaseVersionUpgrader.Upgrade (IDbManager manager, int sourceVersion) => this.Upgrade((TManager)manager, sourceVersion);

        #endregion
    }
}

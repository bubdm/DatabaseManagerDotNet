using System;
using System.Data.Common;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Cleanup
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbCleanupProcessor" /> and <see cref="IDbCleanupProcessor{TConnection,TTransaction}" />.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database cleanup processor implementations use this base class as it already implements most of the database-independent logic defined by <see cref="IDbCleanupProcessor" /> and <see cref="IDbCleanupProcessor{TConnection,TTransaction}" />.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DbCleanupProcessorBase <TConnection, TTransaction> : IDbCleanupProcessor<TConnection, TTransaction>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbCleanupProcessorBase{TConnection,TTransaction}" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        protected DbCleanupProcessorBase (ILogger logger)
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




        #region Interface: IDbCleanupProcessor<TConnection,TTransaction>

        /// <inheritdoc />
        public abstract bool Cleanup (IDbManager<TConnection, TTransaction> manager);

        /// <inheritdoc />
        bool IDbCleanupProcessor.Cleanup (IDbManager manager)
        {
            return this.Cleanup((IDbManager<TConnection, TTransaction>)manager);
        }

        #endregion
    }
}

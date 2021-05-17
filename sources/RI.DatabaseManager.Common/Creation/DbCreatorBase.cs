using System;
using System.Data.Common;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder.Options;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Creation
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbCreator" /> and <see cref="IDbCreator{TConnection,TTransaction,TParameterTypes}" />.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database creator implementations use this base class as it already implements most of the database-independent logic defined by <see cref="IDbCreator" /> and <see cref="IDbCreator{TConnection,TTransaction,TParameterTypes}" />.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DbCreatorBase <TConnection, TTransaction, TParameterTypes> : IDbCreator<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbCreatorBase{TConnection,TTransaction,TParameterTypes}" />.
        /// </summary>
        /// <param name="options"> The used database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        protected DbCreatorBase(IDbManagerOptions options, ILogger logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Options = options;
            this.Logger = logger;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used database manager options.
        /// </summary>
        /// <value>
        ///     The used database manager options.
        /// </value>
        protected IDbManagerOptions Options { get; }

        /// <summary>
        ///     Gets the used logger.
        /// </summary>
        /// <value>
        ///     The used logger.
        /// </value>
        protected ILogger Logger { get; }

        /// <summary>
        ///     Writes a log message.
        /// </summary>
        /// <param name="level"> The log level of the log message. </param>
        /// <param name="format"> Log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc. which are expanded by <paramref name="args" />). </param>
        /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
        protected void Log(LogLevel level, string format, params object[] args)
        {
            this.Logger.Log(level, this.GetType()
                                       .Name, null, format, args);
        }

        /// <summary>
        ///     Writes a log message.
        /// </summary>
        /// <param name="level"> The log level of the log message. </param>
        /// <param name="exception"> Exception associated with the log message. </param>
        /// <param name="format"> Optional log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc. which are expanded by <paramref name="args" />). </param>
        /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
        protected void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            this.Logger.Log(level, this.GetType()
                                       .Name, exception, format, args);
        }

        #endregion




        #region Interface: IDbCleanupProcessor<TConnection,TTransaction>

        /// <inheritdoc />
        /// TODO: Make base implementation
        public abstract bool Create (IDbManager<TConnection, TTransaction, TParameterTypes> manager);

        /// <inheritdoc />
        bool IDbCreator.Create (IDbManager manager) => this.Create((IDbManager<TConnection, TTransaction, TParameterTypes>)manager);

        #endregion
    }
}

using System;
using System.Data.Common;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Backup
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbBackupCreator" /> and <see cref="IDbBackupCreator{TConnection,TTransaction}" />.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database backup creators implementations use this base class as it already implements most of the database-independent logic defined by <see cref="IDbBackupCreator" /> and <see cref="IDbBackupCreator{TConnection,TTransaction}" />.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DbBackupCreatorBase <TConnection, TTransaction> : IDbBackupCreator<TConnection, TTransaction>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbBackupCreatorBase{TConnection,TTransaction}" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        protected DbBackupCreatorBase (ILogger logger)
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




        #region Interface: IDbBackupCreator<TConnection,TTransaction>

        /// <inheritdoc />
        public abstract bool SupportsBackup { get; }

        /// <inheritdoc />
        public abstract bool SupportsRestore { get; }

        /// <inheritdoc />
        bool IDbBackupCreator.Backup (IDbManager manager, object backupTarget)
        {
            return this.Backup((IDbManager<TConnection, TTransaction>)manager, backupTarget);
        }

        /// <inheritdoc />
        public abstract bool Backup (IDbManager<TConnection, TTransaction> manager, object backupTarget);

        /// <inheritdoc />
        bool IDbBackupCreator.Restore (IDbManager manager, object backupSource)
        {
            return this.Restore((IDbManager<TConnection, TTransaction>)manager, backupSource);
        }

        /// <inheritdoc />
        public abstract bool Restore (IDbManager<TConnection, TTransaction> manager, object backupSource);

        #endregion
    }
}

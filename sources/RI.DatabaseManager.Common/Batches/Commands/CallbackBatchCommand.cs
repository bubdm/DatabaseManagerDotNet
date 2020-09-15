using System;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Commands
{
    /// <summary>
    ///     A batch command which provides a delegate to be called when executed.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class CallbackBatchCommand : IDbBatchCommand
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CallbackBatchCommand{TConnection,TTransaction}" />.
        /// </summary>
        /// <param name="callback"> The delegate to the callback which is called when the command is executed. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        public CallbackBatchCommand (Func<DbConnection, DbTransaction, object> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.Code = callback;
            this.TransactionRequirement = transactionRequirement;
        }

        #endregion




        #region Interface: IDbBatchCommand

        /// <inheritdoc />
        public Func<DbConnection, DbTransaction, object> Code { get; }

        /// <inheritdoc />
        public object Result { get; set; }

        /// <inheritdoc />
        string IDbBatchCommand.Script => null;

        /// <inheritdoc />
        public DbBatchTransactionRequirement TransactionRequirement { get; }

        /// <inheritdoc />
        public bool WasExecuted { get; set; }

        #endregion
    }

    /// <summary>
    ///     A batch command which provides a delegate to be called when executed.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class CallbackBatchCommand <TConnection, TTransaction> : IDbBatchCommand
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CallbackBatchCommand{TConnection,TTransaction}" />.
        /// </summary>
        /// <param name="callback"> The delegate to the callback which is called when the command is executed. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        public CallbackBatchCommand (Func<TConnection, TTransaction, object> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.Callback = callback;
            this.TransactionRequirement = transactionRequirement;

            this.Code = (c, t) => this.Callback((TConnection)c, (TTransaction)t);
        }

        #endregion




        #region Instance Properties/Indexer

        private Func<TConnection, TTransaction, object> Callback { get; }

        #endregion




        #region Interface: IDbBatchCommand

        /// <inheritdoc />
        public Func<DbConnection, DbTransaction, object> Code { get; }

        /// <inheritdoc />
        public object Result { get; set; }

        /// <inheritdoc />
        string IDbBatchCommand.Script => null;

        /// <inheritdoc />
        public DbBatchTransactionRequirement TransactionRequirement { get; }

        /// <inheritdoc />
        public bool WasExecuted { get; set; }

        #endregion
    }
}

using System;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Commands
{
    /// <summary>
    ///     A batch command which provides a delegate to be called when executed.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class CallbackBatchCommand <TConnection, TTransaction, TParameterTypes> : IDbBatchCommand<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CallbackBatchCommand{TConnection,TTransaction,TParameterTypes}" />.
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

            this.Code = callback;
            this.TransactionRequirement = transactionRequirement;
        }

        #endregion




        #region Interface: IDbBatchCommand

        /// <inheritdoc />
        Func<DbConnection, DbTransaction, object> IDbBatchCommand.Code => (c, t) => this.Code((TConnection)c, (TTransaction)t);

        /// <inheritdoc />
        public Func<TConnection, TTransaction, object> Code { get; }

        /// <inheritdoc />
        public object Result { get; set; }

        /// <inheritdoc />
        string IDbBatchCommand.Script => null;

        /// <inheritdoc />
        public DbBatchTransactionRequirement TransactionRequirement { get; }

        /// <inheritdoc />
        public bool WasExecuted { get; set; }

        #endregion




        /// <inheritdoc />
        object ICloneable.Clone () => this.Clone();

        /// <inheritdoc cref="ICloneable.Clone"/>
        public CallbackBatchCommand<TConnection, TTransaction, TParameterTypes> Clone ()
        {
            CallbackBatchCommand<TConnection, TTransaction, TParameterTypes> clone = new CallbackBatchCommand<TConnection, TTransaction, TParameterTypes>(this.Code, this.TransactionRequirement);
            clone.Result = this.Result;
            clone.WasExecuted = this.WasExecuted;
            return clone;
        }
    }
}

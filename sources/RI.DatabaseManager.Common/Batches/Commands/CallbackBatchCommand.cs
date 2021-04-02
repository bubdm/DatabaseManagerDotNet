using System;
using System.Data;
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
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        public CallbackBatchCommand (CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.Code = callback;
            this.TransactionRequirement = transactionRequirement;
            this.IsolationLevel = isolationLevel;
        }

        #endregion




        #region Interface: IDbBatchCommand

        /// <inheritdoc />
        CallbackBatchCommandDelegate IDbBatchCommand.Code => this.CodeExecution;

        private object CodeExecution (DbConnection connection, DbTransaction transaction, IDbBatchCommandParameterCollection parameters, out string error, out Exception exception) => this.Code((TConnection)connection, (TTransaction)transaction, (IDbBatchCommandParameterCollection<TParameterTypes>)parameters, out error, out exception);

        /// <inheritdoc />
        public IDbBatchCommandParameterCollection<TParameterTypes> Parameters { get; } = new DbBatchCommandParameterCollection<TParameterTypes>();

        /// <inheritdoc />
        public CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> Code { get; }

        /// <inheritdoc />
        public object Result { get; set; }

        /// <inheritdoc />
        public Exception Exception { get; set; }

        /// <inheritdoc />
        public string Error { get; set; }

        /// <inheritdoc />
        string IDbBatchCommand.Script => null;

        /// <inheritdoc />
        public DbBatchTransactionRequirement TransactionRequirement { get; }

        /// <inheritdoc />
        public IsolationLevel? IsolationLevel { get; }

        /// <inheritdoc />
        public bool WasExecuted { get; set; }

        /// <inheritdoc />
        IDbBatchCommandParameterCollection IDbBatchCommand.Parameters => this.Parameters;

        #endregion




        /// <inheritdoc />
        object ICloneable.Clone () => this.Clone();

        /// <inheritdoc cref="ICloneable.Clone"/>
        public CallbackBatchCommand<TConnection, TTransaction, TParameterTypes> Clone ()
        {
            CallbackBatchCommand<TConnection, TTransaction, TParameterTypes> clone = new CallbackBatchCommand<TConnection, TTransaction, TParameterTypes>(this.Code, this.TransactionRequirement, this.IsolationLevel);
            clone.Result = this.Result;
            clone.Exception = this.Exception;
            clone.Error = this.Error;
            clone.WasExecuted = this.WasExecuted;

            foreach (IDbBatchCommandParameter<TParameterTypes> parameter in this.Parameters)
            {
                clone.Parameters.Add((IDbBatchCommandParameter<TParameterTypes>)parameter.Clone());
            }

            return clone;
        }
    }
}

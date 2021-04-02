using System;
using System.Data;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Commands
{
    /// <summary>
    ///     A batch command which provides a script to be executed.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> : IDbBatchCommand<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CallbackBatchCommand{TConnection,TTransaction,TParameterTypes}" />.
        /// </summary>
        /// <param name="script"> The database script. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        public ScriptBatchCommand (string script, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
        {
            this.Script = string.IsNullOrWhiteSpace(script) ? null : script;
            this.TransactionRequirement = transactionRequirement;
            this.IsolationLevel = isolationLevel;
        }

        #endregion




        #region Interface: IDbBatchCommand

        /// <inheritdoc />
        CallbackBatchCommandDelegate IDbBatchCommand.Code => null;

        /// <inheritdoc />
        public IDbBatchCommandParameterCollection<TParameterTypes> Parameters { get; } = new DbBatchCommandParameterCollection<TParameterTypes>();

        /// <inheritdoc />
        CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> IDbBatchCommand<TConnection, TTransaction, TParameterTypes>.Code => null;

        /// <inheritdoc />
        public object Result { get; set; }

        /// <inheritdoc />
        public Exception Exception { get; set; }

        /// <inheritdoc />
        public string Error { get; set; }

        /// <inheritdoc />
        public string Script { get; }

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
        object ICloneable.Clone() => this.Clone();

        /// <inheritdoc cref="ICloneable.Clone"/>
        public ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> Clone()
        {
            ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> clone = new ScriptBatchCommand<TConnection, TTransaction, TParameterTypes>(this.Script, this.TransactionRequirement, this.IsolationLevel);
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

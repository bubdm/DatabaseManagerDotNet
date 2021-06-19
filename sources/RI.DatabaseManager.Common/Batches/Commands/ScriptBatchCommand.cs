using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Commands
{
    /// <summary>
    ///     A batch command which provides a script to be executed.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class
        ScriptBatchCommand <TConnection, TTransaction, TParameterTypes> : IDbBatchCommand<TConnection, TTransaction,
            TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CallbackBatchCommand{TConnection,TTransaction,TParameterTypes}" />.
        /// </summary>
        /// <param name="script"> The database script. </param>
        /// <param name="transactionRequirement">
        ///     The optional transaction requirement specification. Default values is
        ///     <see cref="DbBatchTransactionRequirement.DontCare" />.
        /// </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null. </param>
        /// <param name="executionType">
        ///     The optional execution type specification. Default value is
        ///     <see cref="DbBatchExecutionType.Reader" />.
        /// </param>
        public ScriptBatchCommand (string script,
                                   DbBatchTransactionRequirement transactionRequirement =
                                       DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null,
                                   DbBatchExecutionType executionType = DbBatchExecutionType.Reader)
        {
            this.Results = new List<object>();
            this.Script = string.IsNullOrWhiteSpace(script) ? null : script;
            this.TransactionRequirement = transactionRequirement;
            this.IsolationLevel = isolationLevel;
            this.ExecutionType = executionType;
        }

        #endregion




        #region Instance Methods

        /// <inheritdoc cref="ICloneable.Clone" />
        public ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> Clone ()
        {
            ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> clone =
                new ScriptBatchCommand<TConnection, TTransaction, TParameterTypes>(this.Script,
                    this.TransactionRequirement, this.IsolationLevel, this.ExecutionType);

            clone.Results.AddRange(this.Results);
            clone.Exception = this.Exception;
            clone.Error = this.Error;
            clone.WasExecuted = this.WasExecuted;

            foreach (IDbBatchCommandParameter<TParameterTypes> parameter in this.Parameters)
            {
                clone.Parameters.Add((IDbBatchCommandParameter<TParameterTypes>)parameter.Clone());
            }

            return clone;
        }

        #endregion




        #region Interface: ICloneable

        /// <inheritdoc />
        object ICloneable.Clone () => this.Clone();

        #endregion




        #region Interface: IDbBatchCommand

        /// <inheritdoc />
        CallbackBatchCommandDelegate IDbBatchCommand.Code => null;

        /// <inheritdoc />
        public string Error { get; set; }

        /// <inheritdoc />
        public Exception Exception { get; set; }

        /// <inheritdoc />
        public DbBatchExecutionType ExecutionType { get; }

        /// <inheritdoc />
        public IsolationLevel? IsolationLevel { get; }

        /// <inheritdoc />
        IDbBatchCommandParameterCollection IDbBatchCommand.Parameters => this.Parameters;

        /// <inheritdoc />
        public List<object> Results { get; }

        /// <inheritdoc />
        public string Script { get; }

        /// <inheritdoc />
        public DbBatchTransactionRequirement TransactionRequirement { get; }

        /// <inheritdoc />
        public bool WasExecuted { get; set; }

        #endregion




        #region Interface: IDbBatchCommand<TConnection,TTransaction,TParameterTypes>

        /// <inheritdoc />
        CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes>
            IDbBatchCommand<TConnection, TTransaction, TParameterTypes>.Code =>
            null;

        /// <inheritdoc />
        public IDbBatchCommandParameterCollection<TParameterTypes> Parameters { get; } =
            new DbBatchCommandParameterCollection<TParameterTypes>();

        #endregion
    }
}

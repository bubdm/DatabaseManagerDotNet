using System;
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
        public ScriptBatchCommand (string script, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            this.Script = string.IsNullOrWhiteSpace(script) ? null : script;
            this.TransactionRequirement = transactionRequirement;
        }

        #endregion




        #region Interface: IDbBatchCommand

        /// <inheritdoc />
        Func<DbConnection, DbTransaction, object> IDbBatchCommand.Code => null;

        /// <inheritdoc />
        Func<TConnection, TTransaction, object> IDbBatchCommand<TConnection, TTransaction, TParameterTypes>.Code => null;

        /// <inheritdoc />
        public object Result { get; set; }

        /// <inheritdoc />
        public string Script { get; }

        /// <inheritdoc />
        public DbBatchTransactionRequirement TransactionRequirement { get; }

        /// <inheritdoc />
        public bool WasExecuted { get; set; }

        #endregion




        /// <inheritdoc />
        object ICloneable.Clone() => this.Clone();

        /// <inheritdoc cref="ICloneable.Clone"/>
        public ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> Clone()
        {
            ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> clone = new ScriptBatchCommand<TConnection, TTransaction, TParameterTypes>(this.Script, this.TransactionRequirement);
            clone.Result = this.Result;
            clone.WasExecuted = this.WasExecuted;
            return clone;
        }
    }
}

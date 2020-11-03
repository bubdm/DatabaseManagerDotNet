using System;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Commands
{
    /// <summary>
    ///     A batch command which provides a script to be executed.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class ScriptBatchCommand<TConnection, TTransaction> : IDbBatchCommand<TConnection, TTransaction>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CallbackBatchCommand{TConnection,TTransaction}" />.
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
        Func<TConnection, TTransaction, object> IDbBatchCommand<TConnection, TTransaction>.Code => null;

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
        public ScriptBatchCommand<TConnection, TTransaction> Clone()
        {
            ScriptBatchCommand<TConnection, TTransaction> clone = new ScriptBatchCommand<TConnection, TTransaction>(this.Script, this.TransactionRequirement);
            clone.Result = this.Result;
            clone.WasExecuted = this.WasExecuted;
            return clone;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using RI.Abstractions.Logging;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbProcessingStep"/> and <see cref="IDbProcessingStep{TConnection,TTransaction,TManager}"/>.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database processing step implementations use this base class as it already implements most of the database-independent logic defined by <see cref="IDbProcessingStep"/> and <see cref="IDbProcessingStep{TConnection,TTransaction,TManager}"/>.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DbProcessingStep <TConnection, TTransaction, TManager> : IDbProcessingStep<TConnection, TTransaction, TManager>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction, TManager>
    {
        #region Instance Constructor/Destructor

        private ILogger Logger { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="DbProcessingStep{TConnection,TTransaction,TManager}" />
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        protected DbProcessingStep (ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Logger = logger;
        }

        #endregion




        #region Instance Properties/Indexer

        private List<Tuple<SubStepType, DbProcessingStepTransactionRequirement, object>> SubSteps { get; } = new List<Tuple<SubStepType, DbProcessingStepTransactionRequirement, object>>();

        #endregion




        #region Abstracts

        /// <summary>
        ///     Implements the database-specific execution of batches.
        /// </summary>
        /// <param name="batches"> The batches to execute. </param>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="connection"> The used database connection. </param>
        /// <param name="transaction"> The used database transaction. Can be null if no transaction is used. </param>
        protected abstract void ExecuteBatchesImpl (List<string> batches, TManager manager, TConnection connection, TTransaction transaction);

        #endregion




        #region Virtuals

        /// <summary>
        ///     Implements the database-specific execution of callbacks.
        /// </summary>
        /// <param name="callback"> The callback to execute. </param>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="connection"> The used database connection. </param>
        /// <param name="transaction"> The used database transaction. Can be null if no transaction is used. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The default implementation simply invokes <paramref name="callback" />.
        ///     </note>
        /// </remarks>
        protected virtual void ExecuteCallbackImpl (DbProcessingStepDelegate<TConnection, TTransaction, TManager> callback, TManager manager, TConnection connection, TTransaction transaction)
        {
            callback(this, manager, connection, transaction);
        }

        /// <summary>
        ///     Implements the database-specific execution of callbacks.
        /// </summary>
        /// <param name="callback"> The callback to execute. </param>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="connection"> The used database connection. </param>
        /// <param name="transaction"> The used database transaction. Can be null if no transaction is used. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The default implementation simply invokes <paramref name="callback" />.
        ///     </note>
        /// </remarks>
        protected virtual void ExecuteCallbackImpl (DbProcessingStepDelegate callback, TManager manager, TConnection connection, TTransaction transaction)
        {
            callback(this, manager, connection, transaction);
        }

        #endregion




        #region Interface: IDatabaseProcessingStep<TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration>

        /// <inheritdoc />
        public bool RequiresScriptLocator => this.SubSteps.Count == 0 ? false : this.SubSteps.Any(x => x.Item1 == SubStepType.Script);

        /// <inheritdoc />
        public bool RequiresTransaction => this.SubSteps.Count == 0 ? false : this.SubSteps.Any(x => x.Item2 == DbProcessingStepTransactionRequirement.Required);

        /// <inheritdoc />
        public void AddBatch (string batch) => this.AddBatch(batch, DbProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddBatch (string batch, DbProcessingStepTransactionRequirement transactionRequirement)
        {
            if (string.IsNullOrEmpty(batch))
            {
                return;
            }

            this.SubSteps.Add(new Tuple<SubStepType, DbProcessingStepTransactionRequirement, object>(SubStepType.Batch, transactionRequirement, batch));
        }

        /// <inheritdoc />
        public void AddBatches (IEnumerable<string> batches) => this.AddBatches(batches, DbProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddBatches (IEnumerable<string> batches, DbProcessingStepTransactionRequirement transactionRequirement)
        {
            if (batches == null)
            {
                return;
            }

            List<string> batchList = new List<string>(batches);
            batchList.RemoveAll(string.IsNullOrEmpty);

            if (batchList.Count == 0)
            {
                return;
            }

            this.SubSteps.Add(new Tuple<SubStepType, DbProcessingStepTransactionRequirement, object>(SubStepType.Batches, transactionRequirement, batches));
        }

        /// <inheritdoc />
        public void AddCode (DbProcessingStepDelegate<TConnection, TTransaction, TManager> callback) => this.AddCode(callback, DbProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddCode (DbProcessingStepDelegate<TConnection, TTransaction, TManager> callback, DbProcessingStepTransactionRequirement transactionRequirement)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.SubSteps.Add(new Tuple<SubStepType, DbProcessingStepTransactionRequirement, object>(SubStepType.CodeTyped, transactionRequirement, callback));
        }

        /// <inheritdoc />
        public void AddCode (DbProcessingStepDelegate callback) => this.AddCode(callback, DbProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddCode (DbProcessingStepDelegate callback, DbProcessingStepTransactionRequirement transactionRequirement)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.SubSteps.Add(new Tuple<SubStepType, DbProcessingStepTransactionRequirement, object>(SubStepType.CodeGeneric, transactionRequirement, callback));
        }

        /// <inheritdoc />
        public void AddScript (string scriptName) => this.AddScript(scriptName, DbProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddScript (string scriptName, DbProcessingStepTransactionRequirement transactionRequirement)
        {
            if (scriptName == null)
            {
                throw new ArgumentNullException(nameof(scriptName));
            }

            if (string.IsNullOrWhiteSpace(scriptName))
            {
                throw new ArgumentException("The script name is empty.", nameof(scriptName));
            }

            this.SubSteps.Add(new Tuple<SubStepType, DbProcessingStepTransactionRequirement, object>(SubStepType.Script, transactionRequirement, scriptName));
        }

        /// <inheritdoc />
        void IDbProcessingStep.Execute (IDbManager manager, DbConnection connection, DbTransaction transaction) => this.Execute((TManager)manager, (TConnection)connection, (TTransaction)transaction);

        /// <inheritdoc />
        public void Execute (TManager manager, TConnection connection, TTransaction transaction)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (this.SubSteps.Count == 0)
            {
                return;
            }

            if (this.SubSteps.Any(x => x.Item2 == DbProcessingStepTransactionRequirement.Required) && this.SubSteps.Any(x => x.Item2 == DbProcessingStepTransactionRequirement.Disallowed))
            {
                throw new InvalidOperationException("Conflicting transaction requirements.");
            }

            if (this.SubSteps.Any(x => x.Item2 == DbProcessingStepTransactionRequirement.Required) && (transaction == null))
            {
                throw new InvalidOperationException("Transaction required but none provided.");
            }

            if (this.SubSteps.Any(x => x.Item2 == DbProcessingStepTransactionRequirement.Disallowed) && (transaction != null))
            {
                throw new InvalidOperationException("Transaction disallowed but provided.");
            }

            List<object> subSteps = new List<object>();
            foreach (Tuple<SubStepType, DbProcessingStepTransactionRequirement, object> subStep in this.SubSteps)
            {
                List<string> batches = null;
                DbProcessingStepDelegate<TConnection, TTransaction, TManager> callbackTyped = null;
                DbProcessingStepDelegate callbackGeneric = null;

                switch (subStep.Item1)
                {
                    case SubStepType.Script:
                        string scriptName = (string)subStep.Item3;
                        batches = manager.GetScriptBatch(scriptName, true);
                        if (batches == null)
                        {
                            throw new Exception("Batch retrieval failed for script: " + scriptName);
                        }
                        break;

                    case SubStepType.Batch:
                        batches = new List<string>();
                        batches.Add((string)subStep.Item3);
                        break;

                    case SubStepType.Batches:
                        batches = (List<string>)subStep.Item3;
                        break;

                    case SubStepType.CodeTyped:
                        callbackTyped = (DbProcessingStepDelegate<TConnection, TTransaction, TManager>)subStep.Item3;
                        break;

                    case SubStepType.CodeGeneric:
                        callbackGeneric = (DbProcessingStepDelegate)subStep.Item3;
                        break;
                }

                if (batches != null)
                {
                    subSteps.Add(batches);
                }

                if (callbackTyped != null)
                {
                    subSteps.Add(callbackTyped);
                }

                if (callbackGeneric != null)
                {
                    subSteps.Add(callbackGeneric);
                }
            }

            foreach (object subStep in subSteps)
            {
                List<string> batches = subStep as List<string>;
                DbProcessingStepDelegate<TConnection, TTransaction, TManager> callbackTyped = subStep as DbProcessingStepDelegate<TConnection, TTransaction, TManager>;
                DbProcessingStepDelegate callbackGeneric = subStep as DbProcessingStepDelegate;

                if (batches != null)
                {
                    this.ExecuteBatchesImpl(batches, manager, connection, transaction);
                }

                if (callbackTyped != null)
                {
                    this.ExecuteCallbackImpl(callbackTyped, manager, connection, transaction);
                }

                if (callbackGeneric != null)
                {
                    this.ExecuteCallbackImpl(callbackGeneric, manager, connection, transaction);
                }
            }
        }

        #endregion




        #region Type: SubStepType

        private enum SubStepType
        {
            Script,

            Batch,

            Batches,

            CodeTyped,

            CodeGeneric,
        }

        #endregion
    }
}

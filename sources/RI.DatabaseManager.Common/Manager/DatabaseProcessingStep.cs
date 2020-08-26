using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Implements a single database processing step.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IDatabaseProcessingStep{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DatabaseProcessingStep <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseProcessingStep{TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration}" />
        /// </summary>
        protected DatabaseProcessingStep ()
        {
            this.SubSteps = new List<Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>>();
        }

        #endregion




        #region Instance Properties/Indexer

        private List<Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>> SubSteps { get; }

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
        ///     <para>
        ///         The default implementation invokes <paramref name="callback" />.
        ///     </para>
        /// </remarks>
        protected virtual void ExecuteCallbackImpl (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callback, TManager manager, TConnection connection, TTransaction transaction)
        {
            //TODO: Log: this.Log(LogLevel.Debug, "Executing database processing callback: {0}.{1}", callback.Method?.DeclaringType?.FullName ?? "[null]", callback.Method?.Name ?? "[null]");
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
        ///     <para>
        ///         The default implementation invokes <paramref name="callback" />.
        ///     </para>
        /// </remarks>
        protected virtual void ExecuteCallbackImpl (DatabaseProcessingStepDelegate callback, TManager manager, TConnection connection, TTransaction transaction)
        {
            //TODO: Log: this.Log(LogLevel.Debug, "Executing database processing callback: {0}.{1}", callback.Method?.DeclaringType?.FullName ?? "[null]", callback.Method?.Name ?? "[null]");
            callback(this, manager, connection, transaction);
        }

        #endregion




        #region Interface: IDatabaseProcessingStep<TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration>

        /// <inheritdoc />
        public bool RequiresScriptLocator => this.SubSteps.Count == 0 ? false : this.SubSteps.Any(x => x.Item1 == SubStepType.Script);

        /// <inheritdoc />
        public bool RequiresTransaction => this.SubSteps.Count == 0 ? false : this.SubSteps.Any(x => x.Item2 == DatabaseProcessingStepTransactionRequirement.Required);

        /// <inheritdoc />
        public void AddBatch (string batch) => this.AddBatch(batch, DatabaseProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddBatch (string batch, DatabaseProcessingStepTransactionRequirement transactionRequirement)
        {
            if (batch == null)
            {
                return;
            }

            if (batch.IsNullOrEmptyOrWhitespace())
            {
                return;
            }

            this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.Batch, transactionRequirement, batch));
        }

        /// <inheritdoc />
        public void AddBatches (IEnumerable<string> batches) => this.AddBatches(batches, DatabaseProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddBatches (IEnumerable<string> batches, DatabaseProcessingStepTransactionRequirement transactionRequirement)
        {
            if (batches == null)
            {
                return;
            }

            List<string> batchList = new List<string>(batches);
            batchList.RemoveAll(x => x.IsNullOrEmptyOrWhitespace());

            if (batchList.Count == 0)
            {
                return;
            }

            this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.Batches, transactionRequirement, batches));
        }

        /// <inheritdoc />
        public void AddCode (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callback) => this.AddCode(callback, DatabaseProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddCode (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callback, DatabaseProcessingStepTransactionRequirement transactionRequirement)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.CodeTyped, transactionRequirement, callback));
        }

        /// <inheritdoc />
        public void AddCode (DatabaseProcessingStepDelegate callback) => this.AddCode(callback, DatabaseProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddCode (DatabaseProcessingStepDelegate callback, DatabaseProcessingStepTransactionRequirement transactionRequirement)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.CodeGeneric, transactionRequirement, callback));
        }

        /// <inheritdoc />
        public void AddScript (string scriptName) => this.AddScript(scriptName, DatabaseProcessingStepTransactionRequirement.DontCare);

        /// <inheritdoc />
        public void AddScript (string scriptName, DatabaseProcessingStepTransactionRequirement transactionRequirement)
        {
            if (scriptName == null)
            {
                throw new ArgumentNullException(nameof(scriptName));
            }

            if (scriptName.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(scriptName));
            }

            this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.Script, transactionRequirement, scriptName));
        }

        /// <inheritdoc />
        void IDatabaseProcessingStep.Execute (IDatabaseManager manager, DbConnection connection, DbTransaction transaction) => this.Execute((TManager)manager, (TConnection)connection, (TTransaction)transaction);

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

            if (this.SubSteps.Any(x => x.Item2 == DatabaseProcessingStepTransactionRequirement.Required) && this.SubSteps.Any(x => x.Item2 == DatabaseProcessingStepTransactionRequirement.Disallowed))
            {
                throw new Exception("Conflicting transaction requirements.");
            }

            if (this.SubSteps.Any(x => x.Item2 == DatabaseProcessingStepTransactionRequirement.Required) && (transaction == null))
            {
                throw new Exception("Transaction required but none provided.");
            }

            if (this.SubSteps.Any(x => x.Item2 == DatabaseProcessingStepTransactionRequirement.Disallowed) && (transaction != null))
            {
                throw new Exception("Transaction disallowed but provided.");
            }

            List<object> subSteps = new List<object>();
            foreach (Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object> subStep in this.SubSteps)
            {
                List<string> batches = null;
                DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callbackTyped = null;
                DatabaseProcessingStepDelegate callbackGeneric = null;

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
                        callbackTyped = (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>)subStep.Item3;
                        break;

                    case SubStepType.CodeGeneric:
                        callbackGeneric = (DatabaseProcessingStepDelegate)subStep.Item3;
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
                DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callbackTyped = subStep as DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>;
                DatabaseProcessingStepDelegate callbackGeneric = subStep as DatabaseProcessingStepDelegate;

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

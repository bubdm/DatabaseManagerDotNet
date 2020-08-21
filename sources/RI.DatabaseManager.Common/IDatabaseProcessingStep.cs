using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Defines the interface for a single database processing step.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         By adding sub-steps (<see cref="AddScript(string,DatabaseProcessingStepTransactionRequirement)" />, <see cref="AddBatch(string,DatabaseProcessingStepTransactionRequirement)" />, <see cref="AddBatches(IEnumerable{string},DatabaseProcessingStepTransactionRequirement)" />, <see cref="AddCode(DatabaseProcessingStepDelegate,DatabaseProcessingStepTransactionRequirement)" />), a processing step can be heavily customized by using both application code through delegates and/or database scripts.
    ///     </para>
    ///     <para>
    ///         The sub-steps are executed in the order they are added.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseProcessingStep
    {
        /// <summary>
        ///     Gets whether this processing step requires a script locator.
        /// </summary>
        /// <value>
        ///     true if a script locator is required, false otherwise.
        /// </value>
        bool RequiresScriptLocator { get; }

        /// <summary>
        ///     Gets whether this processing step requires a transaction when executed.
        /// </summary>
        /// <value>
        ///     true if a transaction is required, false otherwise.
        /// </value>
        bool RequiresTransaction { get; }

        /// <summary>
        ///     Adds a single batch as database script code.
        /// </summary>
        /// <param name="batch"> The database script. </param>
        /// <remarks>
        ///     <para>
        ///         The database script is executed as passed by this method, without further processing, as a single command.
        ///     </para>
        ///     <para>
        ///         <see cref="DatabaseProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </para>
        ///     <para>
        ///         If <paramref name="batch" /> is null or empty, no sub-step is added.
        ///     </para>
        /// </remarks>
        void AddBatch (string batch);

        /// <summary>
        ///     Adds a single batch as database script code.
        /// </summary>
        /// <param name="batch"> The database script. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <remarks>
        ///     <para>
        ///         The database script is executed as passed by this method, without further processing, as a single command.
        ///     </para>
        ///     <para>
        ///         If <paramref name="batch" /> is null or empty, no sub-step is added.
        ///     </para>
        /// </remarks>
        void AddBatch (string batch, DatabaseProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Adds batches as database script code.
        /// </summary>
        /// <param name="batches"> The database scripts. </param>
        /// <remarks>
        ///     <para>
        ///         The database scripts are executed as passed by this method, without further processing, as a single command per batch.
        ///     </para>
        ///     <para>
        ///         <see cref="DatabaseProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </para>
        ///     <para>
        ///         If <paramref name="batches" /> is null or empty, no sub-step is added.
        ///     </para>
        ///     <para>
        ///         <paramref name="batches" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        void AddBatches (IEnumerable<string> batches);

        /// <summary>
        ///     Adds batches as database script code.
        /// </summary>
        /// <param name="batches"> The database scripts. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <remarks>
        ///     <para>
        ///         The database scripts are executed as passed by this method, without further processing, as a single command per batch.
        ///     </para>
        ///     <para>
        ///         If <paramref name="batches" /> is null or empty, no sub-step is added.
        ///     </para>
        ///     <para>
        ///         <paramref name="batches" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        void AddBatches (IEnumerable<string> batches, DatabaseProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Adds application code as a callback.
        /// </summary>
        /// <param name="callback"> The callback which is executed when the sub-step executes. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="DatabaseProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        void AddCode (DatabaseProcessingStepDelegate callback);

        /// <summary>
        ///     Adds application code as a callback.
        /// </summary>
        /// <param name="callback"> The callback which is executed when the sub-step executes. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        void AddCode (DatabaseProcessingStepDelegate callback, DatabaseProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Adds a script using its script name.
        /// </summary>
        /// <param name="scriptName"> The name of the script. </param>
        /// <remarks>
        ///     <para>
        ///         The script is resolved using the script locator provided by the database manager.
        ///     </para>
        ///     <para>
        ///         <see cref="DatabaseProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="scriptName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="scriptName" /> is an empty string. </exception>
        void AddScript (string scriptName);

        /// <summary>
        ///     Adds a script using its script name.
        /// </summary>
        /// <param name="scriptName"> The name of the script. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <remarks>
        ///     <para>
        ///         The script is resolved using the script locator provided by the database manager.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="scriptName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="scriptName" /> is an empty string. </exception>
        void AddScript (string scriptName, DatabaseProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Executes the processing step and all its sub-steps.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="connection"> The used database connection. </param>
        /// <param name="transaction"> The used database transaction. Can be null if no transaction is used. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> or <paramref name="connection" /> is null. </exception>
        void Execute (IDatabaseManager manager, DbConnection connection, DbTransaction transaction);
    }

    /// <inheritdoc cref="IDatabaseProcessingStep" />
    /// <typeparam name="TConnection"> </typeparam>
    /// <typeparam name="TTransaction"> </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> </typeparam>
    /// <typeparam name="TManager"> </typeparam>
    /// <typeparam name="TConfiguration"> </typeparam>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseProcessingStep <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseProcessingStep
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        /// <summary>
        ///     Adds application code as a callback.
        /// </summary>
        /// <param name="callback"> The callback which is executed when the sub-step executes. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="DatabaseProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        void AddCode (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callback);

        /// <summary>
        ///     Adds application code as a callback.
        /// </summary>
        /// <param name="callback"> The callback which is executed when the sub-step executes. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        void AddCode (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callback, DatabaseProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Executes the processing step and all its sub-steps.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="connection"> The used database connection. </param>
        /// <param name="transaction"> The used database transaction. Can be null if no transaction is used. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> or <paramref name="connection" /> is null. </exception>
        void Execute (TManager manager, TConnection connection, TTransaction transaction);
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     A single database processing step.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         By adding sub-steps (any of the <c>Add...</c> methods), a processing step can be heavily customized by using both application code (using delegates) and/or database scripts.
    ///     </para>
    ///     <para>
    ///         The sub-steps are executed in the order they are added.
    ///     </para>
    /// </remarks>
    public interface IDbProcessingStep
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
        ///     <note type="implement">
        ///         The database script is executed as passed by this method, without further processing, as a single command.
        ///     </note>
        ///     <note type="implement">
        ///         <see cref="DbProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </note>
        ///     <note type="implement">
        ///         If <paramref name="batch" /> is null or empty, no sub-step is added.
        ///     </note>
        /// </remarks>
        void AddBatch (string batch);

        /// <summary>
        ///     Adds a single batch as database script code.
        /// </summary>
        /// <param name="batch"> The database script. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The database script is executed as passed by this method, without further processing, as a single command.
        ///     </note>
        ///     <note type="implement">
        ///         If <paramref name="batch" /> is null or empty, no sub-step is added.
        ///     </note>
        /// </remarks>
        void AddBatch (string batch, DbProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Adds batches as database script code.
        /// </summary>
        /// <param name="batches"> The database scripts. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The database scripts are executed as passed by this method, without further processing, as a single command per batch.
        ///     </note>
        ///     <note type="implement">
        ///         <see cref="DbProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </note>
        ///     <note type="implement">
        ///         If <paramref name="batches" /> is null or empty, no sub-step is added.
        ///     </note>
        ///     <note type="implement">
        ///         Null or empty strings in <paramref name="batches" /> are ignored.
        ///     </note>
        ///     <note type="implement">
        ///         <paramref name="batches" /> is enumerated only once.
        ///     </note>
        /// </remarks>
        void AddBatches (IEnumerable<string> batches);

        /// <summary>
        ///     Adds batches as database script code.
        /// </summary>
        /// <param name="batches"> The database scripts. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The database scripts are executed as passed by this method, without further processing, as a single command per batch.
        ///     </note>
        ///     <note type="implement">
        ///         If <paramref name="batches" /> is null or empty, no sub-step is added.
        ///     </note>
        ///     <note type="implement">
        ///         Null or empty strings in <paramref name="batches" /> are ignored.
        ///     </note>
        ///     <note type="implement">
        ///         <paramref name="batches" /> is enumerated only once.
        ///     </note>
        /// </remarks>
        void AddBatches (IEnumerable<string> batches, DbProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Adds application code as a callback.
        /// </summary>
        /// <param name="callback"> The callback which is executed when the sub-step executes. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="DbProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        void AddCode (DbProcessingStepDelegate callback);

        /// <summary>
        ///     Adds application code as a callback.
        /// </summary>
        /// <param name="callback"> The callback which is executed when the sub-step executes. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        void AddCode (DbProcessingStepDelegate callback, DbProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Adds a script using its script name.
        /// </summary>
        /// <param name="scriptName"> The name of the script. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The script is resolved using the script locator provided by the executing database manager.
        ///     </note>
        ///     <note type="implement">
        ///         <see cref="DbProcessingStepTransactionRequirement.DontCare" /> is used as the transaction requirement.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="scriptName" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="scriptName" /> is an empty string. </exception>
        void AddScript (string scriptName);

        /// <summary>
        ///     Adds a script using its script name.
        /// </summary>
        /// <param name="scriptName"> The name of the script. </param>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The script is resolved using the script locator provided by the executing database manager.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="scriptName" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="scriptName" /> is an empty string. </exception>
        void AddScript (string scriptName, DbProcessingStepTransactionRequirement transactionRequirement);

        /// <summary>
        ///     Executes the processing step and all its sub-steps.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="connection"> The used database connection. </param>
        /// <param name="transaction"> The used database transaction. Can be null if no transaction is used. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> or <paramref name="connection" /> is null. </exception>
        /// <exception cref="InvalidOperationException">Conflicting transaction settings are used.</exception>
        void Execute (IDbManager manager, DbConnection connection, DbTransaction transaction);
    }

    /// <inheritdoc cref="IDbProcessingStep" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public interface IDbProcessingStep <TConnection, TTransaction, TManager> : IDbProcessingStep
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction, TManager>
    {
        /// <inheritdoc cref="IDbProcessingStep.AddCode(DbProcessingStepDelegate)"/>
        void AddCode (DbProcessingStepDelegate<TConnection, TTransaction, TManager> callback);

        /// <inheritdoc cref="IDbProcessingStep.AddCode(DbProcessingStepDelegate,DbProcessingStepTransactionRequirement)"/>
        void AddCode (DbProcessingStepDelegate<TConnection, TTransaction, TManager> callback, DbProcessingStepTransactionRequirement transactionRequirement);

        /// <inheritdoc cref="IDbProcessingStep.Execute"/>
        void Execute (TManager manager, TConnection connection, TTransaction transaction);
    }
}

using System;
using System.Data;
using System.Data.Common;

using RI.DatabaseManager.Batches.Commands;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     A single database command which is part of a database batch.
    /// </summary>
    public interface IDbBatchCommand : ICloneable
    {
        /// <summary>
        ///     Gets the code callback of this command (if used).
        /// </summary>
        /// <value>
        ///     The code callback of this command or null if a script is used instead of code (<see cref="Script" />).
        /// </value>
        CallbackBatchCommandDelegate Code { get; }

        /// <summary>
        ///     Gets or sets the result of the last execution of this command.
        /// </summary>
        /// <value>
        ///     The result of the last execution of this command or null if the command was not executed. Note that "no result" is indicated by a return value of <see cref="DBNull" />.
        /// </value>
        object Result { get; set; }

        /// <summary>
        ///     Gets or sets the exception of the last execution of this command.
        /// </summary>
        /// <value>
        ///     The exception of the last execution of this command or null if the command was not executed or no exception was thrown.
        /// </value>
        Exception Exception { get; set; }

        /// <summary>
        ///     Gets or sets the database provider specified error of the last execution of this command.
        /// </summary>
        /// <value>
        ///     The database provider specified error of the last execution of this command or null if the command was not executed or no error occurred.
        /// </value>
        string Error { get; set; }

        /// <summary>
        ///     Gets the database script code of this command (if used).
        /// </summary>
        /// <value>
        ///     The database script code of this command or null if code is used instead of a script (<see cref="Code" />).
        /// </value>
        string Script { get; }

        /// <summary>
        ///     Gets whether this command requires a transaction when executed.
        /// </summary>
        /// <value>
        ///     One of the <see cref="DbBatchTransactionRequirement" /> values.
        /// </value>
        DbBatchTransactionRequirement TransactionRequirement { get; }

        /// <summary>
        /// Gets the used isolation level, if any.
        /// </summary>
        /// <value>
        /// The used isolation level or null if none is provided or used.
        /// </value>
        /// <remarks>
        /// <para>
        /// <see cref="IsolationLevel"/> is only used with transactions.
        /// </para>
        /// </remarks>
        IsolationLevel? IsolationLevel { get; }

        /// <summary>
        ///     Gets or sets whether this command was executed.
        /// </summary>
        /// <value>
        ///     true if the command was executed, false otherwise.
        /// </value>
        bool WasExecuted { get; set; }

        /// <summary>
        ///     Gets the collection of parameters which are applied to this command.
        /// </summary>
        /// <value>
        ///     The collection of parameters which are applied to this command.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         This property should never be null.
        ///     </note>
        /// </remarks>
        IDbBatchCommandParameterCollection Parameters { get; }
    }

    /// <inheritdoc cref="IDbBatch" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbBatchCommand<TConnection, TTransaction, TParameterTypes> : IDbBatchCommand
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbBatchCommand.Code" />
        new CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> Code { get; }

        /// <inheritdoc cref="IDbBatch.Parameters" />
        new IDbBatchCommandParameterCollection<TParameterTypes> Parameters { get; }
    }
}

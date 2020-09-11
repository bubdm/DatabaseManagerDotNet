using System;
using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     A single database command which is part of a database batch.
    /// </summary>
    public interface IDbBatchCommand
    {
        /// <summary>
        ///     Gets the code callback of this command (if used).
        /// </summary>
        /// <value>
        ///     The code callback of this command or null if a script is used instead of code (<see cref="Script" />).
        /// </value>
        Func<DbConnection, DbTransaction, object> Code { get; }

        /// <summary>
        ///     Gets or sets the result of the last execution of this command.
        /// </summary>
        /// <value>
        ///     The result of the last execution of this command or null if the command was not executed. Note that "no result" is indicated by a return value of <see cref="DBNull" />.
        /// </value>
        object Result { get; set; }

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
        ///     Gets or sets whether this command was executed.
        /// </summary>
        /// <value>
        ///     true if the command was executed, false otherwise.
        /// </value>
        bool WasExecuted { get; set; }
    }
}

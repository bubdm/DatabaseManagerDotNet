using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Commands
{
    /// <summary>
    ///     Callback delegate for code-based commands.
    /// </summary>
    /// <param name="connection"> The used database connection. </param>
    /// <param name="transaction"> The used database transaction or null if no transaction is used. </param>
    /// <param name="parameters"> The parameters used in the command. </param>
    /// <param name="error"> The database specific error which occurred during execution (or null if none is available). </param>
    /// <param name="exception">
    ///     The database specific exception which occurred during execution (or null if none is
    ///     available).
    /// </param>
    /// <returns>
    ///     The result of the code callback.
    /// </returns>
    public delegate object CallbackBatchCommandDelegate (DbConnection connection, DbTransaction transaction,
                                                         DbBatchExecutionType executionType,
                                                         IDbBatchCommandParameterCollection parameters,
                                                         out string error, out Exception exception);

    /// <summary>
    ///     Callback delegate for code-based commands.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <param name="connection"> The used database connection. </param>
    /// <param name="transaction"> The used database transaction or null if no transaction is used. </param>
    /// <param name="parameters"> The parameters used in the command. </param>
    /// <param name="error"> The database specific error which occurred during execution (or null if none is available). </param>
    /// <param name="exception">
    ///     The database specific exception which occurred during execution (or null if none is
    ///     available).
    /// </param>
    /// <returns>
    ///     The list of results of the code callback.
    /// </returns>
    public delegate List<object> CallbackBatchCommandDelegate <TConnection, TTransaction, TParameterTypes> (
        TConnection connection, TTransaction transaction, DbBatchExecutionType executionType,
        IDbBatchCommandParameterCollection<TParameterTypes> parameters, out string error, out Exception exception)
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum;
}

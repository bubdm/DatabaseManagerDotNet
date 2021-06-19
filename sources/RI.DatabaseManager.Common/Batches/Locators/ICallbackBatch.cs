using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Callback batch code contract.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <remarks>
    ///     <para>
    ///         See <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction,TParameterTypes}" /> for more details.
    ///     </para>
    /// </remarks>
    public interface ICallbackBatch <TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <summary>
        ///     Executes the batch code.
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
        ///     The results of the code callback.
        /// </returns>
        List<object> Execute (TConnection connection, TTransaction transaction,
                              IDbBatchCommandParameterCollection<TParameterTypes> parameters, out string error,
                              out Exception exception);
    }
}

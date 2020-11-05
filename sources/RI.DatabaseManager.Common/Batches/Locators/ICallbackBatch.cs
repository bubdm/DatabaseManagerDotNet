using System;
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
    public interface ICallbackBatch<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where  TParameterTypes : Enum
    {
        /// <summary>
        ///     Executes the batch code.
        /// </summary>
        /// <param name="connection"> The used database connection. </param>
        /// <param name="transaction"> The used database transaction or null if no transaction is used. </param>
        /// <returns>
        ///     The result of the code callback.
        /// </returns>
        object Execute (TConnection connection, TTransaction transaction);
    }
}

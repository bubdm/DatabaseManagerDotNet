using System.Data.Common;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Callback batch code contract.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <remarks>
    ///     <para>
    ///         See <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction}" /> for more details.
    ///     </para>
    /// </remarks>
    public interface ICallbackBatch<TConnection, TTransaction>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
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

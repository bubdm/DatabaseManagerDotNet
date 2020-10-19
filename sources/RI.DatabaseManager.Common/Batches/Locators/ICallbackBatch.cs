using System.Data.Common;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Callback batch code contract.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction}" /> for more details.
    ///     </para>
    /// </remarks>
    /// TODO: Make generic
    public interface ICallbackBatch
    {
        /// <summary>
        ///     Executes the batch code.
        /// </summary>
        /// <param name="connection"> The used database connection. </param>
        /// <param name="transaction"> The used database transaction or null if no transaction is used. </param>
        /// <returns>
        ///     The result of the code callback.
        /// </returns>
        object Execute (DbConnection connection, DbTransaction transaction);
    }
}

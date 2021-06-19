using System.Data;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    ///     Stores general database manager options and also provides the means to create the database if it does not yet exist
    ///     (is in the <see cref="DbState.New" /> state).
    /// </summary>
    public interface ISupportDefaultDatabaseCreation : IDbManagerOptions
    {
        /// <summary>
        ///     Gets the default create script.
        /// </summary>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <param name="isolationLevel"> The isolation level requirement. </param>
        /// <param name="executionType"> The optional execution type specification. </param>
        /// <returns>
        ///     The array with the commands of the default create script or null or an empty array if a default create script is
        ///     not available.
        /// </returns>
        string[] GetDefaultCreationScript (out DbBatchTransactionRequirement transactionRequirement,
                                           out IsolationLevel? isolationLevel, out DbBatchExecutionType executionType);
    }
}

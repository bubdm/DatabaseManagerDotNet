using System.Data;

using RI.DatabaseManager.Batches;




namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    /// Stores general database manager options and also provides the means to cleanup the database.
    /// </summary>
    public interface ISupportDefaultDatabaseCleanup : IDbManagerOptions
    {
        /// <summary>
        /// Gets the default cleanup script.
        /// </summary>
        /// <param name="transactionRequirement">The transaction requirement.</param>
        /// <param name="isolationLevel">The isolation level requirement.</param>
        /// <returns>
        /// The array with the commands of the default cleanup script or null or an empty array if a default cleanup script is not available.
        /// </returns>
        string[] GetDefaultCleanupScript(out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel);
    }
}

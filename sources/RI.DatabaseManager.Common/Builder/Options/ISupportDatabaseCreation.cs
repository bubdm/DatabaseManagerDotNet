using System.Data;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    /// Stores general database manager options and also provides the means to create the database if it does not yet exist (is in the <see cref="DbState.New"/> state).
    /// </summary>
    public interface ISupportDatabaseCreation : IDbManagerOptions
    {
        /// <summary>
        /// Gets the default setup script.
        /// </summary>
        /// <param name="transactionRequirement">The transaction requirement.</param>
        /// <param name="isolationLevel">The isolation level requirement.</param>
        /// <returns>
        /// The array with the commands of the default setup script or null or an empty array if a default setup script is not available.
        /// </returns>
        string[] GetDefaultSetupScript (out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel);
    }
}

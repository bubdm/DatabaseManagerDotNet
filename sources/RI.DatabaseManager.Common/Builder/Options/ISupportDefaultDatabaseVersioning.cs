using System.Data;

using RI.DatabaseManager.Batches;




namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    /// Stores general database manager options and also provides the means to detect the version the database.
    /// </summary>
    public interface ISupportDefaultDatabaseVersioning : IDbManagerOptions
    {
        /// <summary>
        /// Gets the default versioning script.
        /// </summary>
        /// <param name="transactionRequirement">The transaction requirement.</param>
        /// <param name="isolationLevel">The isolation level requirement.</param>
        /// <returns>
        /// The array with the commands of the default versioning script or null or an empty array if a default versioning script is not available.
        /// </returns>
        string[] GetDefaultVersioningScript(out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel);
    }
}

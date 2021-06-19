using System.Data;

using RI.DatabaseManager.Batches;




namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    ///     Stores general database manager options and also provides the means to detect the version the database.
    /// </summary>
    /// <remarks>
    ///     TODO: Check
    ///     <para>
    ///         The script must return a scalar value which indicates the current version of the database.
    ///         The script must return -1 to indicate when the database is damaged or in an invalid state or 0 to indicate that
    ///         the database does not yet exist and needs to be created.
    ///     </para>
    ///     <para>
    ///         If the script contains multiple batches, each batch is executed consecutively.
    ///         The execution stops on the first batch which returns -1.
    ///         If no batch returns -1, the last batch determines the version.
    ///     </para>
    /// </remarks>
    public interface ISupportDefaultDatabaseVersioning : IDbManagerOptions
    {
        /// <summary>
        ///     Gets the default versioning script.
        /// </summary>
        /// <param name="transactionRequirement"> The transaction requirement. </param>
        /// <param name="isolationLevel"> The isolation level requirement. </param>
        /// <param name="executionType"> The optional execution type specification. </param>
        /// <returns>
        ///     The array with the commands of the default versioning script or null or an empty array if a default versioning
        ///     script is not available.
        /// </returns>
        string[] GetDefaultVersioningScript (out DbBatchTransactionRequirement transactionRequirement,
                                             out IsolationLevel? isolationLevel,
                                             out DbBatchExecutionType executionType);
    }
}

using System;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Describes the transaction requirements of a batch or a batch command.
    /// </summary>
    [Serializable,]
    public enum DbBatchTransactionRequirement
    {
        /// <summary>
        ///     The batch/command has no transaction requirement and can be used with or without a transaction.
        /// </summary>
        DontCare = 0,

        /// <summary>
        ///     The batch/command requires a transaction.
        /// </summary>
        Required = 1,

        /// <summary>
        ///     The batch/command cannot be used with a transaction.
        /// </summary>
        Disallowed = 2,
    }
}

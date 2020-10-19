using System;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Optional attribute to override batch name and/or transaction requirement of <see cref="ICallbackBatch" /> implementations.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction}" /> for more details.
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class),]
    public sealed class CallbackBatchAttribute : Attribute
    {
        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets the name of the batch.
        /// </summary>
        /// <value>
        ///     The name of the batch or null if the batch name is not specified or overriden respectively.
        /// </value>
        public string Name { get; set; } = null;

        /// <summary>
        ///     Gets or sets the transaction requirement.
        /// </summary>
        /// <value>
        ///     The transaction requirement.
        /// </value>
        public DbBatchTransactionRequirement TransactionRequirement { get; set; } = DbBatchTransactionRequirement.DontCare;

        #endregion
    }
}

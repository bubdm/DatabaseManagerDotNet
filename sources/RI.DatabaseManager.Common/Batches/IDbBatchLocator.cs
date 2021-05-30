using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Batch locator to locate and retrieve database batches and their commands.
    /// </summary>
    /// <remarks>
    ///     <note type="note">
    ///         <see cref="IDbBatchLocator" /> implementations are used by database managers.
    ///         Do not use <see cref="IDbBatchLocator" /> implementations directly.
    ///     </note>
    /// </remarks>
    public interface IDbBatchLocator
    {
        /// <summary>
        ///     Retrieves the batch with a specified name.
        /// </summary>
        /// <param name="name"> The name of the batch. </param>
        /// <param name="commandSeparator">
        ///     The string which is used as the separator to separate commands within the batch or null
        ///     if the batch locators default separators are to be used.
        /// </param>
        /// <param name="batchCreator">
        ///     The batch creator provided by the used database manager which is used to create proper
        ///     batch instances (implementations of <see cref="IDbBatch" />).
        /// </param>
        /// <returns>
        ///     The batch or null if the batch of the specified name could not be found.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="batchCreator" /> is null. </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="name" /> or <paramref name="commandSeparator" /> is an empty
        ///     string.
        /// </exception>
        IDbBatch GetBatch (string name, string commandSeparator, Func<IDbBatch> batchCreator);

        /// <summary>
        ///     Gets the names of all available batches this batch locator can retrieve.
        /// </summary>
        /// <returns>
        ///     The set with the names of all available batches.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetNames" /> should never return null.
        ///     </note>
        /// </remarks>
        ISet<string> GetNames ();
    }

    /// <inheritdoc cref="IDbBatchLocator" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbBatchLocator <TConnection, TTransaction, TParameterTypes> : IDbBatchLocator
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbBatchLocator.GetBatch" />
        IDbBatch<TConnection, TTransaction, TParameterTypes> GetBatch (string name, string commandSeparator,
                                                                       Func<IDbBatch<TConnection, TTransaction,
                                                                           TParameterTypes>> batchCreator);
    }
}

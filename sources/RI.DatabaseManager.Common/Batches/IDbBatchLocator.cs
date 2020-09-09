using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Batch locator to locate and retrieve database batches.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database batch locators are used to locate, retrieve, and preprocess database batches.
    ///     </para>
    /// <para>
    /// What the preprocessing does in detail depends on the implementation of <see cref="IDbBatchLocator" /> but is usually something like replacing placeholders (e.g. current date and time), etc.
    /// </para>
    /// <para>
    /// The database batches can be further divided into commands, separated by <see cref="DefaultCommandSeparator"/>, so that, for example, parts of a SQL script (which would be a single batch) can be executed in individual database commands or transactions.
    /// </para>
    ///     <note type="note">
    /// <see cref="IDbBatchLocator"/> implementations are used by database managers.
    ///         Do not use <see cref="IDbBatchLocator"/> implementations directly.
    ///     </note>
    /// </remarks>
    public interface IDbBatchLocator
    {
        /// <summary>
        ///     Gets or sets the string which is used as the default separator to separate individual commands in a single batch.
        /// </summary>
        /// <value>
        ///     The string which is used as the default separator to separate individual commands in a single batch or null if the batch is not to be separated into individual commands.
        /// </value>
        /// <remarks>
        ///<note type="note">
        /// Not all command types (<see cref="IDbBatchCommand"/> implementations) can use separators, it applies primarily for scripts (e.g. SQL script files).
        /// </note>
        ///<note type="implement">
        /// The default value of this property is expected to be <c>GO</c>.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="value"/> is an empty string.</exception>
        /// TODO: Keep or remove? Also consider the doc comments!
        string DefaultCommandSeparator { get; set; }

        /// <summary>
        /// Gets the names of all available batches this batch locator can retrieve.
        /// </summary>
        /// <returns>
        /// The set with the names of all available batches.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetNames" /> should never return null.
        ///     </note>
        /// </remarks>
        ISet<string> GetNames ();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <param name="name"> The name of the batch. </param>
        /// <param name="commandSeparator"> The string which is used as the separator to separate commands within the batch or null if the batch locators default separators are to be used. </param>
        /// <param name="preprocess"> Specifies whether the batch is to be preprocessed, if applicable. </param>
        /// <returns>
        ///     The batch or null if the batch of the specified name could not be found.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> or <paramref name="commandSeparator" /> is an empty string. </exception>
        IDbBatch<TConnection, TTransaction> GetBatch<TConnection, TTransaction>(string name, string commandSeparator, bool preprocess)
            where TConnection : DbConnection
            where TTransaction : DbTransaction;
    }
}

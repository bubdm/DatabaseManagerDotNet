using System.Collections.Generic;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     A single database batch which groups multiple commands into one unit.
    /// </summary>
    public interface IDbBatch
    {
        /// <summary>
        ///     Gets the list of all commands of this batch.
        /// </summary>
        /// <value>
        ///     The list of all commands of this batch.
        /// </value>
        /// <remarks>
        ///     <note type="important">
        ///         The order of the commands in the list is the order the commands are supposed to be used.
        ///     </note>
        ///     <note type="implement">
        ///         This property should never be null.
        ///     </note>
        /// </remarks>
        List<IDbBatchCommand> Commands { get; }
    }
}

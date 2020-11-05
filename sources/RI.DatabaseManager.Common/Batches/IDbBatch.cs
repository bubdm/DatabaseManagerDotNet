using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     A single database batch which groups multiple commands into one unit.
    /// </summary>
    public interface IDbBatch : ICloneable
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
        IList<IDbBatchCommand> Commands { get; }
    }

    /// <inheritdoc cref="IDbBatch" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbBatch <TConnection, TTransaction, TParameterTypes> : IDbBatch
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbBatch.Commands" />
        new IList<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>> Commands { get; }
    }
}

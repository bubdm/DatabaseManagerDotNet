using System;
using System.Collections.Generic;
using System.Data;
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

        /// <summary>
        ///     Gets the collection of parameters which are applied to all commands.
        /// </summary>
        /// <value>
        ///     The collection of parameters which are applied to all commands.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         This property should never be null.
        ///     </note>
        /// </remarks>
        IDbBatchCommandParameterCollection Parameters { get; }

        /// <summary>
        /// Gets the used isolation level, if any.
        /// </summary>
        /// <value>
        /// The used isolation level or null if none is provided or used.
        /// </value>
        /// <remarks>
        /// <para>
        /// <see cref="IsolationLevel"/> is only used with transactions.
        /// </para>
        /// </remarks>
        IsolationLevel? IsolationLevel { get; }
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

        /// <inheritdoc cref="IDbBatch.Parameters" />
        new IDbBatchCommandParameterCollection<TParameterTypes> Parameters { get; }
    }
}

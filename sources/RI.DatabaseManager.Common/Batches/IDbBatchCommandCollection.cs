using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using RI.DatabaseManager.Batches.Commands;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     A collection (set) of batch commands.
    /// </summary>
    public interface IDbBatchCommandCollection : ICloneable
    {
        /// <summary>
        /// Gets all commands of the collection.
        /// </summary>
        /// <returns>
        /// The list of all commands.
        /// If there are no commands, an empty list is returned.
        /// </returns>
        IList<IDbBatchCommand> GetAll();

        /// <summary>
        /// Clears the collection of all commands.
        /// </summary>
        void ClearCommands();
    }

    /// <inheritdoc cref="RI.DatabaseManager.Batches.IDbBatchCommandCollection" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbBatchCommandCollection<TConnection, TTransaction, TParameterTypes> : IDbBatchCommandCollection, IList<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <summary>
        /// Adds multiple commands to the collection.
        /// </summary>
        /// <param name="commands">The sequence of commands to add.</param>
        /// <remarks>
        /// <para>
        /// <paramref name="commands"/> is only enumerated once.
        /// </para>
        /// </remarks>
        void AddRange(IEnumerable<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>> commands);

        /// <summary>
        /// Adds a callback command to the collection.
        /// </summary>
        /// <param name="callback"> The delegate to the callback which is called when the command is executed. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <returns>
        /// The created and added command.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null. </exception>
        IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddCallback (CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null);

        /// <summary>
        /// Adds a script command to the collection.
        /// </summary>
        /// <param name="script"> The database script. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <returns>
        /// The created and added command.
        /// </returns>
        IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScript(string script, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null);
    }
}

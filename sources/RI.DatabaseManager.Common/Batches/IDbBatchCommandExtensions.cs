using System;
using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IDbBatchCommand" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class IDbBatchCommandExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Resets the state from the last execution of this command.
        /// </summary>
        /// <param name="command"> The used command. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
        public static void Reset (this IDbBatchCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.Result = null;
            command.WasExecuted = false;
        }

        /// <summary>
        /// Merges parameters of a command with those from a batch.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <param name="command"> The used command. </param>
        /// <param name="batch">The batch whose parameters are merged with the one from the command.</param>
        /// <returns>
        /// The collection which contains all merged parameters from <paramref name="command"/> and <paramref name="batch"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="command" /> or <paramref name="batch"/> is null. </exception>
        public static DbBatchCommandParameterCollection<TParameterTypes> MergeParameters <TConnection, TTransaction, TParameterTypes> (this IDbBatchCommand<TConnection, TTransaction, TParameterTypes> command, IDbBatch<TConnection, TTransaction, TParameterTypes> batch)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            DbBatchCommandParameterCollection<TParameterTypes> merged = new DbBatchCommandParameterCollection<TParameterTypes>();

            foreach (IDbBatchCommandParameter<TParameterTypes> parameter in batch.Parameters)
            {
                merged.Add((IDbBatchCommandParameter<TParameterTypes>)parameter.Clone());
            }

            foreach (IDbBatchCommandParameter<TParameterTypes> parameter in command.Parameters)
            {
                merged.Add((IDbBatchCommandParameter<TParameterTypes>)parameter.Clone());
            }

            return merged;
        }

        #endregion
    }
}

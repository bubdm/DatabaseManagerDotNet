using System;




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

        #endregion
    }
}

using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using RI.DatabaseManager.Batches.Commands;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IDbBatch" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class IDbBatchExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Adds a code callback to the batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <param name="callback"> The callback. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <returns>
        ///     The index in the list of commands the callback was added.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         If <paramref name="callback" /> is null, null is added to the command list.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static int AddCode <TConnection, TTransaction> (this IDbBatch batch, Func<TConnection, TTransaction, object> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            batch.Commands.Add(callback == null ? null : new CallbackBatchCommand<TConnection, TTransaction>(callback, transactionRequirement));
            return batch.Commands.Count - 1;
        }

        /// <summary>
        ///     Adds a code callback to the batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <param name="callback"> The callback. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <returns>
        ///     The index in the list of commands the callback was added.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         If <paramref name="callback" /> is null, null is added to the command list.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static int AddCode (this IDbBatch batch, Func<DbConnection, DbTransaction, object> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            batch.Commands.Add(callback == null ? null : new CallbackBatchCommand(callback, transactionRequirement));
            return batch.Commands.Count - 1;
        }

        /// <summary>
        ///     Adds a database script to the batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <param name="script"> The script. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         If <paramref name="script" /> is null, null is added to the command list.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static int AddScript (this IDbBatch batch, string script, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            batch.Commands.Add(script == null ? null : new ScriptBatchCommand(script, transactionRequirement));
            return batch.Commands.Count - 1;
        }

        public static int AddScriptFromReader (this IDbBatch batch, TextReader reader, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            //TODO: Implement
        }

        public static int AddScriptFromStream (this IDbBatch batch, Stream stream, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            //TODO: Implement
        }

        public static int AddScriptFromFile (this IDbBatch batch, string file, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            //TODO: Implement
        }

        public static int AddScriptFromAssemblyResource (this IDbBatch batch, Assembly assembly, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            //TODO: Implement
        }

        public static int AddCode (this IDbBatch batch, Type callbackImplementation, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
        {
            //TODO: Implement
        }

        /// <summary>
        ///     Gets the result of the last executed command from the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     The result of the last executed command or null if no command was executed.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static object GetLastResult (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            object result = null;

            foreach (IDbBatchCommand command in batch.Commands)
            {
                if (command?.WasExecuted ?? false)
                {
                    result = command.Result;
                }
            }

            return result;
        }

        /// <summary>
        ///     Determines whether a batch requires a transaction to execute.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     true if a transaction is required, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> Conflicting transaction requirements are used (e.g. one command uses <see cref="DbBatchTransactionRequirement.Required" /> while another uses <see cref="DbBatchTransactionRequirement.Disallowed" />). </exception>
        public static bool RequiresTransaction (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            bool required = batch.Commands.Any(x => (x?.TransactionRequirement ?? DbBatchTransactionRequirement.DontCare) == DbBatchTransactionRequirement.Required);
            bool disallowed = batch.Commands.Any(x => (x?.TransactionRequirement ?? DbBatchTransactionRequirement.DontCare) == DbBatchTransactionRequirement.Disallowed);

            if (required && disallowed)
            {
                throw new InvalidOperationException("Conflicting transaction requirements.");
            }

            return required;
        }

        /// <summary>
        ///     Resets the states of all commands of the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static void Reset (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            foreach (IDbBatchCommand command in batch.Commands)
            {
                command?.Reset();
            }
        }

        /// <summary>
        ///     Determines whether a batch was fully executed (all of the commands were executed).
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     true if all commands were executed, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static bool WasFullyExecuted (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            return batch.Commands.All(x => x?.WasExecuted ?? true);
        }

        /// <summary>
        ///     Determines whether a batch was at least partially executed (some or all of the commands were executed).
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     true if some or all commands were executed, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static bool WasPartiallyExecuted (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            return batch.Commands.Any(x => x?.WasExecuted ?? true);
        }

        #endregion
    }
}

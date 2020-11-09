using System;
using System.Collections.Generic;
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
        ///     Adds a code callback to the batch as a single command.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
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
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddCode <TConnection, TTransaction, TParameterTypes> (this IDbBatch<TConnection,TTransaction, TParameterTypes> batch, Func<TConnection, TTransaction, IDbBatchCommandParameterCollection<TParameterTypes>, object> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            CallbackBatchCommand<TConnection, TTransaction, TParameterTypes> command = callback == null ? null : new CallbackBatchCommand<TConnection, TTransaction, TParameterTypes>(callback, transactionRequirement);

            batch.Commands.Add(command);

            return command;
        }

        /// <summary>
        ///     Adds a database script to the batch as a single command.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
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
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScript<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, string script, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> command = script == null ? null : new ScriptBatchCommand<TConnection, TTransaction, TParameterTypes>(script, transactionRequirement);

            batch.Commands.Add(command);

            return command;
        }

        /// <summary>
        /// Adds a database script to the batch as a single command.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <param name="batch"> The batch. </param>
        /// <param name="reader"> The text reader from which the script is read. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> or <paramref name="reader"/> is null. </exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScriptFromReader<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, TextReader reader, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return batch.AddScript(reader.ReadToEnd(), transactionRequirement);
        }

        /// <summary>
        /// Adds a database script to the batch as a single command.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <param name="batch"> The batch. </param>
        /// <param name="stream"> The stream from which the script is read. </param>
        /// <param name="encoding"> The optional encoding to read the script. Default value is null, using <see cref="Encoding.UTF8"/>. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> or <paramref name="stream"/> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable.</exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScriptFromStream<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, Stream stream, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream to read script is not readable.", nameof(stream));
            }

            using (StreamReader sr = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                return batch.AddScriptFromReader(sr, transactionRequirement);
            }
        }

        /// <summary>
        /// Adds a database script to the batch as a single command.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <param name="batch"> The batch. </param>
        /// <param name="file"> The file from which the script is read. </param>
        /// <param name="encoding"> The optional encoding to read the script. Default value is null, using <see cref="Encoding.UTF8"/>. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> or <paramref name="file"/> is null. </exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScriptFromFile<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, string file, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return batch.AddScriptFromStream(fs, encoding, transactionRequirement);
            }
        }

        /// <summary>
        /// Adds a database script to the batch as a single command.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <param name="batch"> The batch. </param>
        /// <param name="assembly"> The assembly which contains the resource named by <paramref name="name"/>. </param>
        /// <param name="name"> The embedded assembly resource name of the script. </param>
        /// <param name="encoding"> The optional encoding to read the script. Default value is null, using <see cref="Encoding.UTF8"/>. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" />, <paramref name="assembly"/>, or <paramref name="name"/> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScriptFromAssemblyResource<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, Assembly assembly, string name, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The string argument is empty.", nameof(name));
            }

            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                return batch.AddScriptFromStream(stream, encoding, transactionRequirement);
            }
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
        ///     Determines whether a batch disallows transactions for execution.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     true if transactions is disallowed, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> Conflicting transaction requirements are used (e.g. one command uses <see cref="DbBatchTransactionRequirement.Required" /> while another uses <see cref="DbBatchTransactionRequirement.Disallowed" />). </exception>
        public static bool DisallowsTransaction(this IDbBatch batch)
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

            return disallowed;
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

        /// <summary>
        ///     Determines whether a batch is empty (has no commands).
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     true if the batch is empty (has no commands), false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static bool IsEmpty (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            return batch.Commands.Count == 0;
        }

        /// <summary>
        ///     Clears all commands of a batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static void Clear (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            batch.Commands.Clear();
            batch.Reset();
        }

        /// <summary>
        /// Splits the commands of a batch into separate batches (one batch per command).
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <param name="batch"> The batch to split. </param>
        /// <param name="filter">An optional predicate to filter the commands to split. The default value splits all commands.</param>
        /// <returns>
        /// A list of batches, each with a single command.
        /// If <paramref name="batch"/> contains no commands or no commands passed <paramref name="filter"/>, an empty list is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static IList<DbBatch<TConnection, TTransaction, TParameterTypes>> SplitCommands <TConnection, TTransaction, TParameterTypes> (this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, Predicate<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>> filter = null)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            filter ??= _ => true;

            List<DbBatch<TConnection, TTransaction, TParameterTypes>> batches = new List<DbBatch<TConnection, TTransaction, TParameterTypes>>();

            foreach (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> command in batch.Commands)
            {
                if (filter(command))
                {
                    DbBatch<TConnection, TTransaction, TParameterTypes> splittedBatch = new DbBatch<TConnection, TTransaction, TParameterTypes>();
                    splittedBatch.Commands.Add(command);
                    batches.Add(splittedBatch);
                }
            }

            return batches;
        }

        #endregion
    }
}

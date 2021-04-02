using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <returns>
        ///     The index in the list of commands the callback was added.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         If <paramref name="callback" /> is null, null is added to the command list.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddCode <TConnection, TTransaction, TParameterTypes> (this IDbBatch<TConnection,TTransaction, TParameterTypes> batch, CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            CallbackBatchCommand<TConnection, TTransaction, TParameterTypes> command = callback == null ? null : new CallbackBatchCommand<TConnection, TTransaction, TParameterTypes>(callback, transactionRequirement, isolationLevel);

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
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         If <paramref name="script" /> is null, null is added to the command list.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScript<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, string script, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> command = script == null ? null : new ScriptBatchCommand<TConnection, TTransaction, TParameterTypes>(script, transactionRequirement, isolationLevel);

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
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> or <paramref name="reader"/> is null. </exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScriptFromReader<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, TextReader reader, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
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

            return batch.AddScript(reader.ReadToEnd(), transactionRequirement, isolationLevel);
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
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> or <paramref name="stream"/> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable.</exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScriptFromStream<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, Stream stream, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
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
                return batch.AddScriptFromReader(sr, transactionRequirement, isolationLevel);
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
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> or <paramref name="file"/> is null. </exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScriptFromFile<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, string file, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
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
                return batch.AddScriptFromStream(fs, encoding, transactionRequirement, isolationLevel);
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
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <returns>
        ///     The index in the list of commands the script was added.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" />, <paramref name="assembly"/>, or <paramref name="name"/> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
        public static IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScriptFromAssemblyResource<TConnection, TTransaction, TParameterTypes>(this IDbBatch<TConnection, TTransaction, TParameterTypes> batch, Assembly assembly, string name, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
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
                return batch.AddScriptFromStream(stream, encoding, transactionRequirement, isolationLevel);
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
        public static object GetResult (this IDbBatch batch)
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
        ///     Gets all results of the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     The list of all results of all executed commands.
        /// The list is empty if no command was executed.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static List<object> GetResults(this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            return batch.Commands.Where(x => x.WasExecuted)
                        .Select(x => x.Result)
                        .ToList();
        }

        /// <summary>
        ///     Gets the error of the last executed command from the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     The error of the last executed command or null if no command was executed.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static string GetError(this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            string result = null;

            foreach (IDbBatchCommand command in batch.Commands)
            {
                if (command?.WasExecuted ?? false)
                {
                    string candidate = command.Error;

                    if (!string.IsNullOrWhiteSpace(candidate))
                    {
                        result = candidate;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Gets all errors of the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     The list of all errors of all executed commands.
        /// The list is empty if no command was executed.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static List<string> GetErrors(this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            return batch.Commands.Where(x => x.WasExecuted)
                        .Select(x => x.Error)
                        .ToList();
        }

        /// <summary>
        ///     Gets the exception of the last executed command from the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     The exception of the last executed command or null if no command was executed.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static Exception GetException(this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            Exception result = null;

            foreach (IDbBatchCommand command in batch.Commands)
            {
                if (command?.WasExecuted ?? false)
                {
                    Exception candidate = command.Exception;

                    if (candidate != null)
                    {
                        result = candidate;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Gets all exceptions of the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     The list of all exceptions of all executed commands.
        /// The list is empty if no command was executed.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static List<Exception> GetExceptions(this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            return batch.Commands.Where(x => x.WasExecuted)
                        .Select(x => x.Exception)
                        .ToList();
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
        ///     Determines the required isolation level of a batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     The isolation level of the batch or null if none is specified.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> Conflicting isolation level requirements are used (e.g. one command uses <see cref="IsolationLevel.ReadCommitted" /> while another uses <see cref="IsolationLevel.Serializable" />). </exception>
        public static IsolationLevel? GetRequiredIsolationLevel (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            IsolationLevel? isolationLevel = null;

            for (int i1 = 0; i1 < batch.Commands.Count; i1++)
            {
                IDbBatchCommand command = batch.Commands[i1];

                if (isolationLevel == null)
                {
                    isolationLevel = command.IsolationLevel;
                }
                else if (i1 >= 1)
                {
                    if (command.IsolationLevel != null)
                    {
                        if (command.IsolationLevel != batch.Commands[i1 - 1].IsolationLevel)
                        {
                            throw new InvalidOperationException("Conflicting isolation level requirements.");
                        }
                    }
                }
            }

            return isolationLevel;
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
        ///     Determines whether a batch has failed (has any command witherror or exception).
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <returns>
        ///     true if the batch has failed (at least in execution of one command), false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="batch" /> is null. </exception>
        public static bool HasFailed (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            return batch.Commands.Any(x => (!string.IsNullOrWhiteSpace(x.Error)) || (x.Exception != null));
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
                    splittedBatch.Commands.Add((IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)command.Clone());

                    foreach (IDbBatchCommandParameter<TParameterTypes> parameter in batch.Parameters)
                    {
                        if (splittedBatch.Parameters.Contains(parameter.Name))
                        {
                            splittedBatch.Parameters.Remove(parameter.Name);

                        }

                        splittedBatch.Parameters.Add((IDbBatchCommandParameter<TParameterTypes>)parameter.Clone());
                    }

                    batches.Add(splittedBatch);
                }
            }

            return batches;
        }

        /// <summary>
        /// Merges multiple batches into one.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <param name="batches"> The sequence of batches to merge. </param>
        /// <param name="filter">An optional predicate to filter the batches to merge. The default value merges all batches.</param>
        /// <returns>
        /// The single batch which contains all merged batches.
        /// </returns>
        /// <remarks>
        /// <note type="note">
        /// All batches in <paramref name="batches"/> must have the same isolation level, otherwise <see cref="ArgumentException"/> is thrown.
        /// Batches without isolation level specified will not be checked for isolation level equality.
        /// </note>
        /// <para>
        /// <paramref name="batches"/> is enumerated only once.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="batches" /> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="batches"/> contains batches with different isolation levels.</exception>
        public static DbBatch<TConnection, TTransaction, TParameterTypes> MergeCommands <TConnection, TTransaction, TParameterTypes>(this IEnumerable<IDbBatch<TConnection, TTransaction, TParameterTypes>> batches, Predicate<IDbBatch<TConnection, TTransaction, TParameterTypes>> filter = null)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            if (batches == null)
            {
                throw new ArgumentNullException(nameof(batches));
            }

            filter ??= _ => true;
            IDbBatch<TConnection, TTransaction, TParameterTypes>[] array = batches.ToArray();
            DbBatch < TConnection, TTransaction, TParameterTypes > merged = new DbBatch<TConnection, TTransaction, TParameterTypes>();

            for (int i1 = 0; i1 < array.Length; i1++)
            {
                IDbBatch<TConnection, TTransaction, TParameterTypes> batch = array[i1];

                if (!filter(batch))
                {
                    continue;
                }

                foreach (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> command in array[i1]
                    .Commands)
                {
                    merged.Commands.Add((IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)command.Clone());
                }

                foreach (IDbBatchCommandParameter<TParameterTypes> parameter in array[i1]
                    .Parameters)
                {
                    if (merged.Parameters.Contains(parameter.Name))
                    {
                        merged.Parameters.Remove(parameter.Name);

                    }

                    merged.Parameters.Add((IDbBatchCommandParameter<TParameterTypes>)parameter.Clone());
                }
            }

            return merged;
        }

        /// <summary>
        /// Rethrows the exception of the last executed command from the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <exception cref="Exception"> The exception of the last executed command from the last execution of this batch. </exception>
        public static void RethrowException (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            Exception exception = batch.GetException();

            if (exception != null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }
        }

        /// <summary>
        /// Rethrows all exceptions of the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <exception cref="AggregateException"> The aggregated exception which contains all exceptions of the last execution of this batch. </exception>
        public static void RethrowExceptions (this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            List<Exception> exceptions = batch.GetExceptions();

            if (exceptions.Count != 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Rethrows the error or exception of the last executed command from the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <exception cref="DbBatchErrorException"> The error of the last executed command from the last execution of this batch. </exception>
        /// <exception cref="Exception"> The exception of the last executed command from the last execution of this batch. </exception>
        public static void RethrowErrorOrException(this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            string error = batch.GetError();

            if (!string.IsNullOrWhiteSpace(error))
            {
                throw new DbBatchErrorException(error);
            }

            batch.RethrowException();
        }

        /// <summary>
        /// Rethrows all errors and exceptions of the last execution of this batch.
        /// </summary>
        /// <param name="batch"> The batch. </param>
        /// <exception cref="AggregateException"> The aggregated exception which contains all errors and exceptions of the last execution of this batch. </exception>
        public static void RethrowErrorsOrExceptions(this IDbBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            List<Exception> exceptions = batch.GetExceptions();
            List<string> errors = batch.GetErrors();
            
            List<Exception> finalExceptions = new List<Exception>();
            finalExceptions.AddRange(exceptions);
            finalExceptions.AddRange(errors.Select(x => new DbBatchErrorException(x)));

            throw new AggregateException(finalExceptions);
        }

        #endregion
    }
}

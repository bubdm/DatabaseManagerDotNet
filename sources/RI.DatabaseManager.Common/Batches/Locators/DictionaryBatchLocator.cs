﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;

using RI.DatabaseManager.Batches.Commands;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which uses a dictionary to hold scripts and/or code.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <remarks>
    ///     <para>
    ///         <see cref="Scripts" /> holds named scripts as strings.
    ///         The keys are the batch name, the value is the script.
    ///     </para>
    ///     <para>
    ///         <see cref="Callbacks" /> holds named callbacks as delegates.
    ///         The keys are the batch name, the value is the callback.
    ///     </para>
    ///     <para>
    ///         <see cref="TransactionRequirements" /> holds information about transaction requirements for scripts or callbacks.
    ///         The keys are the batch name, the value is one of the <see cref="DbBatchTransactionRequirement" /> values.
    ///         If no value is set for a given batch name, <see cref="DbBatchTransactionRequirement.DontCare" /> is used.
    ///     </para>
    ///     <para>
    ///         Currently, the following options are supported which are extracted from the scripts (see <see cref="DbBatchLocatorBase{TConnection,TTransaction,TParameterTypes}.OptionsFormat" /> for more details):
    ///     </para>
    ///     <list type="bullet">
    ///         <item> <c> TransactionRequirement </c> [optional] One of the <see cref="DbBatchTransactionRequirement" /> values (as string), e.g. <c> /* DBMANAGER:TransactionRequirement=Disallowed */ </c> </item>
    ///     </list>
    ///     <note type="note">
    ///         If the <c> TransactionRequirement </c> option is not specified, the value from <see cref="TransactionRequirements" /> is used. If that is also not available, <see cref="DbBatchTransactionRequirement.DontCare" /> is used.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class DictionaryBatchLocator<TConnection, TTransaction, TParameterTypes> : DbBatchLocatorBase<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DictionaryBatchLocator{TConnection,TTransaction,TParameterTypes}" />.
        /// </summary>
        public DictionaryBatchLocator ()
        {
            this.Scripts = new Dictionary<string, string>(this.DefaultNameComparer);
            this.Callbacks = new Dictionary<string, CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes>>(this.DefaultNameComparer);
            this.TransactionRequirements = new Dictionary<string, DbBatchTransactionRequirement>(this.DefaultNameComparer);
            this.IsolationLevels = new Dictionary<string, IsolationLevel?>(this.DefaultNameComparer);
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the dictionary with callbacks as batches.
        /// </summary>
        /// <value>
        ///     The dictionary with callbacks.
        /// </value>
        public Dictionary<string, CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes>> Callbacks { get; }

        /// <summary>
        ///     Gets the dictionary with scripts as batches.
        /// </summary>
        /// <value>
        ///     The dictionary with scripts.
        /// </value>
        public Dictionary<string, string> Scripts { get; }

        /// <summary>
        ///     Gets the dictionary with transaction requirements.
        /// </summary>
        /// <value>
        ///     The dictionary with transaction requirements.
        /// </value>
        public Dictionary<string, DbBatchTransactionRequirement> TransactionRequirements { get; }

        /// <summary>
        ///     Gets the dictionary with isolation level requirements.
        /// </summary>
        /// <value>
        ///     The dictionary with isolation level requirements.
        /// </value>
        public Dictionary<string, IsolationLevel?> IsolationLevels { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override bool FillBatch (IDbBatch<TConnection, TTransaction, TParameterTypes> batch, string name, string commandSeparator)
        {
            DbBatchTransactionRequirement transactionRequirement = this.TransactionRequirements.ContainsKey(name) ? this.TransactionRequirements[name] : DbBatchTransactionRequirement.DontCare;
            IsolationLevel? isolationLevel = this.IsolationLevels.ContainsKey(name) ? this.IsolationLevels[name] : null;

            bool found = false;

            if (this.Callbacks.ContainsKey(name))
            {
                CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> callback = this.Callbacks[name];
                batch.AddCode(callback, transactionRequirement, isolationLevel);
                found = true;
            }

            if (this.Scripts.ContainsKey(name))
            {
                string script = this.Scripts[name];
                List<string> commands = this.SeparateScriptCommands(script, commandSeparator);

                foreach (string command in commands)
                {
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        DbBatchTransactionRequirement commandSpecifiedTransactionRequirement = this.GetTransactionRequirementFromCommandOptions(command);
                        IsolationLevel? commandSpecifiedIsolationLevel = this.GetIsolationLevelFromCommandOptions(command);

                        if ((commandSpecifiedTransactionRequirement != DbBatchTransactionRequirement.DontCare) && (transactionRequirement != DbBatchTransactionRequirement.DontCare))
                        {
                            if (commandSpecifiedTransactionRequirement != transactionRequirement)
                            {
                                throw new InvalidOperationException("Conflicting transaction requirements.");
                            }
                        }

                        if ((commandSpecifiedIsolationLevel != null) && (isolationLevel != null))
                        {
                            if (commandSpecifiedIsolationLevel != isolationLevel)
                            {
                                throw new InvalidOperationException("Conflicting isolation level requirements.");
                            }
                        }

                        if (commandSpecifiedTransactionRequirement != DbBatchTransactionRequirement.DontCare)
                        {
                            transactionRequirement = commandSpecifiedTransactionRequirement;
                        }

                        if (commandSpecifiedIsolationLevel != null)
                        {
                            isolationLevel = commandSpecifiedIsolationLevel;
                        }

                        batch.AddScript(command, transactionRequirement, isolationLevel);
                    }
                }

                found = true;
            }

            return found;
        }

        /// <inheritdoc />
        protected override IEnumerable<string> GetNames ()
        {
            List<string> names = new List<string>();
            names.AddRange(this.Scripts.Keys);
            names.AddRange(this.Callbacks.Keys);
            return names;
        }

        /// <inheritdoc />
        public override bool SupportsScripts => true;

        /// <inheritdoc />
        public override bool SupportsCallbacks => true;

        #endregion

        /// <summary>
        ///     Adds a code callback as a single batch.
        /// </summary>
        /// <param name="batchName"> The name of the batch. </param>
        /// <param name="callback"> The callback. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="batchName" /> or <paramref name="callback"/> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="batchName" /> is an empty string. </exception>
        public void AddCode(string batchName, CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> callback, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
        {
            if (batchName == null)
            {
                throw new ArgumentNullException(nameof(batchName));
            }

            if (string.IsNullOrWhiteSpace(batchName))
            {
                throw new ArgumentException("The string argument is empty.", nameof(batchName));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.Callbacks.Add(batchName, callback);
            this.TransactionRequirements.Add(batchName, transactionRequirement);
            this.IsolationLevels.Add(batchName, isolationLevel);
        }

        /// <summary>
        ///     Adds a database script as a single batch.
        /// </summary>
        /// <param name="batchName"> The name of the batch. </param>
        /// <param name="script"> The script. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="batchName" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="batchName" /> is an empty string. </exception>
        public void AddScript(string batchName, string script, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
        {
            if (batchName == null)
            {
                throw new ArgumentNullException(nameof(batchName));
            }

            if (string.IsNullOrWhiteSpace(batchName))
            {
                throw new ArgumentException("The string argument is empty.", nameof(batchName));
            }

            script ??= string.Empty;

            this.Scripts.Add(batchName, script);
            this.TransactionRequirements.Add(batchName, transactionRequirement);
            this.IsolationLevels.Add(batchName, isolationLevel);
        }

        /// <summary>
        /// Adds a database script as a single batch.
        /// </summary>
        /// <param name="batchName"> The name of the batch. </param>
        /// <param name="reader"> The text reader from which the script is read. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="batchName" /> or <paramref name="reader"/> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="batchName" /> is an empty string. </exception>
        public void AddScriptFromReader(string batchName, TextReader reader, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
        {
            if (batchName == null)
            {
                throw new ArgumentNullException(nameof(batchName));
            }

            if (string.IsNullOrWhiteSpace(batchName))
            {
                throw new ArgumentException("The string argument is empty.", nameof(batchName));
            }

            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.AddScript(batchName, reader.ReadToEnd(), transactionRequirement, isolationLevel);
        }

        /// <summary>
        /// Adds a database script as a single batch.
        /// </summary>
        /// <param name="batchName"> The name of the batch. </param>
        /// <param name="stream"> The stream from which the script is read. </param>
        /// <param name="encoding"> The optional encoding to read the script. Default value is null, using <see cref="Encoding.UTF8"/>. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="batchName" /> or <paramref name="stream"/> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="batchName" /> is an empty string or <paramref name="stream"/> is not readable.</exception>
        public void AddScriptFromStream(string batchName, Stream stream, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
        {
            if (batchName == null)
            {
                throw new ArgumentNullException(nameof(batchName));
            }

            if (string.IsNullOrWhiteSpace(batchName))
            {
                throw new ArgumentException("The string argument is empty.", nameof(batchName));
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
                this.AddScriptFromReader(batchName, sr, transactionRequirement, isolationLevel);
            }
        }

        /// <summary>
        /// Adds a database script as a single batch.
        /// </summary>
        /// <param name="batchName"> The name of the batch. </param>
        /// <param name="file"> The file from which the script is read. </param>
        /// <param name="encoding"> The optional encoding to read the script. Default value is null, using <see cref="Encoding.UTF8"/>. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="batchName" /> or <paramref name="file"/> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="batchName" /> is an empty string. </exception>
        public void AddScriptFromFile(string batchName, string file, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
        {
            if (batchName == null)
            {
                throw new ArgumentNullException(nameof(batchName));
            }

            if (string.IsNullOrWhiteSpace(batchName))
            {
                throw new ArgumentException("The string argument is empty.", nameof(batchName));
            }

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this.AddScriptFromStream(batchName, fs, encoding, transactionRequirement, isolationLevel);
            }
        }

        /// <summary>
        /// Adds a database script as a single batch.
        /// </summary>
        /// <param name="batchName"> The name of the batch. </param>
        /// <param name="assembly"> The assembly which contains the resource named by <paramref name="name"/>. </param>
        /// <param name="name"> The embedded assembly resource name of the script. </param>
        /// <param name="encoding"> The optional encoding to read the script. Default value is null, using <see cref="Encoding.UTF8"/>. </param>
        /// <param name="transactionRequirement"> The optional transaction requirement specification. Default values is <see cref="DbBatchTransactionRequirement.DontCare" />. </param>
        /// <param name="isolationLevel"> The optional isolation level requirement specification. Default value is null.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="batchName" />, <paramref name="assembly"/>, or <paramref name="name"/> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="batchName"/> or <paramref name="name"/> is an empty string.</exception>
        public void AddScriptFromAssemblyResource(string batchName, Assembly assembly, string name, Encoding encoding = null, DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare, IsolationLevel? isolationLevel = null)
        {
            if (batchName == null)
            {
                throw new ArgumentNullException(nameof(batchName));
            }

            if (string.IsNullOrWhiteSpace(batchName))
            {
                throw new ArgumentException("The string argument is empty.", nameof(batchName));
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
                this.AddScriptFromStream(batchName, stream, encoding, transactionRequirement, isolationLevel);
            }
        }
    }
}

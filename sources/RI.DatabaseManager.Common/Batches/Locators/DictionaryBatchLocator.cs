using System;
using System.Collections.Generic;
using System.Data.Common;

using RI.Abstractions.Logging;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which uses a dictionary to hold scripts and/or code.
    /// </summary>
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
    ///         Currently, the following options are supported which are extracted from the scripts (see <see cref="DbBatchLocatorBase.OptionsFormat" /> for more details):
    ///     </para>
    ///     <list type="bullet">
    ///         <item> <c> TransactionRequirement </c> [optional] One of the <see cref="DbBatchTransactionRequirement" /> values (as string), e.g. <c> /* DBMANAGER:TransactionRequirement=Disallowed */ </c> </item>
    ///     </list>
    ///     <note type="note">
    ///         If the <c> TransactionRequirement </c> option is not specified, the value from <see cref="TransactionRequirements" /> is used. If that is also not available, <see cref="DbBatchTransactionRequirement.DontCare" /> is used.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class DictionaryBatchLocator : DbBatchLocatorBase
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DictionaryBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public DictionaryBatchLocator (ILogger logger) : base(logger)
        {
            this.Scripts = new Dictionary<string, string>(this.DefaultNameComparer);
            this.Callbacks = new Dictionary<string, Func<DbConnection, DbTransaction, object>>(this.DefaultNameComparer);
            this.TransactionRequirements = new Dictionary<string, DbBatchTransactionRequirement>(this.DefaultNameComparer);
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the dictionary with callbacks as batches.
        /// </summary>
        /// <value>
        ///     The dictionary with callbacks.
        /// </value>
        public Dictionary<string, Func<DbConnection, DbTransaction, object>> Callbacks { get; }

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

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override bool FillBatch (IDbBatch batch, string name, string commandSeparator)
        {
            DbBatchTransactionRequirement transactionRequirement = this.TransactionRequirements.ContainsKey(name) ? this.TransactionRequirements[name] : DbBatchTransactionRequirement.DontCare;

            bool found = false;

            if (this.Callbacks.ContainsKey(name))
            {
                Func<DbConnection, DbTransaction, object> callback = this.Callbacks[name];
                batch.AddCode(callback, transactionRequirement);
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

                        if ((commandSpecifiedTransactionRequirement != DbBatchTransactionRequirement.DontCare) && (transactionRequirement != DbBatchTransactionRequirement.DontCare))
                        {
                            if (commandSpecifiedTransactionRequirement != transactionRequirement)
                            {
                                throw new InvalidOperationException("Conflicting transaction requirements.");
                            }
                        }

                        if (commandSpecifiedTransactionRequirement != DbBatchTransactionRequirement.DontCare)
                        {
                            transactionRequirement = commandSpecifiedTransactionRequirement;
                        }

                        batch.AddScript(command, transactionRequirement);
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

        #endregion
    }
}

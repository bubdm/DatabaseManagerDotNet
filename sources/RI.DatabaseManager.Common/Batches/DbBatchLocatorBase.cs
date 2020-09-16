using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using RI.Abstractions.Logging;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbBatchLocator" />.
    /// </summary>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database batch locator implementations use this base class as it already implements already some boilerplate code.
    ///     </note>
    /// <note type="important">
    /// See <see cref="OptionsFormat"/> for details about how additional script options can be extracted from scripts.
    /// </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DbBatchLocatorBase : IDbBatchLocator
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbBatchLocatorBase"/>.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        protected DbBatchLocatorBase (ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Logger = logger;

            this.OptionsFormat = @"(/\*\s*DBMANAGER:)(?<key>.+?)(=)(?<value>.+?)(\s*\*/)";
        }

        /// <summary>
        /// Gets the used logger.
        /// </summary>
        /// <value>
        /// The used logger.
        /// </value>
        protected ILogger Logger { get; }

        /// <inheritdoc />
        IDbBatch IDbBatchLocator.GetBatch(string name, string commandSeparator)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The string argument is empty.", nameof(name));
            }

            if (commandSeparator != null)
            {
                if (string.IsNullOrWhiteSpace(commandSeparator))
                {
                    throw new ArgumentException("The string argument is empty.", nameof(commandSeparator));
                }
            }

            commandSeparator ??= this.DefaultCommandSeparator;
            commandSeparator = string.IsNullOrWhiteSpace(commandSeparator) ? null : commandSeparator;

            DbBatch batch = new DbBatch();

            if (!this.FillBatch(batch, name, commandSeparator))
            {
                return null;
            }

            return batch;
        }

        /// <inheritdoc />
        ISet<string> IDbBatchLocator.GetNames()
        {
            return new HashSet<string>(this.GetNames()?.Where(x => x != null) ?? new string[0], this.DefaultNameComparer);
        }

        /// <summary>
        /// Gets the names of all available batches this batch locator can retrieve.
        /// </summary>
        /// <returns>
        /// The sequence with the names of all available batches.
        ///     Details about failures should be written to logs.
        /// </returns>
        protected abstract IEnumerable<string> GetNames ();

        /// <summary>
        /// Retrieves the batch with a specified name.
        /// </summary>
        /// <param name="batch">The prepared batch instance which is to be filled with code or scripts associated with the specified name.</param>
        /// <param name="name"> The name of the batch. </param>
        /// <param name="commandSeparator"> The string which is used as the separator to separate commands within the batch or null if neither a specific command separator nor a value through <see cref="DefaultCommandSeparator"/> is provided. </param>
        /// <returns>
        /// true if the batch could be successfully retrieved, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        protected abstract bool FillBatch (IDbBatch batch, string name, string commandSeparator);

        /// <summary>
        /// Gets the default command separator string used by this batch locator.
        /// </summary>
        /// <value>
        /// The default command separator string used by this batch locator or null if this batch locator does not support command separators.
        /// </value>
        /// <remarks>
        ///<note type="implement">
        /// The default implementation returns <c>{Environment.NewLine}GO{Environment.NewLine}</c>.
        /// </note>
        /// </remarks>
        protected virtual string DefaultCommandSeparator => $"{Environment.NewLine}GO{Environment.NewLine}";

        /// <summary>
        /// Gets the string comparer used for comparing batch names.
        /// </summary>
        /// <value>
        /// The string comparer used for comparing batch names.
        /// </value>
        /// <remarks>
        ///<note type="implement">
        /// The default implementation returns <see cref="StringComparer.InvariantCultureIgnoreCase"/>.
        /// </note>
        ///<note type="implement">
        ///         <see cref="DefaultNameComparer" /> should never return null.
        /// </note>
        /// </remarks>
        protected virtual StringComparer DefaultNameComparer => StringComparer.InvariantCultureIgnoreCase;

        /// <summary>
        /// Splits a script into individual commands using a specified command separator.
        /// </summary>
        /// <param name="script">The script to split into individual commands.</param>
        /// <param name="commandSeparator"> The string which is used as the separator to separate commands within the batch. </param>
        /// <returns>
        /// A list of commands from the script.
        /// If the script is null or empty, an empty list will be returned.
        /// If the command separator is null or empty, a list with a single item, the original script, will be returned.
        /// </returns>
        protected virtual List<string> SeparateScriptCommands (string script, string commandSeparator)
        {
            script = script?.Replace("\r", "")
                           .Replace("\n", Environment.NewLine);

            commandSeparator = commandSeparator?.Replace("\r", "")
                                               .Replace("\n", Environment.NewLine);

            if (string.IsNullOrWhiteSpace(script))
            {
                return new List<string>();
            }

            if (string.IsNullOrWhiteSpace(commandSeparator))
            {
                List<string> singleCommand = new List<string>();
                singleCommand.Add(script);
                return singleCommand;
            }

            List<string> pieces = script.Split(new[] { commandSeparator }, StringSplitOptions.None).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return pieces;
        }

        private string _optionsFormat;

        /// <summary>
        ///     Gets or sets the used options format as a regular expression (RegEx) used to extract additional script options from a script.
        /// </summary>
        /// <value>
        ///     The used options format as a regular expression (RegEx) used to extract additional script options from a script.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value is <c>(/\*\s*DBMANAGER:)(?&lt;key&gt;.+?)(=)(?&lt;value&gt;.+?)(\s*\*/)</c>.
        ///     </note>
        /// <note type="important">
        /// The options format must be a regular expression which provides two named captures: <c>key</c> and <c>value</c>. Those captures are used to extract key/value pairs from the script itself (e.g. SQL code).
        /// Using the default value, <c>/* DBMANAGER:MyValue=123</c> would provide a key <c>MyValue</c> with the value <c>123</c>.
        /// </note>
        /// </remarks>
        /// <para>
        /// Each database batch locator implementation defines which options to support.
        /// <see cref="OptionsFormat"/> and <see cref="GetOptionsFromCommand"/>, <see cref="GetTransactionRequirementFromCommandOptions(string)"/>, and <see cref="GetTransactionRequirementFromCommandOptions(IDictionary{string,string})"/> support as boilerplate implementation which must be explicitly used by database batch locator implementations.
        /// </para>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is an empty string.</exception>
        public string OptionsFormat
        {
            get => this._optionsFormat;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("The string argument is empty.", nameof(value));
                }

                this._optionsFormat = value;
            }
        }

        /// <summary>
        /// Gets the options defined in a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// The dictionary which contains the options as key/value pairs.
        /// If the command is null or empty, an empty dictionary will be returned.
        /// </returns>
        /// <remarks>
        ///<note type="note">
        /// See <see cref="OptionsFormat"/> for more information.
        /// </note>
        /// </remarks>
        protected virtual Dictionary<string, string> GetOptionsFromCommand(string command)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>(this.DefaultNameComparer);

            if (string.IsNullOrWhiteSpace(command))
            {
                return keyValuePairs;
            }

            MatchCollection matches = Regex.Matches(command, this.OptionsFormat, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                string key = match.Groups["key"]
                                  ?.Value;

                string value = match.Groups["value"]
                                    ?.Value;

                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                keyValuePairs.Add(key, value);
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Gets the transaction requirement of a command based on its options.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// One of the <see cref="DbBatchTransactionRequirement"/> values if the option <c>TransactionRequirement</c> was specified and has a valid value, <see cref="DbBatchTransactionRequirement.DontCare"/> otherwise.
        /// If command is null or empty, <see cref="DbBatchTransactionRequirement.DontCare"/> is returned.
        /// </returns>
        protected virtual DbBatchTransactionRequirement GetTransactionRequirementFromCommandOptions (string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return DbBatchTransactionRequirement.DontCare;
            }

            Dictionary<string, string> options = this.GetOptionsFromCommand(command);

            return this.GetTransactionRequirementFromCommandOptions(options);
        }

        /// <summary>
        /// Gets the transaction requirement of a command based on its options.
        /// </summary>
        /// <param name="options"> The already extracted command options (e.g. by <see cref="GetOptionsFromCommand"/>).</param>
        /// <returns>
        /// One of the <see cref="DbBatchTransactionRequirement"/> values if the option <c>TransactionRequirement</c> was specified and has a valid value, <see cref="DbBatchTransactionRequirement.DontCare"/> otherwise.
        /// If command is null or empty, <see cref="DbBatchTransactionRequirement.DontCare"/> is returned.
        /// </returns>
        protected virtual DbBatchTransactionRequirement GetTransactionRequirementFromCommandOptions (IDictionary<string, string> options)
        {
            if (options == null)
            {
                return DbBatchTransactionRequirement.DontCare;
            }
            
            const string key = "TransactionRequirement";

            if (options.ContainsKey(key))
            {
                string value = options[key];

                if (Enum.TryParse(value, true, out DbBatchTransactionRequirement tr))
                {
                    return tr;
                }

                this.Logger.LogWarning(this.GetType().Name, null, $"Invalid value for TransactionRequirement script option: {value}");
            }

            return DbBatchTransactionRequirement.DontCare;
        }
    }
}

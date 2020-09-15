using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using RI.Abstractions.Logging;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which searches assemblies for script resources.
    /// </summary>
    /// <note type="important">
    /// See <see cref="NameFormat"/> for details about how script resources are searched in assemblies.
    /// </note>
    /// <note type="important">
    /// See <see cref="OptionsFormat"/> for details about how additional script options are extracted from the scripts.
    /// </note>
    /// <para>
    ///     <see cref="Assemblies" /> is the list of assemblies used to lookup scripts.
    /// </para>
    /// <para>
    ///     <see cref="Encoding" /> is the used encoding for reading the scripts as strings from the assembly resources.
    /// </para>
    /// <threadsafety static="false" instance="false" />
    public sealed class AssemblyScriptBatchLocator : DbBatchLocatorBase
    {
        private Encoding _encoding;

        private string _nameFormat;

        private string _optionsFormat;




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyScriptBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyScriptBatchLocator (ILogger logger)
            : this(logger, (IEnumerable<Assembly>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyScriptBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="assemblies"> The sequence of assemblies. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="assemblies" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyScriptBatchLocator (ILogger logger, IEnumerable<Assembly> assemblies) : base(logger)
        {
            this.NameFormat = @"(?<name>^.+?)([\.]+[^.]*)(sql$)";
            this.OptionsFormat = @"(/\*\s*DBMANAGER:)(?<key>.+?)(=)(?<value>.+?)(\s*\*/)";
            this.Encoding = Encoding.UTF8;
            this.Assemblies = new List<Assembly>();

            if (assemblies != null)
            {
                this.Assemblies.AddRange(assemblies);
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyScriptBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="assemblies"> The array of assemblies. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyScriptBatchLocator (ILogger logger, params Assembly[] assemblies)
            : this(logger, (IEnumerable<Assembly>)assemblies) { }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the list of used assemblies to lookup scripts.
        /// </summary>
        /// <value>
        ///     The list of used assemblies to lookup scripts.
        /// </value>
        public List<Assembly> Assemblies { get; }

        /// <summary>
        ///     Gets or sets the used encoding for reading the scripts as strings from the assembly resources.
        /// </summary>
        /// <value>
        ///     The used encoding for reading the scripts as strings from the assembly resources.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value is <see cref="System.Text.Encoding.UTF8" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public Encoding Encoding
        {
            get => this._encoding;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._encoding = value;
            }
        }

        /// <summary>
        ///     Gets or sets the used name format as a regular expression (RegEx) used to lookup scripts using the assembly resource names.
        /// </summary>
        /// <value>
        ///     The used name format as a regular expression (RegEx) used to lookup scripts using the assembly resource names.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value is <c>(?&lt;name&gt;^.+?)([\.]+[^.]*)(sql$)</c>.
        ///     </note>
        /// <note type="important">
        /// The name format must be a regular expression which provides a named capture <c>name</c>. That capture is used to extract the name of the batch from the resources manifest name.
        /// The resource manifest name is typically <c>Folder1.Folder2.Filename.Extension</c>, e.g. <c>MyApp.Scripts.Cleanup.sql</c> in an assembly named MyApp with a folder named Scripts which contains an embedded assembly resource named Cleanup.sql.
        /// Using the default value, <c>MyApp.Scripts.Cleanup</c> would be extracted from <c>MyApp.Scripts.Cleanup.sql</c>.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public string NameFormat
        {
            get => this._nameFormat;
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

                this._nameFormat = value;
            }
        }

        /// <summary>
        ///     Gets or sets the used options format as a regular expression (RegEx) used to extract additional script options from the script itself.
        /// </summary>
        /// <value>
        ///     The used options format as a regular expression (RegEx) used to extract additional script options from the script itself.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value is <c>(/\*\s*DBMANAGER:)(?&lt;key&gt;.+?)(=)(?&lt;value&gt;.+?)(\s*\*/)</c>.
        ///     </note>
        /// <note type="important">
        /// The options format must be a regular expression which provides two named captures: <c>key</c> and <c>value</c>. Those captures are used to extract key/value pairs from the script itself.
        /// Using the default value, <c>/* DBMANAGER:MyValue=123</c> would provide a key <c>MyValue</c> with the value <c>123</c>.
        /// </note>
        /// <para>
        /// Currently, the following options are supported:
        /// </para>
        /// <list type="bullet">
        ///   <item><c>TransactionRequirement</c> [optional] One of the <see cref="DbBatchTransactionRequirement"/> values (as string), e.g. <c>/* DBMANAGER:TransactionRequirement=Disallowed */</c></item>
        /// </list>
        /// <note type="note">
        /// If the <c>TransactionRequirement</c> option is not specified, <see cref="DbBatchTransactionRequirement.DontCare"/> is used.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
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

        #endregion




        /// <inheritdoc />
        protected override IEnumerable<string> GetNames ()
        {
            List<string> names = new List<string>();

            foreach (Assembly assembly in this.Assemblies)
            {
                string[] resources = assembly.GetManifestResourceNames();

                foreach (string resource in resources)
                {
                    string name = this.GetBatchNameFromResourceName(resource);

                    if (name != null)
                    {
                        names.Add(name);
                    }
                }
            }

            return names;
        }

        /// <inheritdoc />
        protected override bool FillBatch (IDbBatch batch, string name, string commandSeparator)
        {
            bool found = false;

            foreach (Assembly assembly in this.Assemblies)
            {
                string[] resources = assembly.GetManifestResourceNames();

                foreach (string resource in resources)
                {
                    string candidate = this.GetBatchNameFromResourceName(resource);

                    if (this.DefaultNameComparer.Equals(name, candidate))
                    {
                        string script;

                        using (Stream s = assembly.GetManifestResourceStream(resource))
                        {
                            if (s == null)
                            {
                                this.Logger.LogError(this.GetType().Name, null, $"Failed to read batch \"{name}\" as resource \"{resource}\" from assembly: {assembly.FullName}");
                                continue;
                            }

                            using (StreamReader sr = new StreamReader(s, this.Encoding))
                            {
                                script = sr.ReadToEnd();
                            }
                        }

                        List<string> commands = this.SeparateScriptCommands(script, commandSeparator);

                        foreach (string command in commands)
                        {
                            if (!string.IsNullOrWhiteSpace(command))
                            {
                                DbBatchTransactionRequirement transactionRequirement = this.GetTransactionRequirementFromCommand(command);
                                batch.AddScript(command, transactionRequirement);
                            }
                        }

                        found = true;
                    }
                }
            }

            return found;
        }

        private string GetBatchNameFromResourceName (string resource)
        {
            Match match = Regex.Match(resource, this.NameFormat, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return null;
            }

            string name = match.Groups["name"]
                               ?.Value;

            return string.IsNullOrWhiteSpace(name) ? null : name;
        }

        private Dictionary<string, string> GetScriptOptionsFromCommand (string command)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>(this.DefaultNameComparer);
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

        private DbBatchTransactionRequirement GetTransactionRequirementFromCommand (string command)
        {
            Dictionary<string, string> keyValuePairs = this.GetScriptOptionsFromCommand(command);

            const string key = "TransactionRequirement";

            if (keyValuePairs.ContainsKey(key))
            {
                string value = keyValuePairs[key];

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

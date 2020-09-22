using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which searches assemblies for script resources.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         See <see cref="NameFormat" /> for details about how script resources are searched in assemblies.
    ///     </note>
    ///     <para>
    ///         <see cref="Assemblies" /> is the list of assemblies used to lookup scripts.
    ///     </para>
    ///     <para>
    ///         <see cref="Encoding" /> is the used encoding for reading the scripts as strings from the assembly resources.
    ///     </para>
    ///     <para>
    ///         Currently, the following options are supported which are extracted from the scripts (see <see cref="DbBatchLocatorBase.OptionsFormat" /> for more details):
    ///     </para>
    ///     <list type="bullet">
    ///         <item> <c> TransactionRequirement </c> [optional] One of the <see cref="DbBatchTransactionRequirement" /> values (as string), e.g. <c> /* DBMANAGER:TransactionRequirement=Disallowed */ </c> </item>
    ///     </list>
    ///     <note type="note">
    ///         If the <c> TransactionRequirement </c> option is not specified, <see cref="DbBatchTransactionRequirement.DontCare" /> is used.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class AssemblyScriptBatchLocator : DbBatchLocatorBase
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyScriptBatchLocator" />.
        /// </summary>
        public AssemblyScriptBatchLocator ()
            : this((IEnumerable<Assembly>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyScriptBatchLocator" />.
        /// </summary>
        /// <param name="assemblies"> The sequence of assemblies. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="assemblies" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        public AssemblyScriptBatchLocator (IEnumerable<Assembly> assemblies)
        {
            this.NameFormat = @"(?<name>^.+?)([\.]+[^.]*)(sql$)";
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
        /// <param name="assemblies"> The array of assemblies. </param>
        public AssemblyScriptBatchLocator (params Assembly[] assemblies)
            : this((IEnumerable<Assembly>)assemblies) { }

        #endregion




        #region Instance Fields

        private Encoding _encoding;

        private string _nameFormat;

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
        /// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
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
        ///         The default value is <c> (?&lt;name&gt;^.+?)([\.]+[^.]*)(sql$) </c>.
        ///     </note>
        ///     <note type="important">
        ///         The name format must be a regular expression which provides a named capture <c> name </c>. That capture is used to extract the name of the batch from the resources manifest name.
        ///         The resource manifest name is typically <c> Folder1.Folder2.Filename.Extension </c>, e.g. <c> MyApp.Scripts.Cleanup.sql </c> in an assembly named MyApp with a folder named Scripts which contains an embedded assembly resource named Cleanup.sql.
        ///         Using the default value, <c> MyApp.Scripts.Cleanup </c> would be extracted from <c> MyApp.Scripts.Cleanup.sql </c>.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
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

        #endregion




        #region Instance Methods

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

        #endregion




        #region Overrides

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
                                DbBatchTransactionRequirement transactionRequirement = this.GetTransactionRequirementFromCommandOptions(command);
                                batch.AddScript(command, transactionRequirement);
                            }
                        }

                        found = true;
                    }
                }
            }

            return found;
        }


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
        public override bool SupportsScripts => true;

        /// <inheritdoc />
        public override bool SupportsCallbacks => false;

        #endregion
    }
}

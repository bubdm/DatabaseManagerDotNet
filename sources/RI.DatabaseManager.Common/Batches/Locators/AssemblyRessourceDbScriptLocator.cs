using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Scripts
{
    /// <summary>
    ///     Script locator implementation which gets scripts from assembly resources.
    /// </summary>
    /// <para>
    ///     <see cref="Assemblies" /> is the list of assemblies used to lookup scripts.
    /// </para>
    /// <para>
    ///     <see cref="Encoding" /> is the used encoding for reading the scripts as strings from the assembly resources.
    /// </para>
    /// <threadsafety static="false" instance="false" />
    public sealed class AssemblyRessourceDbScriptLocator : DbScriptLocatorBase
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyRessourceDbScriptLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyRessourceDbScriptLocator (ILogger logger)
            : this(logger, (IEnumerable<Assembly>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyRessourceDbScriptLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="assemblies"> The sequence of assemblies. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="assemblies" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyRessourceDbScriptLocator (ILogger logger, IEnumerable<Assembly> assemblies) : base(logger)
        {
            this.Encoding = Encoding.UTF8;
            this.Assemblies = new List<Assembly>();

            if (assemblies != null)
            {
                this.Assemblies.AddRange(assemblies);
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyRessourceDbScriptLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="assemblies"> The array of assemblies. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyRessourceDbScriptLocator (ILogger logger, params Assembly[] assemblies)
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
        ///     Gets the used encoding for reading the scripts as strings from the assembly resources.
        /// </summary>
        /// <value>
        ///     The used encoding for reading the scripts as strings from the assembly resources.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         If <see cref="Encoding" /> is null, <see cref="System.Text.Encoding.UTF8" /> is used.
        ///     </para>
        ///     <note type="implement">
        ///         The default value is <see cref="System.Text.Encoding.UTF8" />.
        ///     </note>
        /// </remarks>
        public Encoding Encoding { get; set; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override string LocateAndReadScript (IDbManager manager, string name)
        {
            //TODO: #9: Use more advanced resource lookup (+ document in class remarks)
            foreach (Assembly assembly in this.Assemblies)
            {
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    if (stream == null)
                    {
                        continue;
                    }

                    using (StreamReader sr = new StreamReader(stream, this.Encoding ?? Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }

            return null;
        }

        #endregion
    }
}

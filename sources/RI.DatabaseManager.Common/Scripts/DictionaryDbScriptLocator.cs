using System;
using System.Collections.Generic;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Scripts
{
    /// <summary>
    ///     Script locator implementation using a simple dictionary.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Scripts"/> is the dictionary where the keys are used as script names and the values are the actual script contents.
    /// </para>
    ///     <para>
    ///         <see cref="StringComparer.InvariantCultureIgnoreCase" /> is used to compare the keys / script names in <see cref="Scripts"/>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class DictionaryDbScriptLocator : DbScriptLocatorBase
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DictionaryDbScriptLocator" />.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public DictionaryDbScriptLocator (ILogger logger) : this(logger, null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DictionaryDbScriptLocator" />.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <param name="scripts">The used key/value pairs.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public DictionaryDbScriptLocator(ILogger logger, IDictionary<string, string> scripts) : base(logger)
        {
            this.Scripts = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            if (scripts != null)
            {
                foreach (KeyValuePair<string, string> script in scripts)
                {
                    this.Scripts.Add(script.Key, script.Value);
                }
            }
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the dictionary with the used key/value pairs or name/script pairs respectively.
        /// </summary>
        /// <value>
        ///     The dictionary with the used key/value pairs or name/script pairs respectively.
        /// </value>
        public Dictionary<string, string> Scripts { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override string LocateAndReadScript (IDbManager manager, string name)
        {
            if (!this.Scripts.ContainsKey(name))
            {
                return null;
            }

            return this.Scripts[name];
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;




namespace RI.Framework.Data.Database.Scripts
{
    /// <summary>
    ///     Implements a database script locator which uses name-script-pairs.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="StringComparerEx.InvariantCultureIgnoreCase" /> is used to compare names of the name-script-pairs.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class DictionaryScriptLocator : DatabaseScriptLocator
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DictionaryScriptLocator" />.
        /// </summary>
        public DictionaryScriptLocator ()
            : this(null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DictionaryScriptLocator" />.
        /// </summary>
        /// <param name="scripts"> A dictionary with predefined name-script-pairs. </param>
        public DictionaryScriptLocator (IDictionary<string, string> scripts)
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
        ///     Gets the dictionary with the used name-script-pairs.
        /// </summary>
        /// <value>
        ///     The dictionary with the used name-script-pairs.
        /// </value>
        public Dictionary<string, string> Scripts { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override string LocateAndReadScript (IDatabaseManager manager, string name)
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

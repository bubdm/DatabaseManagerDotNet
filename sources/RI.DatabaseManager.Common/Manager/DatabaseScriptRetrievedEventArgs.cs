using System;
using System.Collections.Generic;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Event arguments for the <see cref="IDatabaseManager.ScriptRetrieved" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public sealed class DatabaseScriptRetrievedEventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseScriptRetrievedEventArgs" />.
        /// </summary>
        /// <param name="name"> The name of the retrieved script. </param>
        /// <param name="preprocess"> Specifies whether the script is preprocessed. </param>
        /// <param name="scriptBatches"> The list of retrieved individual batches. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="scriptBatches" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public DatabaseScriptRetrievedEventArgs (string name, bool preprocess, List<string> scriptBatches)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (scriptBatches == null)
            {
                throw new ArgumentNullException(nameof(scriptBatches));
            }

            this.Name = name;
            this.Preprocess = preprocess;
            this.ScriptBatches = scriptBatches;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the name of the retrieved script,
        /// </summary>
        /// <value>
        ///     The name of the retrieved script.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Gets whether the script is preprocessed.
        /// </summary>
        /// <value>
        ///     true if the script is preprocessed, false otherwise.
        /// </value>
        public bool Preprocess { get; }


        /// <summary>
        ///     Gets the list of retrieved individual batches.
        /// </summary>
        /// <value>
        ///     The list of retrieved individual batches.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         <see cref="ScriptBatches" /> can be modified.
        ///         Modifications are returned by <see cref="IDatabaseManager.GetScriptBatch" />.
        ///     </para>
        /// </remarks>
        public List<string> ScriptBatches { get; }

        #endregion
    }
}

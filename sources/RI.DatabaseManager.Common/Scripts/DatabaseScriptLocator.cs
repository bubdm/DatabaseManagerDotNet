using System;
using System.Collections.Generic;
using System.Linq;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Scripts
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDatabaseScriptLocator"/>.
    /// </summary>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that script locator implementations use this base class as it already implements most of the database-independent logic defined by <see cref="IDatabaseScriptLocator"/>.
    ///     </note>
    ///     <note type="implement">
    ///         This boilerplate implementation does the following preprocessing:
    ///         Trim all batches, remove empty batches, replace placeholder values.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DatabaseScriptLocator : IDatabaseScriptLocator
    {
        #region Constants

        /// <summary>
        ///     Gets the used string comparer used to compare placeholder names for equality.
        /// </summary>
        /// <value>
        ///     The used string comparer used to compare placeholder names for equality.
        /// </value>
        /// <remarks>
        ///<para>
        ///<see cref="StringComparer.OrdinalIgnoreCase"/> is used.
        /// </para>
        /// </remarks>
        public static readonly StringComparer PlaceholderNameComparer = StringComparer.OrdinalIgnoreCase;

        #endregion




        #region Static Methods

        /// <summary>
        ///     Implements default splitting of a script into individual batches.
        /// </summary>
        /// <param name="script"> The script to split into individual batches. </param>
        /// <param name="separator"> The used batch separator. </param>
        /// <returns>
        ///     The list of batches.
        ///     Empty batches are removed and an empty list is returned if no non-empty batches are remaining.
        ///     <paramref name="script"/> is returned as the single item if <paramref name="separator"/> is null, an empty string, or only consists of whitespace.
        /// </returns>
        public static List<string> SplitBatchesDefault(string script, string separator)
        {
            script = script?.Replace("\r", "")
                           .Replace("\n", Environment.NewLine);
            separator = separator?.Replace("\r", "")
                                 .Replace("\n", Environment.NewLine);

            if (string.IsNullOrWhiteSpace(script))
            {
                return new List<string>();
            }

            if (string.IsNullOrWhiteSpace(separator))
            {
                List<string> singleBatch = new List<string>();
                singleBatch.Add(script);
                return singleBatch;
            }

            string[] pieces = script.Split(new []{ separator }, StringSplitOptions.None);
            List<string> batches = new List<string>(pieces);
            batches.RemoveAll(string.IsNullOrWhiteSpace);
            return batches;
        }

        #endregion

        private ILogger Logger { get; }




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseScriptLocator" />.
        /// </summary>
        /// <param name="logger">The used logger.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        protected DatabaseScriptLocator (ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Logger = logger;

            this.DefaultBatchSeparator = "GO";
            this.Placeholders = new Dictionary<string, Func<string, string>>(DatabaseScriptLocator.PlaceholderNameComparer);
        }

        #endregion




        #region Instance Fields

        private string _defaultBatchSeparator;

        #endregion




        #region Instance Properties/Indexer

        private Dictionary<string, Func<string, string>> Placeholders { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Sets the resolver for a specified placeholder name.
        /// </summary>
        /// <param name="name"> The name of the placeholder. </param>
        /// <param name="resolver"> The resolver which is used to resolve the value for the placeholder or null to remove the placeholder. </param>
        /// <remarks>
        ///     <para>
        ///         If the placeholder resolver is already set, the existing resolver is replaced by <paramref name="resolver" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public void SetPlaceholderResolver (string name, Func<string, string> resolver)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Argument is an empty string.", nameof(name));
            }

            this.Placeholders.Remove(name);

            if (resolver == null)
            {
                return;
            }

            this.Placeholders.Add(name, resolver);
        }

        #endregion




        #region Abstracts

        /// <summary>
        ///     Called to locate and read the script with a specified name into a string.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="name"> The name of the script. </param>
        /// <returns>
        ///     The script or null if the script could not be found.
        /// </returns>
        protected abstract string LocateAndReadScript (IDbManager manager, string name);

        #endregion




        #region Virtuals

        /// <summary>
        ///     Gets the value for a single placeholder.
        /// </summary>
        /// <param name="name"> The name of the placeholder. </param>
        /// <returns>
        ///     The value of the placeholder or null if the value is not found or the placeholder is not defined.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         The default implementation uses the resolvers set through <see cref="SetPlaceholderResolver" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public virtual string GetPlaceholderValue (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Argument is an empty string.", nameof(name));
            }

            if (!this.Placeholders.ContainsKey(name))
            {
                return null;
            }

            return this.Placeholders[name](name);
        }

        /// <summary>
        ///     Performs additional batch preprocessing, as required by the current <see cref="IDatabaseScriptLocator" /> implementation.
        /// </summary>
        /// <param name="batches"> The batches to preprocess. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The default implementation does nothing.
        ///     </note>
        /// </remarks>
        protected virtual void AdditionalPreprocessing (List<string> batches)
        {
        }

        /// <summary>
        ///     Replaces placeholders in a batch.
        /// </summary>
        /// <param name="batch"> The current batch in which the placeholders are to be replaced. </param>
        /// <returns>
        ///     The batch with its placeholders replaced or null if the placeholder replacing failed.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         <see cref="ReplacePlaceholders" /> is separately called for each individual batch.
        ///     </note>
        ///     <note type="implement">
        ///         The default implementation uses <see cref="GetPlaceholderValue" /> to retrieve the values for placeholders defined by <see cref="SetPlaceholderResolver"/> and then performs a simple search-and-replace on <paramref name="batch" />.
        ///     </note>
        /// </remarks>
        protected virtual string ReplacePlaceholders (string batch)
        {
            string processed = batch;

            foreach (string placeholder in this.Placeholders.Keys)
            {
                string value = this.GetPlaceholderValue(placeholder);
                if (value != null)
                {
                    processed = processed.Replace(placeholder, value);
                }
            }

            return processed;
        }

        /// <summary>
        ///     Splits a script into individual batches.
        /// </summary>
        /// <param name="script"> The script to split into individual batches. </param>
        /// <param name="separator"> The used batch separator. </param>
        /// <returns>
        ///     The list of batches or null if the splitting failed.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         <see cref="SplitBatches(string,string)" /> is not called if the used batch separator string is null.
        ///     </note>
        ///     <note type="implement">
        ///         The default implementation uses <see cref="SplitBatchesDefault(string,string)" />.
        ///     </note>
        /// </remarks>
        protected virtual List<string> SplitBatches (string script, string separator)
        {
            return DatabaseScriptLocator.SplitBatchesDefault(script, separator);
        }

        #endregion




        #region Interface: IDatabaseScriptLocator

        /// <summary>
        /// Writes a log message for this script locator.
        /// </summary>
        /// <param name="level"> The log level of the log message. </param>
        /// <param name="format"> Log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc. which are expanded by <paramref name="args" />). </param>
        /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
        protected void Log(LogLevel level, string format, params object[] args) => this.Logger.Log(level, this.ToString(), null, format, args);

        /// <inheritdoc />
        public string DefaultBatchSeparator
        {
            get
            {
                return this._defaultBatchSeparator;
            }
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("Argument is an empty string.", nameof(value));
                    }
                }

                this._defaultBatchSeparator = value;
            }
        }


        /// <inheritdoc />
        public List<string> GetScriptBatches (IDbManager manager, string name, string batchSeparator, bool preprocess)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Argument is an empty string.", nameof(name));
            }

            if (batchSeparator != null)
            {
                if (string.IsNullOrWhiteSpace(batchSeparator))
                {
                    throw new ArgumentException("Argument is an empty string.", nameof(batchSeparator));
                }
            }

            string script = this.LocateAndReadScript(manager, name);

            if (script == null)
            {
                this.Log(LogLevel.Warning, "Script not found: {0}" + name);
                return null;
            }

            batchSeparator ??= this.DefaultBatchSeparator;

            List<string> batches;

            if (batchSeparator == null)
            {
                batches = new List<string>();
                batches.Add(script);
            }
            else
            {
                batches = this.SplitBatches(script, batchSeparator);
                if (batches == null)
                {
                    this.Log(LogLevel.Warning, "Script is invalid (splitting batches failed): {0}" + name);
                    return null;
                }
            }

            if (preprocess)
            {
                batches = new List<string>(batches.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)));

                for (int i1 = 0; i1 < batches.Count; i1++)
                {
                    string replaced = this.ReplacePlaceholders(batches[i1]);
                    if (replaced == null)
                    {
                        this.Log(LogLevel.Warning, "Script is invalid (replacing placeholders failed): {0}" + name);
                        return null;
                    }
                    batches[i1] = replaced;
                }

                this.AdditionalPreprocessing(batches);

                batches = new List<string>(batches.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            return batches;
        }

        #endregion
    }
}

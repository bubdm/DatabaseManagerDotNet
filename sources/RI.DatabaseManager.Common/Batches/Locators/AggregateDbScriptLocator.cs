using System;
using System.Collections;
using System.Collections.Generic;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Scripts
{
    /// <summary>
    ///     Script locator implementation which combines multiple script locators.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         If <see cref="IDbScriptLocator.DefaultBatchSeparator" /> of this script locator is not null, that value will always be used if the <c>batchSeparator</c> parameter of <see cref="GetScriptBatches"/> is null and therefore will override the values of the individual script locators.
    ///     </note>
    ///     <para>
    ///         <see cref="AggregateDbScriptLocator"/> is both a <see cref="IDbScriptLocator"/> and <see cref="IList{T}"/> implementation.
    /// It can dynamically combine multiple script locators and present it as one, doing lookup of scripts in the order of the list.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class AggregateDbScriptLocator : IDbScriptLocator, IList<IDbScriptLocator>, ICollection<IDbScriptLocator>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateDbScriptLocator" />.
        /// </summary>
        public AggregateDbScriptLocator ()
            : this((IEnumerable<IDbScriptLocator>)null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateDbScriptLocator" />.
        /// </summary>
        /// <param name="scriptLocators"> The sequence of script locators which are aggregated. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="scriptLocators" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        public AggregateDbScriptLocator (IEnumerable<IDbScriptLocator> scriptLocators)
        {
            this.DefaultBatchSeparator = null;
            this.ScriptLocators = new List<IDbScriptLocator>();

            if (scriptLocators != null)
            {
                foreach (IDbScriptLocator scriptLocator in scriptLocators)
                {
                    this.Add(scriptLocator);
                }
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateDbScriptLocator" />.
        /// </summary>
        /// <param name="scriptLocators"> The array of script locators which are aggregated. </param>
        public AggregateDbScriptLocator (params IDbScriptLocator[] scriptLocators)
            : this((IEnumerable<IDbScriptLocator>)scriptLocators)
        {
        }

        #endregion




        #region Instance Properties/Indexer

        private List<IDbScriptLocator> ScriptLocators { get; }

        #endregion




        #region Interface: ICollection<IDatabaseScriptLocator>

        /// <inheritdoc />
        public int Count => this.ScriptLocators.Count;

        /// <inheritdoc />
        bool ICollection<IDbScriptLocator>.IsReadOnly => false;

        /// <inheritdoc />
        public void Add (IDbScriptLocator item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.ScriptLocators.Add(item);
        }

        /// <inheritdoc />
        public void Clear () => this.ScriptLocators.Clear();

        /// <inheritdoc />
        public bool Contains (IDbScriptLocator item) => this.ScriptLocators.Contains(item);

        /// <inheritdoc />
        void ICollection<IDbScriptLocator>.CopyTo (IDbScriptLocator[] array, int arrayIndex) => this.ScriptLocators.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator () => this.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<IDbScriptLocator> GetEnumerator () => this.ScriptLocators.GetEnumerator();

        /// <inheritdoc />
        public bool Remove (IDbScriptLocator item) => this.ScriptLocators.Remove(item);

        #endregion




        #region Interface: IDatabaseScriptLocator

        private string _defaultBatchSeparator;

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

            foreach (IDbScriptLocator scriptLocator in this.ScriptLocators)
            {
                List<string> batches = scriptLocator.GetScriptBatches(manager, name, batchSeparator ?? this.DefaultBatchSeparator, preprocess);

                if (batches != null)
                {
                    return batches;
                }
            }

            return null;
        }

        #endregion




        /// <inheritdoc />
        public int IndexOf (IDbScriptLocator item) => this.ScriptLocators.IndexOf(item);

        /// <inheritdoc />
        public void Insert (int index, IDbScriptLocator item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.ScriptLocators.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt (int index) => this.ScriptLocators.RemoveAt(index);

        /// <inheritdoc />
        public IDbScriptLocator this [int index]
        {
            get => this.ScriptLocators[index];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.ScriptLocators[index] = value;
            }
        }
    }
}

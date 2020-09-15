using System;
using System.Collections;
using System.Collections.Generic;

using RI.Abstractions.Logging;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which combines multiple batch locators.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="AggregateBatchLocator" /> is both a <see cref="IDbBatchLocator" /> and <see cref="IList{IDbBatchLocator}" /> implementation.
    ///         It can dynamically combine multiple script locators and present it as one, doing lookup of scripts in the order of the list.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class AggregateBatchLocator : DbBatchLocatorBase, IList<IDbBatchLocator>, ICollection<IDbBatchLocator>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public AggregateBatchLocator(ILogger logger)
            : this(logger, (IEnumerable<IDbBatchLocator>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="batchLocators"> The sequence of batch locators which are aggregated. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="batchLocators" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public AggregateBatchLocator(ILogger logger, IEnumerable<IDbBatchLocator> batchLocators)
        : base(logger)
        {
            this.BatchLocators = new List<IDbBatchLocator>();

            if (batchLocators != null)
            {
                foreach (IDbBatchLocator batchLocator in batchLocators)
                {
                    this.Add(batchLocator);
                }
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="batchLocators"> The array of batch locators which are aggregated. </param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public AggregateBatchLocator(ILogger logger, params IDbBatchLocator[] batchLocators)
            : this(logger, (IEnumerable<IDbBatchLocator>)batchLocators) { }

        #endregion




        #region Instance Properties/Indexer

        private List<IDbBatchLocator> BatchLocators { get; }

        #endregion




        #region Interface: IDbScriptLocator

        /// <inheritdoc />
        protected override IEnumerable<string> GetNames ()
        {
            List<string> names = new List<string>();

            foreach (IDbBatchLocator batchLocator in this.BatchLocators)
            {
                ISet<string> currentNames = batchLocator.GetNames();

                if (currentNames != null)
                {
                    names.AddRange(currentNames);
                }
            }

            return names;
        }

        /// <inheritdoc />
        protected override bool FillBatch (IDbBatch batch, string name, string commandSeparator)
        {
            foreach (IDbBatchLocator batchLocator in this.BatchLocators)
            {
                IDbBatch currentBatch = batchLocator.GetBatch(name, commandSeparator);

                if (currentBatch != null)
                {
                    batch.Commands.AddRange(currentBatch.Commands);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        protected override string DefaultCommandSeparator => null;

        #endregion




        #region Interface: IList<IDbScriptLocator>

        /// <inheritdoc />
        public int Count => this.BatchLocators.Count;

        /// <inheritdoc />
        bool ICollection<IDbBatchLocator>.IsReadOnly => false;

        /// <inheritdoc />
        public IDbBatchLocator this [int index]
        {
            get => this.BatchLocators[index];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.BatchLocators[index] = value;
            }
        }

        /// <inheritdoc />
        public void Add (IDbBatchLocator item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.BatchLocators.Add(item);
        }

        /// <inheritdoc />
        public void Clear ()
        {
            this.BatchLocators.Clear();
        }

        /// <inheritdoc />
        public bool Contains (IDbBatchLocator item)
        {
            return this.BatchLocators.Contains(item);
        }

        /// <inheritdoc />
        void ICollection<IDbBatchLocator>.CopyTo (IDbBatchLocator[] array, int arrayIndex)
        {
            this.BatchLocators.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<IDbBatchLocator> GetEnumerator ()
        {
            return this.BatchLocators.GetEnumerator();
        }


        /// <inheritdoc />
        public int IndexOf (IDbBatchLocator item)
        {
            return this.BatchLocators.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert (int index, IDbBatchLocator item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.BatchLocators.Insert(index, item);
        }

        /// <inheritdoc />
        public bool Remove (IDbBatchLocator item)
        {
            return this.BatchLocators.Remove(item);
        }

        /// <inheritdoc />
        public void RemoveAt (int index)
        {
            this.BatchLocators.RemoveAt(index);
        }

        #endregion
    }
}

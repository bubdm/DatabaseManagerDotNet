using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which combines multiple batch locators.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <remarks>
    ///     <para>
    ///         <see cref="AggregateBatchLocator{TConnection,TTransaction}" /> is both a <see cref="IDbBatchLocator" /> and <see cref="IList{T}" /> implementation.
    ///         It can dynamically combine multiple script locators and present it as one, doing lookup of scripts in the order of the list.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class AggregateBatchLocator<TConnection, TTransaction> : IDbBatchLocator<TConnection, TTransaction>, IList<IDbBatchLocator<TConnection, TTransaction>>, ICollection<IDbBatchLocator<TConnection, TTransaction>>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateBatchLocator{TConnection,TTransaction}" />.
        /// </summary>
        public AggregateBatchLocator ()
            : this((IEnumerable<IDbBatchLocator<TConnection, TTransaction>>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateBatchLocator{TConnection,TTransaction}" />.
        /// </summary>
        /// <param name="batchLocators"> The sequence of batch locators which are aggregated. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="batchLocators" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        public AggregateBatchLocator (IEnumerable<IDbBatchLocator<TConnection, TTransaction>> batchLocators)
        {
            this.BatchLocators = new List<IDbBatchLocator<TConnection, TTransaction>>();

            if (batchLocators != null)
            {
                foreach (IDbBatchLocator<TConnection, TTransaction> batchLocator in batchLocators)
                {
                    this.Add(batchLocator);
                }
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateBatchLocator{TConnection,TTransaction}" />.
        /// </summary>
        /// <param name="batchLocators"> The array of batch locators which are aggregated. </param>
        public AggregateBatchLocator (params IDbBatchLocator<TConnection, TTransaction>[] batchLocators)
            : this((IEnumerable<IDbBatchLocator<TConnection, TTransaction>>)batchLocators) { }

        #endregion




        #region Instance Properties/Indexer

        private List<IDbBatchLocator<TConnection, TTransaction>> BatchLocators { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        IDbBatch<TConnection, TTransaction> IDbBatchLocator<TConnection, TTransaction>.GetBatch (string name, string commandSeparator, Func<IDbBatch<TConnection, TTransaction>> batchCreator)
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

            if (batchCreator == null)
            {
                throw new ArgumentNullException(nameof(batchCreator));
            }

            foreach (IDbBatchLocator<TConnection, TTransaction> batchLocator in this.BatchLocators)
            {
                IDbBatch<TConnection, TTransaction> currentBatch = batchLocator.GetBatch(name, commandSeparator, batchCreator);

                if (currentBatch != null)
                {
                    return currentBatch;
                }
            }

            return null;
        }

        /// <inheritdoc />
        IDbBatch IDbBatchLocator.GetBatch(string name, string commandSeparator, Func<IDbBatch> batchCreator) => ((IDbBatchLocator<TConnection, TTransaction>)this).GetBatch(name, commandSeparator, batchCreator);

        /// <inheritdoc />
        ISet<string> IDbBatchLocator.GetNames ()
        {
            ISet<string> names = null;

            foreach (IDbBatchLocator batchLocator in this.BatchLocators)
            {
                ISet<string> currentNames = batchLocator.GetNames();

                if (names == null)
                {
                    names = currentNames;
                }
                else
                {
                    foreach (string currentName in currentNames)
                    {
                        names.Add(currentName);
                    }
                }
            }

            return names;
        }

        #endregion




        #region Interface: IList<IDbBatchLocator<TConnection, TTransaction>>

        /// <inheritdoc />
        public int Count => this.BatchLocators.Count;

        /// <inheritdoc />
        bool ICollection<IDbBatchLocator<TConnection, TTransaction>>.IsReadOnly => false;

        /// <inheritdoc />
        public IDbBatchLocator<TConnection, TTransaction> this [int index]
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
        public void Add (IDbBatchLocator<TConnection, TTransaction> item)
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
        public bool Contains (IDbBatchLocator<TConnection, TTransaction> item)
        {
            return this.BatchLocators.Contains(item);
        }

        /// <inheritdoc />
        void ICollection<IDbBatchLocator<TConnection, TTransaction>>.CopyTo (IDbBatchLocator<TConnection, TTransaction>[] array, int arrayIndex)
        {
            this.BatchLocators.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<IDbBatchLocator<TConnection, TTransaction>> GetEnumerator ()
        {
            return this.BatchLocators.GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf (IDbBatchLocator<TConnection, TTransaction> item)
        {
            return this.BatchLocators.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert (int index, IDbBatchLocator<TConnection, TTransaction> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.BatchLocators.Insert(index, item);
        }

        /// <inheritdoc />
        public bool Remove (IDbBatchLocator<TConnection, TTransaction> item)
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

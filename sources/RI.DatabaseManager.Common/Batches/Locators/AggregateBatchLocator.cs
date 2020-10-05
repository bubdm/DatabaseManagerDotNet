﻿using System;
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
    public sealed class AggregateBatchLocator<TConnection, TTransaction> : IDbBatchLocator<TConnection, TTransaction>, IList<IDbBatchLocator>, ICollection<IDbBatchLocator>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateBatchLocator" />.
        /// </summary>
        public AggregateBatchLocator ()
            : this((IEnumerable<IDbBatchLocator>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateBatchLocator" />.
        /// </summary>
        /// <param name="batchLocators"> The sequence of batch locators which are aggregated. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="batchLocators" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        public AggregateBatchLocator (IEnumerable<IDbBatchLocator> batchLocators)
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
        /// <param name="batchLocators"> The array of batch locators which are aggregated. </param>
        public AggregateBatchLocator (params IDbBatchLocator[] batchLocators)
            : this((IEnumerable<IDbBatchLocator>)batchLocators) { }

        #endregion




        #region Instance Properties/Indexer

        private List<IDbBatchLocator> BatchLocators { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        IDbBatch IDbBatchLocator.GetBatch(string name, string commandSeparator, Func<IDbBatch> batchCreator)
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

            foreach (IDbBatchLocator batchLocator in this.BatchLocators)
            {
                IDbBatch currentBatch = batchLocator.GetBatch(name, commandSeparator, batchCreator);

                if (currentBatch != null)
                {
                    return currentBatch;
                }
            }

            return null;
        }

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




        #region Interface: IList<IDbBatchLocator>

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

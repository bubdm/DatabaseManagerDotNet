using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

using RI.DatabaseManager.Batches.Commands;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Default implementation of <see cref="IDbBatchCommandCollection" /> and
    ///     <see cref="IDbBatchCommandCollection{TConnection, TTransaction, TParameterTypes}" /> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class
        DbBatchCommandCollection <TConnection, TTransaction, TParameterTypes> : IDbBatchCommandCollection<TConnection,
            TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Properties/Indexer

        private List<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>> InternalList { get; } =
            new List<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>>();

        #endregion




        #region Instance Methods

        /// <inheritdoc cref="ICloneable.Clone" />
        public DbBatchCommandCollection<TConnection, TTransaction, TParameterTypes> Clone ()
        {
            DbBatchCommandCollection<TConnection, TTransaction, TParameterTypes> clone =
                new DbBatchCommandCollection<TConnection, TTransaction, TParameterTypes>();

            foreach (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> command in this.InternalList)
            {
                clone.Add(command);
            }

            return clone;
        }

        #endregion




        #region Interface: ICloneable

        /// <inheritdoc />
        object ICloneable.Clone () => this.Clone();

        #endregion




        #region Interface: ICollection<IDbBatchCommand<TConnection,TTransaction,TParameterTypes>>

        /// <inheritdoc />
        public int Count => this.InternalList.Count;

        /// <inheritdoc />
        public bool IsReadOnly =>
            ((ICollection<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>>)this.InternalList).IsReadOnly;

        /// <inheritdoc />
        public void Add (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> item) =>
            this.InternalList.Add(item);

        /// <inheritdoc />
        public void Clear () => this.InternalList.Clear();

        /// <inheritdoc />
        public bool Contains (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> item) =>
            this.InternalList.Contains(item);

        /// <inheritdoc />
        public void CopyTo (IDbBatchCommand<TConnection, TTransaction, TParameterTypes>[] array, int arrayIndex) =>
            this.InternalList.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> item) =>
            this.InternalList.Remove(item);

        #endregion




        #region Interface: IDbBatchCommandCollection

        /// <inheritdoc />
        void IDbBatchCommandCollection.ClearCommands () => this.Clear();

        /// <inheritdoc />
        public IList<IDbBatchCommand> GetAll () =>
            this.InternalList.Cast<IDbBatchCommand>()
                .ToList();

        #endregion




        #region Interface: IDbBatchCommandCollection<TConnection,TTransaction,TParameterTypes>

        /// <inheritdoc />
        public IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddCallback (
            CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> callback,
            DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare,
            IsolationLevel? isolationLevel = null, DbBatchExecutionType executionType = DbBatchExecutionType.Reader)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            CallbackBatchCommand<TConnection, TTransaction, TParameterTypes> command =
                new CallbackBatchCommand<TConnection, TTransaction, TParameterTypes>(callback, transactionRequirement,
                    isolationLevel, executionType);

            this.InternalList.Add(command);

            return command;
        }

        /// <inheritdoc />
        public void AddRange (IEnumerable<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>> commands)
        {
            if (commands == null)
            {
                return;
            }

            foreach (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> command in commands)
            {
                this.InternalList.Add(command);
            }
        }

        /// <inheritdoc />
        public IDbBatchCommand<TConnection, TTransaction, TParameterTypes> AddScript (string script,
            DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare,
            IsolationLevel? isolationLevel = null, DbBatchExecutionType executionType = DbBatchExecutionType.Reader)
        {
            ScriptBatchCommand<TConnection, TTransaction, TParameterTypes> command =
                new ScriptBatchCommand<TConnection, TTransaction, TParameterTypes>(script, transactionRequirement,
                    isolationLevel, executionType);

            this.InternalList.Add(command);

            return command;
        }

        #endregion




        #region Interface: IEnumerable

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator () => this.InternalList.GetEnumerator();

        #endregion




        #region Interface: IEnumerable<IDbBatchCommand<TConnection,TTransaction,TParameterTypes>>

        /// <inheritdoc />
        public IEnumerator<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>> GetEnumerator () =>
            this.InternalList.GetEnumerator();

        #endregion




        #region Interface: IList<IDbBatchCommand<TConnection,TTransaction,TParameterTypes>>

        /// <inheritdoc />
        public IDbBatchCommand<TConnection, TTransaction, TParameterTypes> this [int index]
        {
            get => this.InternalList[index];
            set => this.InternalList[index] = value;
        }

        /// <inheritdoc />
        public int IndexOf (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> item) =>
            this.InternalList.IndexOf(item);

        /// <inheritdoc />
        public void Insert (int index, IDbBatchCommand<TConnection, TTransaction, TParameterTypes> item) =>
            this.InternalList.Insert(index, item);

        /// <inheritdoc />
        public void RemoveAt (int index) => this.InternalList.RemoveAt(index);

        #endregion
    }
}

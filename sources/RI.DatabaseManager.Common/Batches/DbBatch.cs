using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Default implementation of <see cref="IDbBatch" /> and <see cref="IDbBatch{TConnection,TTransaction,TParameterTypes}"/> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbBatch<TConnection, TTransaction, TParameterTypes> : IDbBatch<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        private sealed class CommandCollection : List<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>>, IList<IDbBatchCommand>
        {
            /// <inheritdoc />
            IEnumerator<IDbBatchCommand> IEnumerable<IDbBatchCommand>.GetEnumerator () => this.GetEnumerator();

            /// <inheritdoc />
            void ICollection<IDbBatchCommand>.Add (IDbBatchCommand item) => this.Add((IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)item);

            /// <inheritdoc />
            bool ICollection<IDbBatchCommand>.Contains (IDbBatchCommand item) => this.Contains((IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)item);

            /// <inheritdoc />
            void ICollection<IDbBatchCommand>.CopyTo (IDbBatchCommand[] array, int arrayIndex) => this.CopyTo((IDbBatchCommand<TConnection, TTransaction, TParameterTypes>[])array, arrayIndex);

            /// <inheritdoc />
            bool ICollection<IDbBatchCommand>.Remove (IDbBatchCommand item) => this.Remove((IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)item);

            /// <inheritdoc />
            bool ICollection<IDbBatchCommand>.IsReadOnly => ((IList<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>>)this).IsReadOnly;

            /// <inheritdoc />
            int IList<IDbBatchCommand>.IndexOf (IDbBatchCommand item) => this.IndexOf((IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)item);

            /// <inheritdoc />
            void IList<IDbBatchCommand>.Insert(int index, IDbBatchCommand item) => this.Insert(index, (IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)item);

            /// <inheritdoc />
            IDbBatchCommand IList<IDbBatchCommand>.this [int index]
            {
                get => this[index];
                set => this[index] = (IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)value;
            }
        }

        public DbBatch () { }

        private CommandCollection CommandsInternal { get; } = new CommandCollection();

        /// <inheritdoc />
        public IList<IDbBatchCommand<TConnection, TTransaction, TParameterTypes>> Commands => this.CommandsInternal;

        /// <inheritdoc />
        public IDbBatchCommandParameterCollection<TParameterTypes> Parameters { get; private set; } = new DbBatchCommandParameterCollection<TParameterTypes>();

        /// <inheritdoc />
        public IsolationLevel? IsolationLevel { get; set; } = System.Data.IsolationLevel.ReadCommitted;

        /// <inheritdoc />
        IDbBatchCommandParameterCollection IDbBatch.Parameters => this.Parameters;

        /// <inheritdoc />
        IList<IDbBatchCommand> IDbBatch.Commands => this.CommandsInternal;

        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <inheritdoc cref="ICloneable.Clone"/>
        public DbBatch<TConnection, TTransaction, TParameterTypes> Clone()
        {
            DbBatch<TConnection, TTransaction, TParameterTypes> clone = new DbBatch<TConnection, TTransaction, TParameterTypes>();

            foreach (IDbBatchCommand<TConnection, TTransaction, TParameterTypes> command in this.CommandsInternal)
            {
                clone.CommandsInternal.Add((IDbBatchCommand<TConnection, TTransaction, TParameterTypes>)command.Clone());
            }

            clone.Parameters = (DbBatchCommandParameterCollection<TParameterTypes>)this.Parameters.Clone();
            clone.IsolationLevel = this.IsolationLevel;

            return clone;
        }
    }
}

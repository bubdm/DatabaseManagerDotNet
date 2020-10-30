using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Default implementation of <see cref="IDbBatch" /> and <see cref="IDbBatch{TConnection,TTransaction}"/> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbBatch<TConnection, TTransaction> : IDbBatch<TConnection, TTransaction>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        private sealed class CommandCollection <TC, TT> : List<IDbBatchCommand<TConnection, TTransaction>>, IList<IDbBatchCommand>
            where TC : DbConnection
            where TT : DbTransaction
        {
            /// <inheritdoc />
            IEnumerator<IDbBatchCommand> IEnumerable<IDbBatchCommand>.GetEnumerator () => this.GetEnumerator();

            /// <inheritdoc />
            void ICollection<IDbBatchCommand>.Add (IDbBatchCommand item) => this.Add((IDbBatchCommand<TConnection, TTransaction>)item);

            /// <inheritdoc />
            bool ICollection<IDbBatchCommand>.Contains (IDbBatchCommand item) => this.Contains((IDbBatchCommand<TConnection, TTransaction>)item);

            /// <inheritdoc />
            void ICollection<IDbBatchCommand>.CopyTo (IDbBatchCommand[] array, int arrayIndex) => this.CopyTo((IDbBatchCommand<TConnection, TTransaction>[])array, arrayIndex);

            /// <inheritdoc />
            bool ICollection<IDbBatchCommand>.Remove (IDbBatchCommand item) => this.Remove((IDbBatchCommand<TConnection, TTransaction>)item);

            /// <inheritdoc />
            bool ICollection<IDbBatchCommand>.IsReadOnly => ((IList<IDbBatchCommand<TConnection, TTransaction>>)this).IsReadOnly;

            /// <inheritdoc />
            int IList<IDbBatchCommand>.IndexOf (IDbBatchCommand item) => this.IndexOf((IDbBatchCommand<TConnection, TTransaction>)item);

            /// <inheritdoc />
            void IList<IDbBatchCommand>.Insert(int index, IDbBatchCommand item) => this.Insert(index, (IDbBatchCommand<TConnection, TTransaction>)item);

            /// <inheritdoc />
            IDbBatchCommand IList<IDbBatchCommand>.this [int index]
            {
                get => this[index];
                set => this[index] = (IDbBatchCommand<TConnection, TTransaction>)value;
            }
        }

        private CommandCollection<TConnection, TTransaction> CommandsInternal { get; } = new CommandCollection<TConnection, TTransaction>();

        /// <inheritdoc />
        public IList<IDbBatchCommand<TConnection, TTransaction>> Commands => this.CommandsInternal;

        /// <inheritdoc />
        IList<IDbBatchCommand> IDbBatch.Commands => this.CommandsInternal;

        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <inheritdoc cref="ICloneable.Clone"/>
        public DbBatch<TConnection, TTransaction> Clone()
        {
            DbBatch<TConnection, TTransaction> clone = new DbBatch<TConnection, TTransaction>();

            foreach (IDbBatchCommand<TConnection, TTransaction> command in this.CommandsInternal)
            {
                clone.CommandsInternal.Add((IDbBatchCommand<TConnection, TTransaction>)command.Clone());
            }

            return clone;
        }
    }
}

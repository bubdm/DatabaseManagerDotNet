using System;
using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Default implementation of <see cref="IDbBatch" /> and
    ///     <see cref="IDbBatch{TConnection,TTransaction,TParameterTypes}" /> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class
        DbBatch <TConnection, TTransaction, TParameterTypes> : IDbBatch<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Methods

        /// <inheritdoc cref="ICloneable.Clone" />
        public DbBatch<TConnection, TTransaction, TParameterTypes> Clone ()
        {
            DbBatch<TConnection, TTransaction, TParameterTypes> clone =
                new DbBatch<TConnection, TTransaction, TParameterTypes>();

            clone.Commands =
                (IDbBatchCommandCollection<TConnection, TTransaction, TParameterTypes>)this.Commands.Clone();

            clone.Parameters = (DbBatchCommandParameterCollection<TParameterTypes>)this.Parameters.Clone();

            return clone;
        }

        #endregion




        #region Interface: ICloneable

        /// <inheritdoc />
        object ICloneable.Clone ()
        {
            return this.Clone();
        }

        #endregion




        #region Interface: IDbBatch

        /// <inheritdoc />
        IDbBatchCommandCollection IDbBatch.Commands => this.Commands;

        /// <inheritdoc />
        IDbBatchCommandParameterCollection IDbBatch.Parameters => this.Parameters;

        #endregion




        #region Interface: IDbBatch<TConnection,TTransaction,TParameterTypes>

        /// <inheritdoc />
        public IDbBatchCommandCollection<TConnection, TTransaction, TParameterTypes> Commands { get; private set; } =
            new DbBatchCommandCollection<TConnection, TTransaction, TParameterTypes>();

        /// <inheritdoc />
        public IDbBatchCommandParameterCollection<TParameterTypes> Parameters { get; private set; } =
            new DbBatchCommandParameterCollection<TParameterTypes>();

        #endregion
    }
}

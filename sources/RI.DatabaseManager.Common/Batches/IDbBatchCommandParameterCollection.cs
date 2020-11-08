using System;
using System.Collections.Generic;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     A collection (set) of batch command parameters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The parameters contained in <see cref="IDbBatchCommandParameterCollection"/> must be unique, distinguished by their names (<see cref="IDbBatchCommandParameter.Name"/>).
    /// </para>
    /// </remarks>
    public interface IDbBatchCommandParameterCollection : ICloneable
    {
    }

    /// <inheritdoc cref="IDbBatchCommandParameterCollection" />
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbBatchCommandParameterCollection<TParameterTypes> : IDbBatchCommandParameterCollection, ISet<IDbBatchCommandParameter<TParameterTypes>>
        where TParameterTypes : Enum
    {
    }
}

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
    public interface IDbBatchCommandParameterCollection : ISet<IDbBatchCommandParameter>
    {
    }

    /// <inheritdoc cref="IDbBatchCommandParameterCollection" />
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IDbBatchCommandParameterCollection<TParameterTypes> : IDbBatchCommandParameterCollection, ISet<IDbBatchCommandParameter<TParameterTypes>>
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="ICollection{T}.Clear" />
        new void Clear ();

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        new IEnumerator<IDbBatchCommandParameter> GetEnumerator();

        /// <inheritdoc cref="ICollection{T}.Count" />
        new int Count { get; }

        /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
        new bool IsReadOnly { get; }
    }
}

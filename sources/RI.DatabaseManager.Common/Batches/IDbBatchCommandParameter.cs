using System;
using System.Data;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     A single batch command parameter.
    /// </summary>
    public interface IDbBatchCommandParameter : IEquatable<IDbBatchCommandParameter>, ICloneable
    {
        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        /// <value>
        ///     The name of the parameter.
        /// </value>
        string Name { get; }

        /// <summary>
        ///     Gets the type of the parameter.
        /// </summary>
        /// <value>
        ///     The type of the parameter.
        /// </value>
        DbType Type { get; }

        /// <summary>
        ///     Gets or sets the value of the parameter.
        /// </summary>
        /// <value>
        ///     The value of the parameter. Can be null.
        /// </value>
        object Value { get; set; }
    }

    /// <inheritdoc cref="IDbBatchCommandParameter" />
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbBatchCommandParameter <TParameterTypes> : IDbBatchCommandParameter,
                                                                  IEquatable<IDbBatchCommandParameter<TParameterTypes>>
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbBatchCommandParameter.Type" />
        new TParameterTypes Type { get; }
    }
}

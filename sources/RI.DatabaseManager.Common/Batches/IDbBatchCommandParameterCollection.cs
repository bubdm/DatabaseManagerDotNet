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
        /// <summary>
        /// Checks whether a parameter with the given name is already in the collection.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>
        /// true if the collection contains a parameter with the given name, false otherwise.
        /// </returns>
        bool Contains (string name);

        /// <summary>
        /// Removes a parameter with the given name from the collection.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>
        /// true if the collection contained a parameter with the given name (which was removed), false otherwise.
        /// </returns>
        bool Remove(string name);

        /// <summary>
        /// Gets all command parameters of the collection.
        /// </summary>
        /// <returns>
        /// The enumerator for all parameters.
        /// If there are no parameters, an empty enumerator is returned.
        /// </returns>
        IEnumerable<IDbBatchCommandParameter> GetAll ();

        /// <summary>
        /// Clears the collection of all parameters.
        /// </summary>
        void Clear ();
    }

    /// <inheritdoc cref="IDbBatchCommandParameterCollection" />
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbBatchCommandParameterCollection<TParameterTypes> : IDbBatchCommandParameterCollection, ISet<IDbBatchCommandParameter<TParameterTypes>>
        where TParameterTypes : Enum
    {
        /// <summary>
        /// Adds multiple parameters to the collection.
        /// </summary>
        /// <param name="parameters">The sequence of parameters to add.</param>
        /// <remarks>
        /// <para>
        /// <paramref name="parameters"/> is only enumerated once.
        /// </para>
        /// </remarks>
        void AddRange (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> parameters);

        /// <summary>
        /// Adds a parameter to the collection.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The database type of the parameter.</param>
        /// <param name="value">The optional parameter value. Default value is null.</param>
        /// <returns>
        /// The created and added parameter.
        /// </returns>
        IDbBatchCommandParameter<TParameterTypes> Add(string name, TParameterTypes type, object value = null);
    }
}

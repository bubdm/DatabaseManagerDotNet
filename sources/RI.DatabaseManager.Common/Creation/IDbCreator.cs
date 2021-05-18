using System;
using System.Data.Common;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Creation
{
    /// <summary>
    ///     Database creator to create a non-existing database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database creators are used to create new databases if they do not yet exist.
    ///         What the creation does in detail depends on the database type and the implementation of <see cref="IDbCreator" />.
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDbCreator" /> are always specific for a particular type of database (or particular implementation of <see cref="IDbManager" /> respectively).
    ///     </para>
    ///     <note type="note">
    ///         Database creators are optional.
    ///         If not configured, creation is not available / not supported.
    ///     </note>
    ///     <note type="note">
    ///         <see cref="IDbCreator" /> implementations are used by database managers.
    ///         Do not use <see cref="IDbCreator" /> implementations directly.
    ///     </note>
    /// </remarks>
    public interface IDbCreator
    {
        /// <summary>
        ///     Creates the database
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <returns>
        ///     true if the creation was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> is null. </exception>
        bool Create (IDbManager manager);
    }

    /// <inheritdoc cref="IDbCreator" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbCreator <TConnection, TTransaction, TParameterTypes> : IDbCreator
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbCreator.Create" />
        bool Create (IDbManager<TConnection, TTransaction, TParameterTypes> manager);
    }
}

using System;
using System.Data.Common;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Cleanup
{
    /// <summary>
    ///     Database cleanup processor.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database cleanup processors are used to cleanup a database.
    ///         What the cleanup does in detail depends on the database type and the implementation of <see cref="IDbCleanupProcessor" /> but is usually something like &quot;vacuum&quot;, recreate indices, etc.
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDbCleanupProcessor" /> are always specific for a particular type of database (or particular implementation of <see cref="IDbManager" /> respectively).
    ///     </para>
    ///     <note type="note">
    ///         Database cleanup processors are optional.
    ///         If not configured, cleanup is not available / not supported.
    ///     </note>
    ///     <note type="note">
    ///         <see cref="IDbCleanupProcessor" /> implementations are used by database managers.
    ///         Do not use <see cref="IDbCleanupProcessor" /> implementations directly.
    ///     </note>
    /// </remarks>
    public interface IDbCleanupProcessor
    {
        /// <summary>
        ///     Performs a cleanup of a database.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <returns>
        ///     true if the cleanup was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> is null. </exception>
        bool Cleanup (IDbManager manager);
    }

    /// <inheritdoc cref="IDbCleanupProcessor" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbCleanupProcessor <TConnection, TTransaction, TParameterTypes> : IDbCleanupProcessor
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbCleanupProcessor.Cleanup" />
        bool Cleanup (IDbManager<TConnection, TTransaction, TParameterTypes> manager);
    }
}

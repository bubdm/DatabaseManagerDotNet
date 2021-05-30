using System;
using System.Data.Common;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Versioning
{
    /// <summary>
    ///     Database version detector to detect the schema version of a database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database version detectors are used to detect the state and version of a database.
    ///         What the version detection does in detail depends on the database type and the implementation of
    ///         <see cref="IDbVersionDetector" />.
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDbVersionDetector" /> are always specific for a particular type of database (or
    ///         particular implementation of <see cref="IDbManager" /> respectively).
    ///     </para>
    ///     <note type="note">
    ///         Database version detectors are mandatory.
    ///         A database version detector must be configured before a database manager can be built or used.
    ///     </note>
    ///     <note type="note">
    ///         <see cref="IDbVersionDetector" /> implementations are used by database managers.
    ///         Do not use <see cref="IDbVersionDetector" /> implementations directly.
    ///     </note>
    /// </remarks>
    public interface IDbVersionDetector
    {
        /// <summary>
        ///     Detects the state and version of a database.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="state">
        ///     Returns the state of the database. Can return null to let <paramref name="manager" /> (or the
        ///     implementation of <see cref="IDbManager" /> respectively) determine the state based on <paramref name="version" />
        ///     alone.
        /// </param>
        /// <param name="version">
        ///     Returns the version of the database. Return -1 to indicate when the database is damaged or in an
        ///     invalid state. Return 0 to indicate that the database does not yet exist and needs to be created.
        /// </param>
        /// <returns>
        ///     true if the state and version of the database could be successfully determined, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> is null. </exception>
        bool Detect (IDbManager manager, out DbState? state, out int version);
    }

    /// <inheritdoc cref="IDbVersionDetector" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbVersionDetector <TConnection, TTransaction, TParameterTypes> : IDbVersionDetector
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbVersionDetector.Detect" />
        bool Detect (IDbManager<TConnection, TTransaction, TParameterTypes> manager, out DbState? state,
                     out int version);
    }
}

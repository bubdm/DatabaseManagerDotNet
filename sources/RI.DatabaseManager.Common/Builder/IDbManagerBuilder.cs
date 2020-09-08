using System.Data.Common;

using RI.Abstractions.Builder;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Database manager builder used to configure and build database managers.
    /// </summary>
    public interface IDbManagerBuilder : IBuilder
    {
    }

    /// <inheritdoc cref="IDbManagerBuilder" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    public interface IDbManagerBuilder <TConnection, TTransaction, TManager> : IDbManagerBuilder
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction, TManager> { }
}

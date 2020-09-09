using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Batches (<see cref="IDbBatch{TConnection,TTransaction}" />) can be used to group multiple commands into one unit.
    ///         Some dependencies (e.g. version detectors) might use batches to retrieve the commands required to execute their functionality.
    ///     </para>
    /// </remarks>
    public interface IDbBatch
    {
        
    }

    public interface IDbBatch<TConnection, TTransaction> : IDbBatch
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {

    }
}

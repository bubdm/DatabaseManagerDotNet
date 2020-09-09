using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    public interface IDbBatch
    {
        
    }

    public interface IDbBatch<TConnection, TTransaction> : IDbBatch
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {

    }
}

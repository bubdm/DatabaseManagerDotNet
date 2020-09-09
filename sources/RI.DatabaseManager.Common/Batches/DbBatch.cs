using System.Data.Common;




namespace RI.DatabaseManager.Batches
{
    public class DbBatch<TConnection, TTransaction> : IDbBatch<TConnection, TTransaction>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        
    }
}

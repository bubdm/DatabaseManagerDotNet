using System.Collections.Generic;
using System.Data.Common;
using System.Net.NetworkInformation;




namespace RI.DatabaseManager.Batches
{
    public interface IDbBatchLocator
    {
        ISet<string> GetNames ();

        IDbBatch<TConnection, TTransaction> GetBatch<TConnection, TTransaction>(string name, string commandSeparator, bool preprocess)
            where TConnection : DbConnection
            where TTransaction : DbTransaction;
    }
}

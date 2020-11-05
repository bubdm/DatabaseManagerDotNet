using System;
using System.Collections.Generic;




namespace RI.DatabaseManager.Batches
{
    public interface IDbBatchCommandParameterCollection : ISet<IDbBatchCommandParameter>
    {
        
    }

    public interface IDbBatchCommandParameterCollection<TParameterTypes> : IDbBatchCommandParameterCollection, ISet<IDbBatchCommandParameter<TParameterTypes>>
        where TParameterTypes : Enum
    {

    }
}

using System;
using System.Data;




namespace RI.DatabaseManager.Batches
{
    public interface IDbBatchCommandParameter : IEquatable<IDbBatchCommandParameter>
    {
        string Name { get; }

        object Value { get; }

        DbType Type { get; }
    }

    public interface IDbBatchCommandParameter<TParameterTypes> : IDbBatchCommandParameter, IEquatable<IDbBatchCommandParameter<TParameterTypes>>
     where TParameterTypes : Enum
    {
        new TParameterTypes Type { get; }
    }
}

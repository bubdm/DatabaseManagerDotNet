using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Batches.Locators;




namespace RI.DatabaseManager.Tests.Resources
{
    [CallbackBatch(Name = "CBx", IsolationLevel = IsolationLevel.Chaos, TransactionRequirement = DbBatchTransactionRequirement.Disallowed)]
    public sealed class TestCallback2 : ICallbackBatch<SQLiteConnection, SQLiteTransaction, DbType>
    {
        /// <inheritdoc />
        public List<object> Execute (SQLiteConnection connection, SQLiteTransaction transaction, IDbBatchCommandParameterCollection<DbType> parameters, out string error, out Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}

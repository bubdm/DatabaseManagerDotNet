using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Batches.Locators;




namespace RI.DatabaseManager.Tests.Resources
{
    public sealed class TestCallback1 : ICallbackBatch<SQLiteConnection, SQLiteTransaction, DbType>
    {
        /// <inheritdoc />
        public object Execute (SQLiteConnection connection, SQLiteTransaction transaction, IDbBatchCommandParameterCollection<DbType> parameters, out string error, out Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}

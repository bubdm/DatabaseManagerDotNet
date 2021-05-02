using System;
using System.Collections.Generic;
using System.Data.Common;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which does nothing.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class NullBatchLocator<TConnection, TTransaction, TParameterTypes> : IDbBatchLocator<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc />
        public IDbBatch<TConnection, TTransaction, TParameterTypes> GetBatch (string name, string commandSeparator, Func<IDbBatch<TConnection, TTransaction, TParameterTypes>> batchCreator) => null;

        /// <inheritdoc />
        public IDbBatch GetBatch (string name, string commandSeparator, Func<IDbBatch> batchCreator) => null;

        /// <inheritdoc />
        public ISet<string> GetNames () => new HashSet<string>();
    }
}

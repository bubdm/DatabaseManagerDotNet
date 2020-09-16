using System.Collections.Generic;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Default implementation of <see cref="IDbBatch" /> suitable for most scenarios.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbBatch : IDbBatch
    {
        #region Interface: IDbBatch

        /// <inheritdoc />
        public List<IDbBatchCommand> Commands { get; } = new List<IDbBatchCommand>();

        #endregion
    }
}

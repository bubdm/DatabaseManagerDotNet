using System;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Describes the execution type of a batch or a batch command.
    /// </summary>
    [Serializable]
    public enum DbBatchExecutionType
    {
        /// <summary>
        ///     The command is executed as reader, reading multiple results.
        /// </summary>
        Reader = 0,

        /// <summary>
        ///     The command is executed as scalar, reading only the first column of the first row.
        /// </summary>
        Scalar = 1,

        /// <summary>
        ///     The command is executed as non-query, reading the number of affected rows.
        /// </summary>
        NonQuery = 2,
    }
}

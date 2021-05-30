namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    ///     Stores general database manager options and also provides the means to upgrade the database.
    /// </summary>
    public interface ISupportDefaultDatabaseUpgrading : IDbManagerOptions
    {
        /// <summary>
        ///     Gets the the default used version upgrader batch name format as a regular expression (RegEx) used to lookup batches
        ///     using their names.
        /// </summary>
        /// <returns>
        ///     The RegExp with batch name format as a regular expression (RegEx) used to lookup batches using their names or null
        ///     or an empty string if a name format is not available.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value should be <c> .+?(?&lt;sourceVersion&gt;\d{4}).* </c> as this is database-independent.
        ///     </note>
        ///     <note type="important">
        ///         The batch name format must be a regular expression which provides a named capture <c> sourceVersion </c>. That
        ///         capture is used to extract the source version from which the batch can upgrade + 1 from the name of the batch.
        ///         Using the default value, <c> 0001 </c> (or <c> 1 </c> as <see cref="int" />) would be extracted as the source
        ///         version from a batch with the name<c> MyDb-Upgrade-0001-01 </c>.
        ///     </note>
        /// </remarks>
        string GetDefaultUpgradingBatchNameFormat ();
    }
}

using System;

using RI.DatabaseManager.Upgrading;




namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    /// Stores general database manager options and also provides a version upgrade RegEx pattern for version upgrader implementations (e.g. based on <see cref="BatchNameBasedDbVersionUpgrader{TConnection,TTransaction,TParameterTypes}"/>).
    /// </summary>
    public interface ISupportVersionUpgradeNameFormat : IDbManagerOptions
    {
        /// <summary>
        ///     Gets or sets the used version upgrader batch name format as a regular expression (RegEx) used to lookup batches using their names.
        /// </summary>
        /// <value>
        ///     The used version upgrader batch name format as a regular expression (RegEx) used to lookup batches using their names.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value should be <c> .+?(?&lt;sourceVersion&gt;\d{4}).* </c>.
        ///     </note>
        ///     <note type="important">
        ///         The name format must be a regular expression which provides a named capture <c> sourceVersion </c>. That capture is used to extract the source version from which the batch can upgrade + 1 from the name of the batch.
        ///         Using the default value, <c> 0001 </c> (or <c>1</c> as <see cref="int"/>) would be extracted as the source version from a batch with the name<c> MyDb-Upgrade-0001-01 </c>.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="value" /> is an empty string. </exception>
        string VersionUpgradeNameFormat { get; set; }
    }
}

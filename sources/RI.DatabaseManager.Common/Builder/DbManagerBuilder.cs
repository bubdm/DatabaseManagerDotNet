using RI.Abstractions.Builder;
using RI.Abstractions.Logging;
using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Upgrading;
using RI.DatabaseManager.Versioning;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Database manager builder used to configure and build database managers.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         <see cref="IBuilder.Build" /> must be called for actually constructing a database manager.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbManagerBuilder : BuilderBase
    {
        /// <inheritdoc />
        protected override void PrepareRegistrations (ILogger logger)
        {
            base.PrepareRegistrations(logger);

            this.ThrowIfNotExactContractCount(typeof(IDatabaseManager), 1);
            this.ThrowIfNotExactContractCount(typeof(IDatabaseVersionDetector), 1);

            this.ThrowIfNotMaxContractCount(typeof(IDatabaseBackupCreator), 1);
            this.ThrowIfNotMaxContractCount(typeof(IDatabaseCleanupProcessor), 1);
            this.ThrowIfNotMaxContractCount(typeof(IDatabaseVersionUpgrader), 1);
        }
    }
}

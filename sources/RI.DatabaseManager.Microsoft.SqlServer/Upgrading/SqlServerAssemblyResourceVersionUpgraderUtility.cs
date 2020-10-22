using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements an assembly version upgrade step extractor for SQL Server databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerAssemblyResourceVersionUpgraderUtility : AssemblyResourceVersionUpgraderUtility<SqlServerDbVersionUpgradeStep, SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDbManager, SqlServerDatabaseManagerConfiguration>
    {
        #region Overrides

        /// <inheritdoc />
        protected override SqlServerDbVersionUpgradeStep CreateProcessingStep (int sourceVersion, string resourceName)
        {
            SqlServerDbVersionUpgradeStep upgradeStep = new SqlServerDbVersionUpgradeStep(sourceVersion);
            if (resourceName != null)
            {
                upgradeStep.AddScript(resourceName, DbProcessingStepTransactionRequirement.Required);
            }
            return upgradeStep;
        }

        #endregion
    }
}

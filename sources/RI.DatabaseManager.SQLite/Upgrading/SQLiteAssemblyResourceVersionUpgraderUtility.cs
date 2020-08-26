using System.Data.SQLite;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements an assembly version upgrade step extractor for SQLite databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteAssemblyResourceVersionUpgraderUtility : AssemblyResourceVersionUpgraderUtility<SQLiteDatabaseVersionUpgradeStep, SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
    {
        #region Overrides

        /// <inheritdoc />
        protected override SQLiteDatabaseVersionUpgradeStep CreateProcessingStep (int sourceVersion, string resourceName)
        {
            SQLiteDatabaseVersionUpgradeStep upgradeStep = new SQLiteDatabaseVersionUpgradeStep(sourceVersion);
            if (resourceName != null)
            {
                upgradeStep.AddScript(resourceName, DatabaseProcessingStepTransactionRequirement.Required);
            }
            return upgradeStep;
        }

        #endregion
    }
}

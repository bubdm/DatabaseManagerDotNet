using System.Data.SQLite;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements an assembly version upgrade step extractor for SQLite databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteAssemblyResourceVersionUpgraderUtility : AssemblyResourceVersionUpgraderUtility<SQLiteDbVersionUpgradeStep, SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
    {
        #region Overrides

        /// <inheritdoc />
        protected override SQLiteDbVersionUpgradeStep CreateProcessingStep (int sourceVersion, string resourceName)
        {
            SQLiteDbVersionUpgradeStep upgradeStep = new SQLiteDbVersionUpgradeStep(sourceVersion);
            if (resourceName != null)
            {
                upgradeStep.AddScript(resourceName, DbProcessingStepTransactionRequirement.Required);
            }
            return upgradeStep;
        }

        #endregion
    }
}

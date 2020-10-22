using System.Data.SQLite;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements an assembly version upgrade step configurators for SQLite databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class SQLiteAssemblyResourceVersionUpgraderStepConfigurator : AssemblyResourceVersionUpgraderStepConfigurator<SQLiteDbVersionUpgradeStep, SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDbManager, SQLiteDatabaseManagerConfiguration>
    {
    }
}

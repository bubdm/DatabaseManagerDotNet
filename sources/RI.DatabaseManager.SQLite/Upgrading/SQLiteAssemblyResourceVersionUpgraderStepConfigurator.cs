using System.Data.SQLite;




namespace RI.Framework.Data.Database.Upgrading
{
    /// <summary>
    ///     Implements an assembly version upgrade step configurators for SQLite databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class SQLiteAssemblyResourceVersionUpgraderStepConfigurator : AssemblyResourceVersionUpgraderStepConfigurator<SQLiteDatabaseVersionUpgradeStep, SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
    {
    }
}

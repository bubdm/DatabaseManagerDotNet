using Microsoft.Data.SqlClient;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements an assembly version upgrade step configurators for SQL Server databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class SqlServerAssemblyResourceVersionUpgraderStepConfigurator : AssemblyResourceVersionUpgraderStepConfigurator<SqlServerDbVersionUpgradeStep, SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
    {
    }
}

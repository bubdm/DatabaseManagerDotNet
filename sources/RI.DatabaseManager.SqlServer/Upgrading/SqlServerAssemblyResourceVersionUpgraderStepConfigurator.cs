using Microsoft.Data.SqlClient;




namespace RI.Framework.Data.Database.Upgrading
{
    /// <summary>
    ///     Implements an assembly version upgrade step configurators for SQL Server databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class SqlServerAssemblyResourceVersionUpgraderStepConfigurator : AssemblyResourceVersionUpgraderStepConfigurator<SqlServerDatabaseVersionUpgradeStep, SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
    {
    }
}

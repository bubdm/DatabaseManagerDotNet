using System.Data.Common;




namespace RI.Framework.Data.Database.Upgrading
{
    /// <summary>
    ///     Defines the interface for assembly version upgrade step configurators.
    /// </summary>
    /// <typeparam name="TProcessingStep"> The type of processing steps this utility creates. </typeparam>
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <remarks>
    ///     <para>
    ///         <see cref="IAssemblyResourceVersionUpgraderStepConfigurator{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> is used with <see cref="AssemblyResourceVersionUpgraderUtility{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" />.
    ///         See <see cref="AssemblyResourceVersionUpgraderUtility{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IAssemblyResourceVersionUpgraderStepConfigurator <in TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TProcessingStep : IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        /// <summary>
        ///     Configures the version upgrade step belonging to the source version as indicated by <see cref="AssemblyResourceVersionUpgraderStepAttribute" />.
        /// </summary>
        /// <param name="step"> The processing step to configure. </param>
        /// <param name="sourceVersion"> The source version. </param>
        /// <param name="scriptFile"> The script file or null if no script is associated with this version upgrade step. </param>
        void ConfigureStep (TProcessingStep step, int sourceVersion, string scriptFile);
    }
}

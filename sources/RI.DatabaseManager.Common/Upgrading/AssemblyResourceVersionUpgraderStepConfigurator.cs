using System.Data.Common;




namespace RI.Framework.Data.Database.Upgrading
{
    /// <summary>
    ///     Implements a base class for assembly version upgrade step configurators.
    /// </summary>
    /// <typeparam name="TProcessingStep"> The type of processing steps this utility creates. </typeparam>
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <remarks>
    ///     <para>
    ///         It is recommended that assembly version upgrade step configurators implementations use this base class as it already implements most of the logic which is database-independent.
    ///     </para>
    ///     <para>
    ///         <see cref="AssemblyResourceVersionUpgraderStepConfigurator{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> is used with <see cref="AssemblyResourceVersionUpgraderUtility{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" />.
    ///         See <see cref="AssemblyResourceVersionUpgraderUtility{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class AssemblyResourceVersionUpgraderStepConfigurator <TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IAssemblyResourceVersionUpgraderStepConfigurator<TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TProcessingStep : IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        #region Interface: IAssemblyResourceVersionUpgraderStepConfigurator<TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration>

        /// <inheritdoc />
        public abstract void ConfigureStep (TProcessingStep step, int sourceVersion, string scriptFile);

        #endregion
    }
}

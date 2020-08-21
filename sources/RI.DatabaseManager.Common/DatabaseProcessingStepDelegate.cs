using System.Data.Common;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Defines a delegate which can be used to define code sub-steps for <see cref="DatabaseProcessingStep{TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration}" />s.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <param name="step"> The processing step being executed. </param>
    /// <param name="manager"> The used database manager. </param>
    /// <param name="connection"> The current connection used to execute the processing step or sub-step respectively. </param>
    /// <param name="transaction"> The current transaction used to execute the processing step or sub-step respectively. Can be null, depending on <see cref="DatabaseProcessingStep{TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration}.RequiresTransaction" />. </param>
    /// <threadsafety static="false" instance="false" />
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public delegate void DatabaseProcessingStepDelegate <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> (DatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> step, TManager manager, TConnection connection, TTransaction transaction)
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new();

    /// <summary>
    ///     Defines a delegate which can be used to define code sub-steps for <see cref="DatabaseProcessingStep{TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration}" />s.
    /// </summary>
    /// <param name="step"> The processing step being executed. </param>
    /// <param name="manager"> The used database manager. </param>
    /// <param name="connection"> The current connection used to execute the processing step or sub-step respectively. </param>
    /// <param name="transaction"> The current transaction used to execute the processing step or sub-step respectively. Can be null, depending on <see cref="DatabaseProcessingStep{TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration}.RequiresTransaction" />. </param>
    /// <threadsafety static="false" instance="false" />
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public delegate void DatabaseProcessingStepDelegate (object step, IDatabaseManager manager, DbConnection connection, DbTransaction transaction);
}

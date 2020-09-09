using System.Data.Common;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Defines a delegate which can be used to define code sub-steps for <see cref="IDbProcessingStep{TConnection,TTransaction,TManager}" />s.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <param name="step"> The processing step being executed. </param>
    /// <param name="manager"> The used database manager. </param>
    /// <param name="connection"> The current connection used to execute the processing step or sub-step respectively. </param>
    /// <param name="transaction"> The current transaction used to execute the processing step or sub-step respectively. Can be null, depending on <see cref="IDbProcessingStep.RequiresTransaction" />. </param>
    /// <threadsafety static="false" instance="false" />
    public delegate void DbProcessingStepDelegate <TConnection, TTransaction, TManager> (IDbProcessingStep<TConnection, TTransaction, TManager> step, TManager manager, TConnection connection, TTransaction transaction)
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TManager : class, IDbManager<TConnection, TTransaction, TManager>;

    /// <summary>
    ///     Defines a delegate which can be used to define code sub-steps for <see cref="IDbProcessingStep" />s.
    /// </summary>
    /// <param name="step"> The processing step being executed. </param>
    /// <param name="manager"> The used database manager. </param>
    /// <param name="connection"> The current connection used to execute the processing step or sub-step respectively. </param>
    /// <param name="transaction"> The current transaction used to execute the processing step or sub-step respectively. Can be null, depending on <see cref="IDbProcessingStep.RequiresTransaction" />. </param>
    /// <threadsafety static="false" instance="false" />
    public delegate void DbProcessingStepDelegate (IDbProcessingStep step, IDbManager manager, DbConnection connection, DbTransaction transaction);
}

using System;
using System.Data.Common;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IDatabaseManager" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class IDatabaseManagerExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Executes an arbitrary database processing step.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
        /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
        /// <typeparam name="TManager"> The type of the database manager. </typeparam>
        /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="step"> The database processing step to execute. </param>
        /// <param name="readOnly"> Specifies whether the connection, used to process the step, should be read-only. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> or <paramref name="step" /> is null </exception>
        public static void ExecuteProcessingStep <TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> (this TManager manager, IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> step, bool readOnly)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TConnectionStringBuilder : DbConnectionStringBuilder
            where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
            where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            using (TConnection connection = manager.CreateConnection(readOnly, false))
            {
                using (TTransaction transaction = step.RequiresTransaction ? (TTransaction)connection.BeginTransaction() : null)
                {
                    step.Execute(manager, connection, transaction);
                }
            }
        }

        #endregion
    }
}

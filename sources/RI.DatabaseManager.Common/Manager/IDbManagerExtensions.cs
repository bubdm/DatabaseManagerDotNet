using System;
using System.Collections.Generic;
using System.Data.Common;

using RI.DatabaseManager.Scripts;
using RI.DatabaseManager.Upgrading;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IDbManager" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class IDbManagerExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Executes an arbitrary database processing step.
        /// </summary>
        /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
        /// <typeparam name="TManager"> The type of the database manager. </typeparam>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="step"> The database processing step to execute. </param>
        /// <param name="readOnly"> Specifies whether the connection, used to process the step, should be read-only. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> or <paramref name="step" /> is null </exception>
        public static void ExecuteProcessingStep <TConnection, TTransaction, TManager> (this TManager manager, IDbProcessingStep<TConnection, TTransaction, TManager> step, bool readOnly)
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TManager : class, IDbManager<TConnection, TTransaction, TManager>
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            using (TConnection connection = manager.CreateConnection(readOnly))
            {
                using (TTransaction transaction = step.RequiresTransaction ? (TTransaction)connection.BeginTransaction() : null)
                {
                    step.Execute(manager, connection, transaction);
                }
            }
        }



        /// <summary>
        ///     Performs an upgrade to highest supported version using the configured <see cref="IDatabaseVersionUpgrader" />.
        /// </summary>
        /// <returns>
        ///     true if the upgrade was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <param name="manager"> The used database manager. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="IDbManager.State" />, <see cref="IDbManager.Version" />, <see cref="IDbManager.IsReady"/>, <see cref="IDbManager.CanUpgrade"/> are updated to reflect the current state and version of the database after upgrade.
        ///     </note>
        ///     <note type="implement">
        ///         If <see cref="IDbManager.MaxVersion" /> is the same as <see cref="IDbManager.Version" />, nothing should be done.
        ///     </note>
        ///     <note type="implement">
        ///         Upgrading is to be performed incrementally, upgrading from n to n+1 until the desired version, <see cref="IDbManager.MaxVersion" />, is reached.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> is null </exception>
        /// <exception cref="InvalidOperationException"> The database is not in a ready or the new state. </exception>
        /// <exception cref="NotSupportedException"> Upgrading is not supported by the database manager or no <see cref="IDatabaseVersionUpgrader" /> is configured. </exception>
        public static bool Upgrade (this IDbManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            return manager.Upgrade(manager.MaxVersion);
        }

        /// <summary>
        ///     Retrieves a script and all its batches using the configured <see cref="IDbScriptLocator" />, using its default batch separator.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="name"> The name of the script. </param>
        /// <param name="preprocess"> Specifies whether the script is to be preprocessed, if applicable. </param>
        /// <returns>
        ///     The batches in the script (list of independently executed commands).
        ///     If the script is empty or does not contain any commands respectively, an empty list is returned.
        ///     If the script could not be found, null is returned.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="GetScriptBatches"/> should be callable at any time as the retrieved script is just retrieved, not executed.
        ///     </note>
        /// </remarks>
        /// <remarks>
        ///<para>
        /// The configured <see cref="IDbScriptLocator" />s <see cref="IDbScriptLocator.DefaultBatchSeparator"/> is used as the batch separator string.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="NotSupportedException"> Retrieving scripts is not supported by the database manager or no <see cref="IDbScriptLocator" /> is configured. </exception>
        public static List<string> GetScriptBatches (this IDbManager manager, string name, bool preprocess)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            return manager.GetScriptBatches(name, null, preprocess);
        }

        #endregion
    }
}

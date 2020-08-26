using System.Collections.Generic;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Scripts
{
    /// <summary>
    ///     Defines the interface for database script locators.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database script locators are used to locate, retrieve, and preprocess database scripts.
    ///         What the preprocessing does in detail depends on the implementation of <see cref="IDatabaseScriptLocator" /> but is usually something like replacing placeholders (e.g. current date and time), etc.
    ///     </para>
    ///     <para>
    ///         Database script locators are used by database managers (<see cref="IDatabaseManager" /> implementations) and version detectors (<see cref="IDatabaseVersionDetector" />), version upgraders (<see cref="IDatabaseVersionUpgrader" />), backup creators (<see cref="IDatabaseBackupCreator" />), and cleanup processors (<see cref="IDatabaseCleanupProcessor" />), depending on their implementation and the used database type.
    ///         Do not use database script locators directly but rather configure to use them through configuration (<see cref="IDatabaseManagerConfiguration.ScriptLocator" />).
    ///     </para>
    ///     <para>
    ///         To retrieve individual scripts for use with connections created by a database manager, use <see cref="IDatabaseManager.GetScriptBatch" />.
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDatabaseScriptLocator" /> are independent of the type of database (or the particular implementation of <see cref="IDatabaseManager" /> respectively).
    ///     </para>
    ///     <para>
    ///         Database script locators are optional.
    ///         However, be aware that some implementations and/or configurations of version detectors (<see cref="IDatabaseVersionDetector" />), version upgraders (<see cref="IDatabaseVersionUpgrader" />), backup creators (<see cref="IDatabaseBackupCreator" />), and cleanup processors (<see cref="IDatabaseCleanupProcessor" />) require a script locator.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface IDatabaseScriptLocator
    {
        /// <summary>
        ///     Gets or sets the string which is used as the separator to separate individual batches in a script.
        /// </summary>
        /// <value>
        ///     The string which is used as the separator to separate individual batches in a script or null if the script is not to be separated into individual batches.
        /// </value>
        /// <exception cref="EmptyStringArgumentException"><paramref name="value"/> is an empty string.</exception>
        string BatchSeparator { get; set; }

        /// <summary>
        ///     Retrieves a script batch with optional preprocessing.
        /// </summary>
        /// <param name="manager"> The used database manager representing the database. </param>
        /// <param name="name"> The name of the script. </param>
        /// <param name="preprocess"> Specifies whether the script is to be preprocessed. </param>
        /// <returns>
        ///     The list of batches in the script.
        ///     If the script is empty or does not contain any batches respectively, an empty list is returned.
        ///     If the script could not be found, null is returned.
        ///     If <see cref="BatchSeparator" /> is null, the whole script is returned as a single item in the returned list.
        /// </returns>
        List<string> GetScriptBatch (IDatabaseManager manager, string name, bool preprocess);
    }
}

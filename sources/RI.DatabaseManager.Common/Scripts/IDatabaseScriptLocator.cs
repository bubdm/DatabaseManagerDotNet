using System;
using System.Collections.Generic;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Scripts
{
    /// <summary>
    ///     Script locator to locate database scripts.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database script locators are used to locate, retrieve, and preprocess database scripts.
    ///         What the preprocessing does in detail depends on the implementation of <see cref="IDatabaseScriptLocator" /> but is usually something like replacing placeholders (e.g. current date and time), etc.
    ///     </para>
    ///     <para>
    ///         Database script locators might also be used by database managers, version detectors, version upgraders, backup creators, and cleanup processors, depending on their implementation and the used database type.
    ///     </para>
    /// <para>
    ///The database scripts can be further divided into batches, separated by <see cref="DefaultBatchSeparator"/>, so that parts of a single script can be executed in individual database commands or transactions.
    /// </para>
    ///     <note type="note">
    ///         Database script locators are optional.
    ///         However, be aware that some implementations and/or configurations of version detectors, version upgraders, backup creators, and cleanup processors require a script locator.
    /// See their respective documentation for more information.
    ///     </note>
    ///     <note type="note">
    ///         Do not use <see cref="IDatabaseScriptLocator"/> implementations directly.
    /// Use <see cref="IDbManager.GetScriptBatches"/> instead.
    ///     </note>
    /// </remarks>
    public interface IDatabaseScriptLocator
    {
        /// <summary>
        ///     Gets or sets the string which is used as the default separator to separate individual batches in a single script.
        /// </summary>
        /// <value>
        ///     The string which is used as the default separator to separate individual batches in a single script or null if the script is not to be separated into individual batches.
        /// </value>
        /// <remarks>
        ///<note type="implement">
        /// The default value of this property is expected to be <c>GO</c>.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="value"/> is an empty string.</exception>
        string DefaultBatchSeparator { get; set; }

        /// <summary>
        ///     Retrieves a script and all its batches.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="name"> The name of the script. </param>
        /// <param name="batchSeparator"> The string which is used as the separator to separate individual batches in the script or null if the script is not to be separated into individual batches. </param>
        /// <param name="preprocess"> Specifies whether the script is to be preprocessed. </param>
        /// <returns>
        ///     The list of batches in the script.
        ///     If the script is empty or does not contain any batches respectively, an empty list is returned.
        ///     If the script could not be found, null is returned.
        ///     If <paramref name="batchSeparator"/> is null, the whole script is returned as a single item in the returned list.
        /// </returns>
        /// <remarks>
        ///<para>
        ///<see cref="DefaultBatchSeparator"/> is ignored as only the value of <paramref name="batchSeparator"/> is used.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
        List<string> GetScriptBatches(IDbManager manager, string name, string batchSeparator, bool preprocess);
    }
}

using System;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Event arguments for the <see cref="IDatabaseManager.VersionChanged" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public sealed class DatabaseVersionChangedEventArgs : EventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseVersionChangedEventArgs" />.
        /// </summary>
        /// <param name="oldVersion"> The old version of the database. </param>
        /// <param name="newVersion"> The new version of the database. </param>
        public DatabaseVersionChangedEventArgs (int oldVersion, int newVersion)
        {
            this.OldVersion = oldVersion;
            this.NewVersion = newVersion;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the new version of the database.
        /// </summary>
        /// <value>
        ///     The new version of the database.
        /// </value>
        public int NewVersion { get; }


        /// <summary>
        ///     Gets the old version of the database.
        /// </summary>
        /// <value>
        ///     The old version of the database.
        /// </value>
        public int OldVersion { get; }

        #endregion
    }
}

using System;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Event arguments for the <see cref="IDatabaseManager.StateChanged" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public sealed class DatabaseStateChangedEventArgs : EventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseStateChangedEventArgs" />.
        /// </summary>
        /// <param name="oldState"> The old state of the database. </param>
        /// <param name="newState"> The new state of the database. </param>
        public DatabaseStateChangedEventArgs (DatabaseState oldState, DatabaseState newState)
        {
            this.OldState = oldState;
            this.NewState = newState;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the new state of the database.
        /// </summary>
        /// <value>
        ///     The new state of the database.
        /// </value>
        public DatabaseState NewState { get; }


        /// <summary>
        ///     Gets the old state of the database.
        /// </summary>
        /// <value>
        ///     The old state of the database.
        /// </value>
        public DatabaseState OldState { get; }

        #endregion
    }
}

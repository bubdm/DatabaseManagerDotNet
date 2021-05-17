using System;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Describes the current state of a database managed by a database manager.
    /// </summary>
    [Serializable,]
    public enum DbState
    {
        /// <summary>
        ///     The database manager is not initialized or has been closed.
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        ///     The database manager is initialized and ready for use, using the newest known/supported version of the database.
        /// </summary>
        ReadyNew = 1,

        /// <summary>
        ///     The database manager is initialized and ready for use, using an older known/supported version of the database.
        /// </summary>
        ReadyOld = 2,

        /// <summary>
        ///     The database manager is initialized and ready for use, using an unknown/unsupported version of the database.
        /// </summary>
        ReadyUnknown = 3,

        /// <summary>
        ///     The database manager is initialized but is not ready for use because the database is not created.
        /// </summary>
        New = 4,

        /// <summary>
        ///     The database manager is initialized but is not ready for use because the database is not available/accessible.
        /// </summary>
        Unavailable = 5,

        /// <summary>
        ///     The database manager is initialized but is not ready for use because the database version is newer than the newest version known/supported.
        /// </summary>
        TooNew = 6,

        /// <summary>
        ///     The database manager is initialized but is not ready for use because the database version is too old and cannot be upgraded.
        /// </summary>
        TooOld = 7,

        /// <summary>
        ///     The database manager is initialized but is not ready for use because the database is damaged or in an invalid state.
        /// </summary>
        DamagedOrInvalid = 8,
    }
}

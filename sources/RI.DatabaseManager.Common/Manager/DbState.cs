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
        ///     The database is not initialized or has been closed.
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        ///     The database is initialized and ready for use, using the newest known/supported version.
        /// </summary>
        ReadyNew = 1,

        /// <summary>
        ///     The database is initialized and ready for use, using an older known/supported version.
        /// </summary>
        ReadyOld = 2,

        /// <summary>
        ///     The database is initialized and ready for use, using an unknown/unsupported version (therefore, upgrading is not available).
        /// </summary>
        ReadyUnknown = 3,

        /// <summary>
        ///     The database is initialized but is not ready for use because it is not available/created and requires an upgrade to create the database.
        /// </summary>
        New = 4,

        /// <summary>
        ///     The database is initialized but is not ready for use because it is not available/accessible.
        /// </summary>
        Unavailable = 5,

        /// <summary>
        ///     The database is initialized but is not ready for use because its version is newer than the newest version known/supported.
        /// </summary>
        TooNew = 6,

        /// <summary>
        ///     The database is initialized but is not ready for use because its version is too old and cannot be upgraded.
        /// </summary>
        TooOld = 7,

        /// <summary>
        ///     The database is initialized but is not ready for use because it is damaged or invalid.
        /// </summary>
        DamagedOrInvalid = 8,
    }
}

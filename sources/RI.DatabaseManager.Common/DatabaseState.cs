using System;




namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Describes the current state of a database managed by an <see cref="IDatabaseManager" />.
	/// </summary>
	[Serializable]
	public enum DatabaseState
	{
		/// <summary>
		///     The database is not initialized.
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
		///     The database is initialized and ready for use, but upgrading is not supported/available and therefore versioning information is not available or unknown respectively.
		/// </summary>
		ReadyUnknown = 3,

		/// <summary>
		///     The database is initialized but is not ready for use because it is not available/created and requires an upgrade to create the database.
		/// </summary>
		New = 4,

		/// <summary>
		///     The database is initialized but is not ready for use because it is not available/created and upgrading is not supported/available so the database cannot be created through an upgrade.
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

namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    /// Stores general database manager options.
    /// </summary>
    public interface IDbManagerOptions
    {
        /// <summary>
        /// Gets the configured connection string.
        /// </summary>
        /// <returns>
        /// The configured connection string or null if no connection string is used or configured.
        /// </returns>
        string GetConnectionString ();
    }
}

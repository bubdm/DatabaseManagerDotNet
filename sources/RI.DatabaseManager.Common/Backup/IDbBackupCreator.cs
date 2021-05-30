using System;
using System.Data.Common;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Backup
{
    /// <summary>
    ///     Database backup creator to create and (optionally) restore backups.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Database backup creators are used to create backups of databases and, optionally, restore them.
    ///         What the backup and/or restore does in detail depends on the database type and the implementation of
    ///         <see cref="IDbBackupCreator" />.
    ///     </para>
    ///     <para>
    ///         Implementations of <see cref="IDbBackupCreator" /> are always specific for a particular type of database (or
    ///         particular implementation of <see cref="IDbManager" /> respectively).
    ///     </para>
    ///     <note type="note">
    ///         Database backup creators are optional.
    ///         If not configured, backup and restore are not available / not supported.
    ///     </note>
    ///     <note type="note">
    ///         <see cref="IDbBackupCreator" /> implementations are used by database managers.
    ///         Do not use <see cref="IDbBackupCreator" /> implementations directly.
    ///     </note>
    /// </remarks>
    public interface IDbBackupCreator
    {
        /// <summary>
        ///     Gets whether this database backup creator supports backup.
        /// </summary>
        /// <value>
        ///     true if backup is supported, false otherwise.
        /// </value>
        bool SupportsBackup { get; }

        /// <summary>
        ///     Gets whether this database backup creator supports restore.
        /// </summary>
        /// <value>
        ///     true if restore is supported, false otherwise.
        /// </value>
        bool SupportsRestore { get; }

        /// <summary>
        ///     Creates a backup of a database to a specified target.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="backupTarget"> The backup creator specific object which describes the backup target. </param>
        /// <returns>
        ///     true if the backup was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> is null. </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="backupTarget" /> is of a type which is not supported by the this
        ///     backup creator.
        /// </exception>
        bool Backup (IDbManager manager, object backupTarget);

        /// <summary>
        ///     Restores a backup of a database from a specified source.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="backupSource"> The backup creator specific object which describes the backup source. </param>
        /// <returns>
        ///     true if the restore was successful, false otherwise.
        ///     Details about failures should be written to logs.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="manager" /> is null. </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="backupSource" /> is of a type which is not supported by the this
        ///     backup creator.
        /// </exception>
        bool Restore (IDbManager manager, object backupSource);
    }

    /// <inheritdoc cref="IDbBackupCreator" />
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    public interface IDbBackupCreator <TConnection, TTransaction, TParameterTypes> : IDbBackupCreator
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <inheritdoc cref="IDbBackupCreator.Backup" />
        bool Backup (IDbManager<TConnection, TTransaction, TParameterTypes> manager, object backupTarget);

        /// <inheritdoc cref="IDbBackupCreator.Restore" />
        bool Restore (IDbManager<TConnection, TTransaction, TParameterTypes> manager, object backupSource);
    }
}

using System;
using System.Data.Common;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Event arguments for the <see cref="IDatabaseManager.ConnectionCreated" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public abstract class DatabaseConnectionCreatedEventArgs : EventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseConnectionCreatedEventArgs" />.
        /// </summary>
        /// <param name="connection"> The connection which has been created. </param>
        /// <param name="readOnly"> Indicates whether the connection is read-only. </param>
        /// <param name="tracked"> Indicates whether the connection is going to be tracked. </param>
        internal DatabaseConnectionCreatedEventArgs (DbConnection connection, bool readOnly, bool tracked)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.Connection = connection;
            this.ReadOnly = readOnly;
            this.Tracked = tracked;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the connection which has been created.
        /// </summary>
        /// <value>
        ///     The connection which has been created.
        /// </value>
        public DbConnection Connection { get; }

        /// <summary>
        ///     Gets whether the connection is read-only.
        /// </summary>
        /// <value>
        ///     true if the connection is read-only, false otherwise.
        /// </value>
        public bool ReadOnly { get; }

        /// <summary>
        ///     Gets whether the connection is tracked.
        /// </summary>
        /// <value>
        ///     true if the connection is tracked, false otherwise.
        /// </value>
        public bool Tracked { get; }

        #endregion
    }

    /// <summary>
    ///     Event arguments for the <see cref="IDatabaseManager{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}.ConnectionCreated" /> event.
    /// </summary>
    /// <typeparam name="TConnection"> The type of database connections. </typeparam>
    [Serializable]
    public sealed class DatabaseConnectionCreatedEventArgs <TConnection> : DatabaseConnectionCreatedEventArgs
        where TConnection : DbConnection
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseConnectionCreatedEventArgs{TConnection}" />.
        /// </summary>
        /// <param name="connection"> The connection which has been created. </param>
        /// <param name="readOnly"> Indicates whether the connection is read-only. </param>
        /// <param name="tracked"> Indicates whether the connection is going to be tracked. </param>
        public DatabaseConnectionCreatedEventArgs (TConnection connection, bool readOnly, bool tracked)
            : base(connection, readOnly, tracked)
        {
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the connection which has been created.
        /// </summary>
        /// <value>
        ///     The connection which has been created.
        /// </value>
        public new TConnection Connection => (TConnection)base.Connection;

        #endregion
    }
}

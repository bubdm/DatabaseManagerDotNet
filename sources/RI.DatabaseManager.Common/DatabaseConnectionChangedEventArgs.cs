using System;
using System.Data;
using System.Data.Common;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Event arguments for the <see cref="IDatabaseManager.ConnectionChanged" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public abstract class DatabaseConnectionChangedEventArgs : EventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseConnectionChangedEventArgs" />.
        /// </summary>
        /// <param name="connection"> The connection which changed its state. </param>
        /// <param name="oldState"> The old state of the connection. </param>
        /// <param name="newState"> The new state of the connection. </param>
        internal DatabaseConnectionChangedEventArgs (DbConnection connection, ConnectionState oldState, ConnectionState newState)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.Connection = connection;
            this.OldState = oldState;
            this.NewState = newState;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the connection which changed its state.
        /// </summary>
        /// <value>
        ///     The connection which changed its state.
        /// </value>
        public DbConnection Connection { get; }

        /// <summary>
        ///     Gets the new state of the connection.
        /// </summary>
        /// <value>
        ///     The new state of the connection.
        /// </value>
        public ConnectionState NewState { get; }


        /// <summary>
        ///     Gets the old state of the connection.
        /// </summary>
        /// <value>
        ///     The old state of the connection.
        /// </value>
        public ConnectionState OldState { get; }

        #endregion
    }

    /// <summary>
    ///     Event arguments for the <see cref="IDatabaseManager{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}.ConnectionChanged" /> event.
    /// </summary>
    /// <typeparam name="TConnection"> The type of database connections. </typeparam>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public sealed class DatabaseConnectionChangedEventArgs <TConnection> : DatabaseConnectionChangedEventArgs
        where TConnection : DbConnection
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DatabaseConnectionChangedEventArgs" />.
        /// </summary>
        /// <param name="connection"> The connection which changed its state. </param>
        /// <param name="oldState"> The old state of the connection. </param>
        /// <param name="newState"> The new state of the connection. </param>
        public DatabaseConnectionChangedEventArgs (TConnection connection, ConnectionState oldState, ConnectionState newState)
            : base(connection, oldState, newState)
        {
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the connection which changed its state.
        /// </summary>
        /// <value>
        ///     The connection which changed its state.
        /// </value>
        public new TConnection Connection => (TConnection)base.Connection;

        #endregion
    }
}

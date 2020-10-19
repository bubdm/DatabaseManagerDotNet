using System;




namespace RI.DatabaseManager.Builder
{
    public sealed class SQLiteDbManagerOptions : ICloneable
    {
        public string ConnectionString { get; set; }

        /// <inheritdoc />
        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        public SQLiteDbManagerOptions Clone ()
        {

        }
    }
}

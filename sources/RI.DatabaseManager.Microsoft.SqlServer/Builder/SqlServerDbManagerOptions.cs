using System;




namespace RI.DatabaseManager.Builder
{
    public sealed class SqlServerDbManagerOptions : ICloneable
    {
        public string ConnectionString { get; set; }

        /// <inheritdoc />
        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        public SqlServerDbManagerOptions Clone ()
        {

        }
    }
}

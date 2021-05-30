using System;

using Microsoft.Extensions.DependencyInjection;

using RI.Abstractions.Builder;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="DbManagerBuilder" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class ServiceCollectionDbManagerBuilderExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Creates a new database manager builder and uses the given Service Collection as the composition container.
        /// </summary>
        /// <param name="services"> The Service Collection to use. </param>
        /// <returns> The database manager builder used to further configure and build the database manager. </returns>
        /// <remarks>
        ///     <note type="important">
        ///         <see cref="AddDbManager" /> does not yet configure or build any services.
        ///         It just prepares the builder for use in further configuration/build steps.
        ///         Services are only added to the Service Collection and usable after <see cref="IBuilder.Build" /> is called on
        ///         the returned <see cref="DbManagerBuilder" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="services" /> is null. </exception>
        public static IDbManagerBuilder AddDbManager (this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            DbManagerBuilder builder = new DbManagerBuilder();
            builder.UseServiceCollection(services);
            return builder;
        }

        #endregion
    }
}

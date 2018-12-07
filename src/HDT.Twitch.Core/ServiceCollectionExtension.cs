using Microsoft.Extensions.DependencyInjection;

namespace HDT.Twitch.Core
{
    /// <summary>
    /// Class ServiceCollectionExtension.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds the bot core.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddBotCore(this IServiceCollection services)
        {
            services.AddSingleton<IClient, Client>();
            return services;
        }
    }
}

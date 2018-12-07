namespace HDT.Twitch.Core.Modules
{
    /// <summary>
    /// Class ModuleExtensions.
    /// </summary>
    public static class ModuleExtensions
    {
        /// <summary>
        /// Usings the modules.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>Client.</returns>
        public static Client UsingModules(this Client client)
        {
            client.AddService(new ModuleService());
            return client;
        }

        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public static void AddModule(this Client client, IModule instance, string name = null)
        {
            client.GetService<ModuleService>().Add(instance, name);
        }
        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client">The client.</param>
        /// <param name="name">The name.</param>
        public static void AddModule<T>(this Client client, string name = null)
            where T : class, IModule, new()
        {
            client.GetService<ModuleService>().Add<T>(name);
        }
        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client">The client.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public static void AddModule<T>(this Client client, T instance, string name = null)
            where T : class, IModule
        {
            client.GetService<ModuleService>().Add(instance, name);
        }
        /// <summary>
        /// Gets the module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client">The client.</param>
        /// <returns>ModuleManager&lt;T&gt;.</returns>
        public static ModuleManager<T> GetModule<T>(this Client client)
            where T : class, IModule
            => client.GetService<ModuleService>().Get<T>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HDT.Twitch.Core.Modules
{
    /// <summary>
    /// Class ModuleService.
    /// Implements the <see cref="HDT.Twitch.Core.IService" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Core.IService" />
    public class ModuleService : IService
    {
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        public Client Client { get; private set; }

        /// <summary>
        /// The add method
        /// </summary>
        private static readonly MethodInfo AddMethod = typeof(ModuleService).GetTypeInfo().GetDeclaredMethods(nameof(Add)).SingleOrDefault(x => x.IsGenericMethodDefinition && x.GetParameters().Length == 3);

        /// <summary>
        /// Gets the modules.
        /// </summary>
        /// <value>The modules.</value>
        public IEnumerable<ModuleManager> Modules => _modules.Values;
        /// <summary>
        /// The modules
        /// </summary>
        private readonly Dictionary<Type, ModuleManager> _modules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleService"/> class.
        /// </summary>
        public ModuleService()
        {
            _modules = new Dictionary<Type, ModuleManager>();
        }

        /// <summary>
        /// Installs the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        void IService.Install(Client client)
        {
            Client = client;
        }

        /// <summary>
        /// Adds the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void Add(IModule instance, string name)
        {
            Type type = instance.GetType();
            AddMethod.MakeGenericMethod(type).Invoke(this, new object[] { instance, name });
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        public void Add<T>(string name)
            where T : class, IModule, new()
            => Add(new T(), name);

        /// <summary>
        /// Adds the specified instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">instance</exception>
        /// <exception cref="InvalidOperationException">
        /// Service needs to be added to a TwitchClient before modules can be installed.
        /// or
        /// This module has already been added.
        /// </exception>
        public void Add<T>(T instance, string name)
            where T : class, IModule
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (Client == null)
                throw new InvalidOperationException("Service needs to be added to a TwitchClient before modules can be installed.");

            Type type = typeof(T);
            if (name == null) name = type.Name;
            if (_modules.ContainsKey(type))
                throw new InvalidOperationException("This module has already been added.");

            ModuleManager<T> manager = new ModuleManager<T>(Client, instance, name);
            _modules.Add(type, manager);
            instance.Install(manager);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ModuleManager&lt;T&gt;.</returns>
        public ModuleManager<T> Get<T>()
            where T : class, IModule
        {
            if (_modules.TryGetValue(typeof(T), out ModuleManager manager))
                return manager as ModuleManager<T>;
            return null;
        }
    }
}

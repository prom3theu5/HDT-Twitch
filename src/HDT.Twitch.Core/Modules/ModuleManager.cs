using HDT.Twitch.Core.Commands;
using Nito.AsyncEx;
using System;

namespace HDT.Twitch.Core.Modules
{
    /// <summary>
    /// Class ModuleManager.
    /// Implements the <see cref="HDT.Twitch.Core.Modules.ModuleManager" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="HDT.Twitch.Core.Modules.ModuleManager" />
    public class ModuleManager<T> : ModuleManager
        where T : class, IModule
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public new T Instance => base.Instance as T;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleManager{T}"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        internal ModuleManager(Client client, T instance, string name)
            : base(client, instance, name)
        {
        }
    }

    /// <summary>
    /// Class ModuleManager.
    /// Implements the <see cref="HDT.Twitch.Core.Modules.ModuleManager" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Core.Modules.ModuleManager" />
    public class ModuleManager
    {
        /// <summary>
        /// The lock
        /// </summary>
        private readonly AsyncLock _lock;

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        public Client Client { get; }
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public IModule Instance { get; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleManager"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        internal ModuleManager(Client client, IModule instance, string name)
        {
            Client = client;
            Instance = instance;
            Name = name;

            Id = name.ToLowerInvariant();
            _lock = new AsyncLock();
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="config">The configuration.</param>
        public void CreateCommands(string prefix, Action<CommandGroupBuilder> config)
        {
            CommandService commandService = Client.GetService<CommandService>();
            commandService.CreateGroup(prefix, x =>
            {
                x.Category(Name);
                config(x);
            });

        }
    }
}

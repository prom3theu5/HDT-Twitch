using HDT.Twitch.Core;
using HDT.Twitch.Core.Modules;
using Serilog;
using System.Collections.Generic;

namespace HDT.Twitch.Commands
{
    /// <summary>
    /// Class ChannelModule.
    /// Implements the <see cref="HDT.Twitch.Core.Modules.IModule" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Core.Modules.IModule" />
    public abstract class ChannelModule : IModule
    {
        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>The commands.</value>
        public HashSet<ChannelCommand> Commands { get; private set; }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public abstract string Prefix { get; }

        /// <summary>
        /// Installs the specified manager.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public abstract void Install(ModuleManager manager);

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        public IClient Client { get; }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelModule" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="logger">The logger.</param>
        protected ChannelModule(IClient client, ILogger logger)
        {
            Commands = new HashSet<ChannelCommand>();
            Client = client;
            Logger = logger;
        }
    }
}

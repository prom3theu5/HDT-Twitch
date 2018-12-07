using HDT.Twitch.Core;
using HDT.Twitch.Core.Modules;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
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
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        public IServiceProvider Services { get; }
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
        /// <param name="services">The services.</param>
        protected ChannelModule(IServiceProvider services)
        {
            Services = services;
            Commands = new HashSet<ChannelCommand>();
            Client = services.GetRequiredService<IClient>();
            Logger = services.GetRequiredService<ILogger>();
        }
    }
}

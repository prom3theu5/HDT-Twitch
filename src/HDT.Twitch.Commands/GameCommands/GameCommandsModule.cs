using HDT.Twitch.Commands.GameCommands.Commands;
using HDT.Twitch.Core.Extensions;
using HDT.Twitch.Core.Modules;
using System;

namespace HDT.Twitch.Commands.GameCommands
{
    /// <summary>
    /// Class GameCommandsModule.
    /// Implements the <see cref="DocBot.Commands.ChannelModule" />
    /// </summary>
    /// <seealso cref="DocBot.Commands.ChannelModule" />
    public class GameCommandsModule : ChannelModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameCommandsModule"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        public GameCommandsModule(IServiceProvider services) : base(services)
        {
            Commands.Add(new ArenaCommand(this));
            Commands.Add(new BestDeckCommand(this));
            Commands.Add(new DeckCommand(this));
            Commands.Add(new HDTCommands(this));
            Commands.Add(new LastReplayCommand(this));
            Commands.Add(new MostPlayedCommand(this));
            Commands.Add(new StatsCommand(this));
        }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public override string Prefix { get; } = "!";

        /// <summary>
        /// Installs the specified manager.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {
                Commands.ForEach(cmd => cmd.Init(cgb));
            });
        }
    }
}


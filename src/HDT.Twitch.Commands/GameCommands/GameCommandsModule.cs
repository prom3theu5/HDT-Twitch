using HDT.Twitch.Commands.GameCommands.Commands;
using HDT.Twitch.Core;
using HDT.Twitch.Core.Extensions;
using HDT.Twitch.Core.Modules;
using Serilog;

namespace HDT.Twitch.Commands.GameCommands
{
    /// <summary>
    /// Class GameCommandsModule.
    /// Implements the <see cref="HDT.Twitch.Commands.ChannelModule" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Commands.ChannelModule" />
    public class GameCommandsModule : ChannelModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameCommandsModule" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="logger">The logger.</param>
        public GameCommandsModule(IClient client, ILogger logger) : base(client, logger)
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


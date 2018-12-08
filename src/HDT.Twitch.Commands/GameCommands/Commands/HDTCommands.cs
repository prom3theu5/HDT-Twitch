using HDT.Twitch.Core;
using HDT.Twitch.Core.Commands;
using System.Threading.Tasks;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    /// <summary>
    /// Class HDTCommands.
    /// Implements the <see cref="HDT.Twitch.Commands.ChannelCommand" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Commands.ChannelCommand" />
    public class HDTCommands : ChannelCommand
    {
        /// <summary>
        /// Creates a new instance of twitch command,
        /// use ": base(module)" in the derived class'
        /// constructor to make sure module is assigned
        /// </summary>
        /// <param name="module">Module this command resides in</param>
        public HDTCommands(ChannelModule module) : base(module)
        {
        }

        /// <summary>
        /// Initializes the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "hdt")
                .Description("Get The Download Link for HDT")
                .Do(DeckTracker);

            builder.CreateCommand(Module.Prefix + "hsreplay")
                .Description("Get The link to HSReplay")
                .Do(HsReplay);

            builder.CreateCommand(Module.Prefix + "commands")
                .Description("Get The Commands List")
                .Do(Commands);
        }

        private Task DeckTracker(CommandEventArgs arg)
        {
            if (!Config.Instance.ChatCommandHdt) return Task.CompletedTask;
            if (Config.Instance.ModeratorOnly)
            {
                if (!arg.IsAdmin) return Task.CompletedTask;
            }
            Module.Client.SendMessage("Hearthstone Deck Tracker: http://hsdecktracker.net");
            return Task.CompletedTask;
        }

        private Task HsReplay(CommandEventArgs arg)
        {
            if (!Config.Instance.ChatCommandHsr) return Task.CompletedTask;
            if (Config.Instance.ModeratorOnly)
            {
                if (!arg.IsAdmin) return Task.CompletedTask;
            }
            Module.Client.SendMessage("HSReplay: http://hsreplay.net");
            return Task.CompletedTask;
        }

        private Task Commands(CommandEventArgs arg)
        {
            if (!Config.Instance.ChatCommandCommands) return Task.CompletedTask;
            if (Config.Instance.ModeratorOnly)
            {
                if (!arg.IsAdmin) return Task.CompletedTask;
            }
            Module.Client.SendMessage("List of available commands: https://github.com/prom3theu5/HDT-Twitch/wiki/Commands");
            return Task.CompletedTask;
        }
    }
}

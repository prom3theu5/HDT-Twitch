using HDT.Twitch.Core.Commands;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    public class HDTCommands : ChannelCommand
    {
        public HDTCommands(ChannelModule module) : base(module)
        {
        }

        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "hdt")
                .Description("Get The Download Link for HDT")
                .Do(x =>
                {
                    Module.Client.SendMessage("Hearthstone Deck Tracker: http://hsdecktracker.net");
                });

            builder.CreateCommand(Module.Prefix + "hsreplay")
                .Description("Get The link to HSReplay")
                .Do(x =>
                {
                    Module.Client.SendMessage("HSReplay: http://hsreplay.net");
                });

            builder.CreateCommand(Module.Prefix + "commands")
                .Description("Get The Commands List")
                .Do(x =>
                {
                    Module.Client.SendMessage("List of available commands: https://github.com/prom3theu5/HDT-Twitch/wiki/Commands");
                });
        }
    }
}

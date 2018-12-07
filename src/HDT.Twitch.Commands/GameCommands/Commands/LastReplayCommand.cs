using HDT.Twitch.Core.Commands;
using Hearthstone_Deck_Tracker.Stats;
using System.Linq;
using System.Threading.Tasks;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    public class LastReplayCommand : ChannelCommand
    {
        private const string CurrentlySupportedDecktypes = "[currently only supported for constructed decks]";

        public LastReplayCommand(ChannelModule module) : base(module)
        {
        }

        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "lastgamereplay")
                .Description("Get The Link To The Last Replay")
                .Do(DoGetReplay());
        }

        private Task DoGetReplay()
        {
            GameStats game = LastGames.Instance.Games.FirstOrDefault();
            if (game != null)
            {
                Module.Client.SendMessage(game.HsReplay.Uploaded
                    ? $"A replay link to the last played game can be found here: {game.HsReplay.ReplayUrl}"
                    : "Game URL was not found. Replay might not have uploaded yet, or Replays are disabled. Please try again later.");
            }
            else
            {
                Module.Client.SendMessage("A game has not been played yet - Oops!");
            }

            return Task.CompletedTask;
        }
    }
}
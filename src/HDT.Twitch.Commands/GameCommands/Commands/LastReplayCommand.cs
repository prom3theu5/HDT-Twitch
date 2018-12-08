using HDT.Twitch.Core;
using HDT.Twitch.Core.Commands;
using Hearthstone_Deck_Tracker.Stats;
using System.Linq;
using System.Threading.Tasks;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    /// <summary>
    /// Class LastReplayCommand.
    /// Implements the <see cref="HDT.Twitch.Commands.ChannelCommand" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Commands.ChannelCommand" />
    public class LastReplayCommand : ChannelCommand
    {
        /// <summary>
        /// The currently supported decktypes
        /// </summary>
        private const string CurrentlySupportedDecktypes = "[currently only supported for constructed decks]";

        /// <summary>
        /// Creates a new instance of twitch command,
        /// use ": base(module)" in the derived class'
        /// constructor to make sure module is assigned
        /// </summary>
        /// <param name="module">Module this command resides in</param>
        public LastReplayCommand(ChannelModule module) : base(module)
        {
        }

        /// <summary>
        /// Initializes the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "lastgamereplay")
                .Description("Get The Link To The Last Replay")
                .Do(DoGetReplay);
        }

        /// <summary>
        /// Does the get replay.
        /// </summary>
        private Task DoGetReplay(CommandEventArgs parameters)
        {
            if (!Config.Instance.ChatCommandHdt) return Task.CompletedTask;
            if (Config.Instance.ModeratorOnly)
            {
                if (!parameters.IsAdmin) return Task.CompletedTask;
            }

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
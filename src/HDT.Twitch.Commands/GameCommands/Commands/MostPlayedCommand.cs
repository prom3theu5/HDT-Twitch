using HDT.Twitch.Core.Commands;
using HDT.Twitch.Core.Extensions;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Config = HDT.Twitch.Core.Config;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    /// <summary>
    /// Class MostPlayedCommand.
    /// Implements the <see cref="HDT.Twitch.Commands.ChannelCommand" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Commands.ChannelCommand" />
    public class MostPlayedCommand : ChannelCommand
    {
        /// <summary>
        /// The missing time frame message
        /// </summary>
        private const string MissingTimeFrameMessage =
            "Please specify a timeframe. Available timeframes are: today, week, season and total. (Example: !stats today)";

        /// <summary>
        /// Creates a new instance of twitch command,
        /// use ": base(module)" in the derived class'
        /// constructor to make sure module is assigned
        /// </summary>
        /// <param name="module">Module this command resides in</param>
        public MostPlayedCommand(ChannelModule module) : base(module)
        {
        }

        /// <summary>
        /// Initializes the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "bestdeck")
                .Description("Get The Best Deck For the Specified Timeframe")
                .Parameter("TimeFrame", ParameterType.Required)
                .Do(DoPostMostPlayed);
        }

        /// <summary>
        /// Does the post most played.
        /// </summary>
        /// <param name="parameters">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
        /// <returns>Task.</returns>
        private Task DoPostMostPlayed(CommandEventArgs parameters)
        {
            if (!Config.Instance.ChatCommandHdt) return Task.CompletedTask;
            if (Config.Instance.ModeratorOnly)
            {
                if (!parameters.IsAdmin) return Task.CompletedTask;
            }

            if (!parameters.Args.Any())
            {
                Module.Client.SendMessage(MissingTimeFrameMessage);
                return Task.CompletedTask;
            }

            string arg = parameters.Args[0];

            var decks =
                DeckList.Instance.Decks.Where(d => !d.IsArenaDeck)
                    .Select(d => new { Deck = d, Games = d.DeckStats.Games.Where(arg.TimeFrameFilter()) });
            var mostPlayed = decks.Where(d => d.Games.Any()).OrderByDescending(d => d.Games.Count()).FirstOrDefault();
            string timeFrame = arg == "today" || arg == "total" ? arg : "this " + arg;
            if (mostPlayed == null)
            {
                Module.Client.SendMessage("No games played " + timeFrame);
                return Task.CompletedTask;
            }
            int wins = mostPlayed.Games.Count(g => g.Result == GameResult.Win);
            int losses = mostPlayed.Games.Count(g => g.Result == GameResult.Loss);
            double winRate = Math.Round(100.0 * wins / (wins + losses), 0);
            Module.Client.SendMessage($"Most played deck {timeFrame}: \"{mostPlayed.Deck.Name}\", Win-Rate: {winRate}% ({wins}-{losses}), Deck Import Code: {mostPlayed.Deck.GetHearthstoneDeckCode()}");

            return Task.CompletedTask;
        }
    }
}
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
    /// Class BestDeckCommand.
    /// Implements the <see cref="HDT.Twitch.Commands.ChannelCommand" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Commands.ChannelCommand" />
    public class BestDeckCommand : ChannelCommand
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
        public BestDeckCommand(ChannelModule module) : base(module)
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
                .Do(DoPostStats);
        }

        /// <summary>
        /// Does the post stats.
        /// </summary>
        /// <param name="parameters">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
        /// <returns>Task.</returns>
        private Task DoPostStats(CommandEventArgs parameters)
        {
            if (!Config.Instance.ChatCommandBestDeckGeneral) return Task.CompletedTask;
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
            var stats =
                decks.Select(
                        d =>
                            new
                            {
                                DeckObj = d,
                                Wins = d.Games.Count(g => g.Result == GameResult.Win),
                                Losses = (d.Games.Count(g => g.Result == GameResult.Loss))
                            })
                    .Where(d => d.Wins + d.Losses > Config.Instance.BestDeckGamesThreshold)
                    .OrderByDescending(d => (double)d.Wins / (d.Wins + d.Losses));
            var best = stats.FirstOrDefault();
            string timeFrame = arg == "today" || arg == "total" ? arg : "this " + arg;
            if (best == null)
            {
                if (Config.Instance.BestDeckGamesThreshold > 1)
                    Module.Client.SendMessage($"Not enough games played {timeFrame} (min: {Config.Instance.BestDeckGamesThreshold})");
                else
                    Module.Client.SendMessage("No games played " + timeFrame);
                return Task.CompletedTask;
            }
            double winRate = Math.Round(100.0 * best.Wins / (best.Wins + best.Losses), 0);
            Module.Client.SendMessage($"Best deck {timeFrame}: \"{best.DeckObj.Deck.Name}\", Win-Rate: {winRate}% ({best.Wins}-{best.Losses}), Deck Import Code: {best.DeckObj.Deck.GetHearthstoneDeckCode()}");

            return Task.CompletedTask;
        }
    }
}
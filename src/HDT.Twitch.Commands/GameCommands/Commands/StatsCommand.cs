using HDT.Twitch.Core;
using HDT.Twitch.Core.Commands;
using HDT.Twitch.Core.Extensions;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    /// <summary>
    /// Class StatsCommand.
    /// Implements the <see cref="HDT.Twitch.Commands.ChannelCommand" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Commands.ChannelCommand" />
    public class StatsCommand : ChannelCommand
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
        public StatsCommand(ChannelModule module) : base(module)
        {
        }

        /// <summary>
        /// Initializes the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "stats")
                .Description("Get The Current Selected Deck")
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

            List<GameStats> games = DeckStatsList.Instance.DeckStats.SelectMany(ds => ds.Value.Games).Where(arg.TimeFrameFilter()).ToList();
            int numGames = games.Count;
            string timeFrame = arg == "today" || arg == "total" ? arg : "this " + arg;
            if (numGames == 0)
            {
                Module.Client.SendMessage($"No games played {timeFrame}.");
                return Task.CompletedTask;
            }
            int numDecks = games.Select(g => g.DeckId).Distinct().Count();
            int wins = games.Count(g => g.Result == GameResult.Win);
            double winRate = Math.Round(100.0 * wins / numGames);
            Module.Client.SendMessage($"Played {numGames} games with {numDecks} decks {timeFrame}. Total stats: {wins}-{numGames - wins} ({winRate}%)");

            return Task.CompletedTask;
        }
    }
}
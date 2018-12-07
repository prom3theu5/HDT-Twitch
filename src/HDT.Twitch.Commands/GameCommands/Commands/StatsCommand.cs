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
    public class StatsCommand : ChannelCommand
    {
        private const string MissingTimeFrameMessage =
            "Please specify a timeframe. Available timeframes are: today, week, season and total. (Example: !stats today)";

        public StatsCommand(ChannelModule module) : base(module)
        {
        }

        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "stats")
                .Description("Get The Current Selected Deck")
                .Parameter("TimeFrame", ParameterType.Required)
                .Do(DoPostStats);
        }

        private Task DoPostStats(CommandEventArgs parameters)
        {
            if (!parameters.Args.Any())
            {
                Client.SendMessage(Module.Client.Channel, MissingTimeFrameMessage);
                return Task.CompletedTask;
            }

            string arg = parameters.Args[0];

            List<GameStats> games = DeckStatsList.Instance.DeckStats.SelectMany(ds => ds.Value.Games).Where(arg.TimeFrameFilter()).ToList();
            int numGames = games.Count;
            string timeFrame = arg == "today" || arg == "total" ? arg : "this " + arg;
            if (numGames == 0)
            {
                Client.SendMessage(Module.Client.Channel, $"No games played {timeFrame}.");
                return Task.CompletedTask;
            }
            int numDecks = games.Select(g => g.DeckId).Distinct().Count();
            int wins = games.Count(g => g.Result == GameResult.Win);
            double winRate = Math.Round(100.0 * wins / numGames);
            Client.SendMessage(Module.Client.Channel, $"Played {numGames} games with {numDecks} decks {timeFrame}. Total stats: {wins}-{numGames - wins} ({winRate}%)");

            return Task.CompletedTask;
        }
    }
}
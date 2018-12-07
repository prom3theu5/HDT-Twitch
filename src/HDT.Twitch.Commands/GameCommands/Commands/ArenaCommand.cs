using HDT.Twitch.Core.Commands;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    public class ArenaCommand : ChannelCommand
    {
        private const string MissingTimeFrameMessage =
            "Please specify a timeframe. Available timeframes are: today, week, season and total. (Example: !stats today)";

        public ArenaCommand(ChannelModule module) : base(module)
        {
        }

        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "arena")
                .Description("Get The Current Arena Deck")
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

            List<Hearthstone_Deck_Tracker.Hearthstone.Deck> arenaRuns = DeckList.Instance.Decks.Where(d => d.IsArenaDeck).ToList();
            switch (arg)
            {
                case "today":
                    arenaRuns = arenaRuns.Where(g => g.LastPlayed.Date == DateTime.Today).ToList();
                    break;
                case "week":
                    arenaRuns = arenaRuns.Where(g => g.LastPlayed.Date > DateTime.Today.AddDays(-7)).ToList();
                    break;
                case "season":
                    arenaRuns =
                        arenaRuns.Where(g => g.LastPlayed.Date.Year == DateTime.Today.Year && g.LastPlayed.Date.Month == DateTime.Today.Month).ToList();
                    break;
                case "total":
                    break;
                default:
                    return Task.CompletedTask;
            }
            string timeFrame = arg == "today" || arg == "total" ? arg : "this " + arg;
            if (!arenaRuns.Any())
            {
                Module.Client.SendMessage($"No arena runs {timeFrame}.");
                return Task.CompletedTask;
            }
            var ordered =
                arenaRuns.Select(run => new { Run = run, Wins = run.DeckStats.Games.Count(g => g.Result == GameResult.Win) })
                         .OrderByDescending(x => x.Wins);
            var best = ordered.Where(run => run.Wins == ordered.First().Wins).ToList();
            var classesObj = best.Select(x => x.Run.Class).Distinct().Select(x => new { Class = x, Count = best.Count(c => c.Run.Class == x) });
            string classes =
                classesObj.Select(x => x.Class + (x.Count > 1 ? $" (x{x.Count})" : "")).Aggregate((c, n) => c + ", " + n);
            Module.Client.SendMessage($"Best arena run {timeFrame}: {best.First().Run.WinLossString} with {classes}");

            return Task.CompletedTask;
        }
    }
}
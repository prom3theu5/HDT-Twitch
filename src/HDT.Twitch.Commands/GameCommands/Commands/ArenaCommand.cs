using HDT.Twitch.Core.Commands;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Config = HDT.Twitch.Core.Config;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    /// <summary>
    /// Class ArenaCommand.
    /// Implements the <see cref="HDT.Twitch.Commands.ChannelCommand" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Commands.ChannelCommand" />
    public class ArenaCommand : ChannelCommand
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
        public ArenaCommand(ChannelModule module) : base(module)
        {
        }

        /// <summary>
        /// Initializes the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "arena")
                .Description("Get The Current Arena Deck")
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
            if (!Config.Instance.ChatCommandArenaGeneral) return Task.CompletedTask;
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
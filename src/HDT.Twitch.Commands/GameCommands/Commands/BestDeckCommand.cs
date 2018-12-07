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
    public class BestDeckCommand : ChannelCommand
    {
        private const string MissingTimeFrameMessage =
            "Please specify a timeframe. Available timeframes are: today, week, season and total. (Example: !stats today)";

        public BestDeckCommand(ChannelModule module) : base(module)
        {
        }

        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "bestdeck")
                .Description("Get The Best Deck For the Specified Timeframe")
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
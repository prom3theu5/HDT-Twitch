using Hearthstone_Deck_Tracker.Stats;
using System;

namespace HDT.Twitch.Core.Extensions
{
    public static class TimeFrame
    {
        public static Func<GameStats, bool> TimeFrameFilter(this string timeFrame)
        {
            switch (timeFrame)
            {
                case "today":
                    return game => game.StartTime.Date == DateTime.Today;
                case "week":
                    return game => game.StartTime > DateTime.Today.AddDays(-7);
                case "season":
                    return game => game.StartTime.Date.Year == DateTime.Today.Year && game.StartTime.Date.Month == DateTime.Today.Month;
                case "total":
                    return game => true;
                default:
                    return game => false;
            }
        }
    }
}

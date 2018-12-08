using Hearthstone_Deck_Tracker;
using System.IO;

namespace HDT.Twitch.Core
{
    public class Config
    {
        private static Config _instance;
        public static string[] TimeFrames => new[] { "today", "week", "season", "total" };
        public static Config Instance => _instance ?? Load();

        public string User { get; set; }
        public string OAuth { get; set; }
        public string Channel { get; set; }
        public bool AutoPostGameResult { get; set; } = true;
        public int AutoPostDelay { get; set; } = 0;
        public bool ChatCommandCommands { get; set; } = true;
        public bool ChatCommandDeck { get; set; } = true;
        public bool ChatCommandAllDecks { get; set; } = true;
        public bool ChatCommandHdt { get; set; } = true;
        public bool ChatCommandHsr { get; set; } = true;
        public bool ChatCommandLastGameReplay { get; set; } = true;
        public bool ChatCommandStatsGeneral { get; set; } = true;
        public string ChatCommandStatsDefault { get; set; } = "today";
        public bool ChatCommandStatsToday { get; set; } = true;
        public bool ChatCommandStatsWeek { get; set; } = true;
        public bool ChatCommandStatsSeason { get; set; } = true;
        public bool ChatCommandStatsTotal { get; set; } = true;
        public bool ChatCommandArenaGeneral { get; set; } = true;
        public string ChatCommandArenaDefault { get; set; } = "today";
        public bool ChatCommandArenaToday { get; set; } = true;
        public bool ChatCommandArenaWeek { get; set; } = true;
        public bool ChatCommandArenaSeason { get; set; } = true;
        public bool ChatCommandArenaTotal { get; set; } = true;
        public bool ChatCommandBestDeckGeneral { get; set; } = true;
        public string ChatCommandBestDeckDefault { get; set; } = "today";
        public bool ChatCommandBestDeckToday { get; set; } = true;
        public bool ChatCommandBestDeckWeek { get; set; } = true;
        public bool ChatCommandBestDeckSeason { get; set; } = true;
        public bool ChatCommandBestDeckTotal { get; set; } = true;
        public bool ChatCommandMostPlayedGeneral { get; set; } = true;
        public string ChatCommandMostPlayedDefault { get; set; } = "today";
        public bool ChatCommandMostPlayedToday { get; set; } = true;
        public bool ChatCommandMostPlayedWeek { get; set; } = true;
        public bool ChatCommandMostPlayedSeason { get; set; } = true;
        public bool ChatCommandMostPlayedTotal { get; set; } = true;
        public int BestDeckGamesThreshold { get; set; } = 3;
        public bool SaveStatsToFile { get; set; } = true;
        public string StatsFileDir { get; set; } = Hearthstone_Deck_Tracker.Config.Instance.DataDir;
        public string StatsFileName { get; set; } = "hdt_activedeck_stats.txt";
        public string StatsFileFullPath => Path.Combine(StatsFileDir, StatsFileName);
        public bool TwitchLogging { get; set; } = true;
        public bool ModeratorOnly { get; set; } = false;

        private static string FilePath => Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.ConfigDir, "Twitch.xml");

        public static T GetConfigItem<T>(string name)
        {
            object prop = Instance.GetType().GetProperty(name).GetValue(Instance, null);
            if (prop == null)
                return default(T);
            return (T)prop;
        }

        public static void Save() => XmlManager<Config>.Save(FilePath, Instance);

        private static Config Load() => _instance = File.Exists(FilePath) ? XmlManager<Config>.Load(FilePath) : new Config();
    }
}
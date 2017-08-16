﻿#region

using System.IO;
using Hearthstone_Deck_Tracker;

#endregion

namespace Twitch
{
	public class Config
	{
		private static Config _instance;

		public static string[] TimeFrames => new[] {"today", "week", "season", "total"};

		public Config()
		{
			AutoPostGameResult = true;
			AutoPostDelay = 0;
			ChatCommandCommands = true;
			ChatCommandDeck = true;
			ChatCommandAllDecks = true;
			ChatCommandHdt = true;
            ChatCommandLastGameReplay = true;
            ChatCommandHsr = true;
            ChatCommandStatsGeneral = true;
			ChatCommandStatsDefault = "today";
			ChatCommandStatsToday = true;
			ChatCommandStatsWeek = true;
			ChatCommandStatsSeason = true;
			ChatCommandStatsTotal = true;
			ChatCommandArenaGeneral = true;
			ChatCommandArenaDefault = "today";
			ChatCommandArenaToday = true;
			ChatCommandArenaWeek = true;
			ChatCommandArenaSeason = true;
			ChatCommandArenaTotal = true;
			ChatCommandBestDeckGeneral = true;
			ChatCommandBestDeckDefault = "today";
			ChatCommandBestDeckToday = true;
			ChatCommandBestDeckWeek = true;
			ChatCommandBestDeckSeason = true;
			ChatCommandBestDeckTotal = true;
			ChatCommandMostPlayedGeneral = true;
			ChatCommandMostPlayedDefault = "today";
			ChatCommandMostPlayedToday = true;
			ChatCommandMostPlayedWeek = true;
			ChatCommandMostPlayedSeason = true;
			ChatCommandMostPlayedTotal = true;
			BestDeckGamesThreshold = 3;
			StatsFileName = "hdt_activedeck_stats.txt";
			StatsFileDir = Hearthstone_Deck_Tracker.Config.Instance.DataDir;
		}

		public static Config Instance => _instance ?? Load();

		public string User { get; set; }
		public string OAuth { get; set; }
		public string Channel { get; set; }
		public bool AutoPostGameResult { get; set; }
		public int AutoPostDelay { get; set; }
		public bool ChatCommandCommands { get; set; }
		public bool ChatCommandDeck { get; set; }
		public bool ChatCommandAllDecks { get; set; }
		public bool ChatCommandHdt { get; set; }
        public bool ChatCommandHsr { get; set; }
        public bool ChatCommandLastGameReplay { get; set; }
        public bool ChatCommandStatsGeneral { get; set; }
		public string ChatCommandStatsDefault { get; set; }
		public bool ChatCommandStatsToday { get; set; }
		public bool ChatCommandStatsWeek { get; set; }
		public bool ChatCommandStatsSeason { get; set; }
		public bool ChatCommandStatsTotal { get; set; }
		public bool ChatCommandArenaGeneral { get; set; }
		public string ChatCommandArenaDefault { get; set; }
		public bool ChatCommandArenaToday { get; set; }
		public bool ChatCommandArenaWeek { get; set; }
		public bool ChatCommandArenaSeason { get; set; }
		public bool ChatCommandArenaTotal { get; set; }
		public bool ChatCommandBestDeckGeneral { get; set; }
		public string ChatCommandBestDeckDefault { get; set; }
		public bool ChatCommandBestDeckToday { get; set; }
		public bool ChatCommandBestDeckWeek { get; set; }
		public bool ChatCommandBestDeckSeason { get; set; }
		public bool ChatCommandBestDeckTotal { get; set; }
		public bool ChatCommandMostPlayedGeneral { get; set; }
		public string ChatCommandMostPlayedDefault { get; set; }
		public bool ChatCommandMostPlayedToday { get; set; }
		public bool ChatCommandMostPlayedWeek { get; set; }
		public bool ChatCommandMostPlayedSeason { get; set; }
		public bool ChatCommandMostPlayedTotal { get; set; }
		public int BestDeckGamesThreshold { get; set; }
		public bool SaveStatsToFile { get; set; }
		public string StatsFileDir { get; set; }
		public string StatsFileName { get; set; }
		public string StatsFileFullPath => Path.Combine(StatsFileDir, StatsFileName);
		public bool TwitchLogging { get; set; }

		private static string FilePath => Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.ConfigDir, "Twitch.xml");

		public static T GetConfigItem<T>(string name)
		{
			var prop = Instance.GetType().GetProperty(name).GetValue(Instance, null);
			if(prop == null)
				return default(T);
			return (T)prop;
		}

		public static void Save() => XmlManager<Config>.Save(FilePath, Instance);

		private static Config Load() => _instance = File.Exists(FilePath) ? XmlManager<Config>.Load(FilePath) : new Config();
	}
}
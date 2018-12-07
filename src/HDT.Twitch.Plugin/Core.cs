using HDT.Twitch.Core;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Config = HDT.Twitch.Core.Config;

namespace HDT.Twitch.Plugin
{
    public class Core
    {
        private static readonly IClient _client;
        private static readonly ILogger _logger;
        public static readonly Events.Events Events;

        static Core()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.RollingFile("log-{Date}.txt")
                .CreateLogger();

            _client = new Client(_logger);
            Events = new Events.Events(_client);
        }

        public static string TwitchTag => "Twitch";

        public static bool Connect()
        {
            _logger.Information("Logging in as " + Config.Instance.User);
            _client.Connect();
            return true;
        }

        public static void Disconnect()
        {
            _logger.Information("Disconnecting from Twitch.");
            _client.Disconnect();
        }

        private static string _currentFileContent;
        public static void Update()
        {
            if (!Config.Instance.SaveStatsToFile)
                return;
            if (DeckList.Instance.ActiveDeckVersion == null)
                return;
            List<Hearthstone_Deck_Tracker.Stats.GameStats> games = DeckList.Instance.ActiveDeckVersion.GetRelevantGames();
            int wins = games.Count(g => g.Result == GameResult.Win);
            int losses = games.Count(g => g.Result == GameResult.Loss);
            string resultString = $"{wins} - {losses}";
            if (_currentFileContent == resultString)
                return;
            try
            {
                using (StreamWriter sr = new StreamWriter(Config.Instance.StatsFileFullPath))
                    sr.WriteLine(resultString);
                _currentFileContent = resultString;
            }
            catch (Exception ex)
            {
                _logger.Error("Error writing to stats file: " + ex, "Twitch");
            }
        }
    }
}
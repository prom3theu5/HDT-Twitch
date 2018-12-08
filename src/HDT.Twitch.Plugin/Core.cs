using HDT.Twitch.Commands.GameCommands;
using HDT.Twitch.Core;
using HDT.Twitch.Core.Commands;
using HDT.Twitch.Core.Modules;
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
        private static IClient _client;
        private static readonly ILogger _logger;
        public static Events.Events Events;

        static Core()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.RollingFile(
                    $"{Path.Combine(Hearthstone_Deck_Tracker.Config.AppDataPath, "HDT.Twitch.Plugin-Log-{Date}.txt")}")
                .CreateLogger();

            _client = new Client(_logger);
            Events = new Events.Events(_client);
        }


        public static string TwitchTag => "Twitch";

        public static bool Connect()
        {
            CommandService commandService = new CommandService(new CommandServiceConfigBuilder
            {
                CustomPrefixHandler = m => 0,
                ErrorHandler = (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(e.Exception?.Message))
                        return;
                    try
                    {
                        _logger.Error("Error in Command: {Error}\r\n{inner}\r\n{stack}", e.Exception.Message,
                            e.Exception.InnerException, e.Exception.StackTrace);
                    }
                    catch
                    {
                    }
                },
                ExecuteHandler = (s, e) =>
                {
                    _logger.Information("Command Executed: {Command} - Args: {Args} - User: {UserId}:{User}",
                        e.Command.Text, e.Args, e.Message.UserId, e.Message.Username);
                }
            });

            //Start Command Service
            _client.AddService(commandService);
            ModuleService modules = _client.AddService(new ModuleService());

            //Add Each Command Module (Group of Commands)
            modules.Add(new GameCommandsModule(_client, _logger), "HearthstoneCommands");

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
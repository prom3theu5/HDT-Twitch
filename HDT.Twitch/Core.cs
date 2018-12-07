#region

using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using static Twitch.ChatCommands;

#endregion

namespace Twitch
{
    public class Core
    {
        private static TwitchClient _client;
        private static readonly Dictionary<string, ChatCommand> Commands;

        static Core()
        {
            Commands = new Dictionary<string, ChatCommand>();
            AddCommand("commands", CommandsCommand, "ChatCommandCommands");
            AddCommand("deck", DeckCommand, "ChatCommandDeck");
            AddCommand("alldecks", AllDecksCommand, "ChatCommandAllDecks");
            AddCommand("hdt", HdtCommand, "ChatCommandHdt");
            AddCommand("hsreplay", HsrCommand, "ChatCommandHsr");
            AddCommand("lastgamereplay", LastReplayCommand, "ChatCommandLastGameReplay");
            AddCommand("stats", () => StatsCommand(Config.Instance.ChatCommandStatsDefault), "ChatCommandStatsGeneral");
            AddCommand("stats today", () => StatsCommand("today"), "ChatCommandStatsToday", "ChatCommandStatsGeneral");
            AddCommand("stats week", () => StatsCommand("week"), "ChatCommandStatsWeek", "ChatCommandStatsGeneral");
            AddCommand("stats season", () => StatsCommand("season"), "ChatCommandStatsSeason", "ChatCommandStatsGeneral");
            AddCommand("stats total", () => StatsCommand("total"), "ChatCommandStatsTotal", "ChatCommandStatsGeneral");
            AddCommand("arena", () => ArenaCommand(Config.Instance.ChatCommandArenaDefault), "ChatCommandArenaGeneral");
            AddCommand("arena today", () => ArenaCommand("today"), "ChatCommandArenaToday", "ChatCommandArenaGeneral");
            AddCommand("arena week", () => ArenaCommand("week"), "ChatCommandArenaWeek", "ChatCommandArenaGeneral");
            AddCommand("arena season", () => ArenaCommand("season"), "ChatCommandArenaSeason", "ChatCommandArenaGeneral");
            AddCommand("arena total", () => ArenaCommand("total"), "ChatCommandArenaTotal", "ChatCommandArenaGeneral");
            AddCommand("bestdeck", () => BestDeckCommand(Config.Instance.ChatCommandBestDeckDefault), "ChatCommandBestDeckGeneral");
            AddCommand("bestdeck today", () => BestDeckCommand("today"), "ChatCommandBestDeckToday", "ChatCommandBestDeckGeneral");
            AddCommand("bestdeck week", () => BestDeckCommand("week"), "ChatCommandBestDeckWeek", "ChatCommandBestDeckGeneral");
            AddCommand("bestdeck season", () => BestDeckCommand("season"), "ChatCommandBestDeckSeason",
                       "ChatCommandBestDeckGeneral");
            AddCommand("bestdeck total", () => BestDeckCommand("total"), "ChatCommandBestDeckTotal", "ChatCommandBestDeckGeneral");
            AddCommand("mostplayed", () => MostPlayedCommand(Config.Instance.ChatCommandMostPlayedDefault),
                       "ChatCommandMostPlayedGeneral");
            AddCommand("mostplayed today", () => MostPlayedCommand("today"), "ChatCommandMostPlayedToday",
                       "ChatCommandMostPlayedGeneral");
            AddCommand("mostplayed week", () => MostPlayedCommand("week"), "ChatCommandMostPlayedWeek",
                       "ChatCommandMostPlayedGeneral");
            AddCommand("mostplayed season", () => MostPlayedCommand("season"), "ChatCommandMostPlayedSeason",
                       "ChatCommandMostPlayedGeneral");
            AddCommand("mostplayed total", () => MostPlayedCommand("total"), "ChatCommandMostPlayedTotal",
                       "ChatCommandMostPlayedGeneral");
        }

        public static string TwitchTag => "Twitch";

        public static List<string> GetCommandNames() => Commands.Select(x => x.Key).ToList();

        public static void AddCommand(string command, Action action, string propName, string generalPropName = null) => Commands.Add(command, new ChatCommand(command, action, propName, generalPropName));

        internal static void Send(string message)
        {
            if (_client == null)
                return;
            _client.SendMessage(Config.Instance.Channel.ToLower(), message);
            Log.Info(message, "Twitch");
        }

        public static bool Connect()
        {
            Log.Info("Logging in as " + Config.Instance.User);
            ConnectionCredentials credentials = new ConnectionCredentials(Config.Instance.User, Config.Instance.OAuth);
            _client = new TwitchClient(protocol: ClientProtocol.TCP);
            _client.Initialize(credentials, Config.Instance.Channel);
            _client.OnMessageReceived += _client_OnMessageReceived; ;
            _client.OnJoinedChannel += _client_OnJoinedChannel;
            _client.Connect();
            return true;
        }

        private static void _client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Send("Hi! (Hearthstone Deck Tracker connected)");
        }

        private static void _client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.StartsWith("!"))
                return;
            string cmd = e.ChatMessage.Message.Substring(1);
            if (Commands.TryGetValue(cmd, out ChatCommand chatCommand))
                chatCommand.Execute(e.ChatMessage);
            else
                Log.Info($"Unknown command by {e.ChatMessage.DisplayName}: {e.ChatMessage.Message}", "Twitch");
        }

        public static void Disconnect()
        {
            if (_client == null || !_client.IsConnected)
                return;
            Send("Bye! (Hearthstone Deck Tracker disconnected)");
            _client.LeaveChannel(Config.Instance.Channel.ToLower());
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
                Log.Error("Error writing to stats file: " + ex, "Twitch");
            }
        }
    }

    public class ChatCommand
    {
        private readonly Action _action;
        private readonly string _command;
        private readonly string _configItem;
        private readonly string _generalConfigItem;
        private DateTime _lastExecute;

        public ChatCommand(string command, Action action, string configItem, string generalConfigItem = null)
        {
            _command = command;
            _action = action;
            _lastExecute = DateTime.MinValue;
            _configItem = configItem;
            _generalConfigItem = generalConfigItem;
        }

        public void Execute(ChatMessage msg)
        {
            Log.Info($"Command \"{_command}\" requested by {msg.DisplayName}.", "Twitch");
            if (_generalConfigItem != null && !Config.GetConfigItem<bool>(_generalConfigItem))
            {
                Log.Info($"Command \"{_command}\" is disabled (general).", "Twitch");
                return;
            }
            if (!Config.GetConfigItem<bool>(_configItem))
            {
                Log.Info($"Command \"{_command}\" is disabled.", "Twitch");
                return;
            }
            if ((DateTime.Now - _lastExecute).TotalSeconds < 10)
            {
                Log.Info($"Time since last execute of {_command} is less than 10 seconds. Not executing.", "Twitch");
                return;
            }
            _lastExecute = DateTime.Now;
            _action.Invoke();
        }
    }
}
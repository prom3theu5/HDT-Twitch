﻿using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System;
using System.Windows;
using System.Windows.Controls;
using Config = HDT.Twitch.Core.Config;

namespace HDT.Twitch.Plugin
{
    public class Twitch : IPlugin
    {
        private SettingsWindow _settingsWindow;

        public void OnLoad()
        {
            Setup();
            if (MenuItem == null)
                GenerateMenuItem();
            GameEvents.OnGameEnd.Add(Core.Events.OnGameEnd);
            GameEvents.OnInMenu.Add(async () => await Core.Events.OnInMenu());
            //UpdateCheck.Run(Version);
        }

        public void OnUnload()
        {
            _settingsWindow?.Close();
            Core.Disconnect();
        }

        public void OnButtonPress() => OpenSettings();

        public void OnUpdate() => Core.Update();

        public string Name => "Twitch";

        public string Description => "Connects to your Twitch Channel to post your decks, stats and more on command. For a detailed list of commands click the \"INFO\" button under settings.";

        public string ButtonText => "Settings";

        public string Author => "Prom3theu5";

        public Version Version => new Version(1, 0, 0, 0);

        public MenuItem MenuItem { get; private set; }

        private void Setup()
        {
            if (!DeckList.Instance.AllTags.Contains(Core.TwitchTag))
            {
                DeckList.Instance.AllTags.Add(Core.TwitchTag);
                DeckList.Save();
                Hearthstone_Deck_Tracker.API.Core.MainWindow.ReloadTags();
            }
        }

        private void GenerateMenuItem()
        {
            MenuItem = new MenuItem { Header = "Twitch Plugin" };
            MenuItem connectMenuItem = new MenuItem { Header = "CONNECT" };
            MenuItem disconnectMenuItem = new MenuItem { Header = "DISCONNECT", Visibility = Visibility.Collapsed };
            MenuItem settingsMenuItem = new MenuItem { Header = "SETTINGS" };

            connectMenuItem.Click += (sender, args) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(Config.Instance.User) || string.IsNullOrEmpty(Config.Instance.Channel) ||
                       string.IsNullOrEmpty(Config.Instance.OAuth))
                        OpenSettings();
                    else if (Core.Connect())
                    {
                        disconnectMenuItem.Header = $"DISCONNECT ({Config.Instance.User}: {Config.Instance.Channel})";
                        disconnectMenuItem.Visibility = Visibility.Visible;
                        connectMenuItem.Visibility = Visibility.Collapsed;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error connecting to Twitch: " + ex);
                }
            };
            disconnectMenuItem.Click += (sender, args) =>
            {
                Core.Disconnect();
                disconnectMenuItem.Visibility = Visibility.Collapsed;
                connectMenuItem.Visibility = Visibility.Visible;
            };
            settingsMenuItem.Click += (sender, args) => OpenSettings();

            MenuItem.Items.Add(connectMenuItem);
            MenuItem.Items.Add(disconnectMenuItem);
            MenuItem.Items.Add(settingsMenuItem);
        }

        private void OpenSettings()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Closed += (sender1, args1) => { _settingsWindow = null; };
                _settingsWindow.Show();
            }
            else
                _settingsWindow.Activate();
        }
    }
}
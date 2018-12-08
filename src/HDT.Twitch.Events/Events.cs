using HDT.Twitch.Core;
using HDT.Twitch.Core.Extensions;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System.Threading.Tasks;
using Config = HDT.Twitch.Core.Config;

namespace HDT.Twitch.Events
{
    public class Events
    {
        private readonly IClient _client;
        private GameStats _lastGame;
        private int _winStreak;

        public Events(IClient client)
        {
            _client = client;
        }

        public async Task OnInMenu()
        {
            if (!_client.TwitchClient.IsConnected) return;
            if (!Config.Instance.AutoPostGameResult)
                return;
            if (_lastGame == null)
                return;

            string winStreak = _winStreak > 2
                ? $"{_winStreak.GetKillingSpree()}! {_winStreak.GetOrdinal()} win in a row"
                : _lastGame.Result.ToString();

            Deck deck = DeckList.Instance.ActiveDeckVersion;

            string winLossString = deck != null ? ": " + deck.WinLossString : "";

            string message =
                $"{winStreak} vs {_lastGame.OpponentName} ({_lastGame.OpponentHero.ToLower()}) after {_lastGame.Duration}{winLossString}";
            _lastGame = null;

            if (Config.Instance.AutoPostDelay > 0)
            {
                Log.Info($"Waiting {Config.Instance.AutoPostDelay} seconds before posting game result...", "Twitch");
                await Task.Delay(Config.Instance.AutoPostDelay * 1000);
            }

            _client.SendMessage(message);
        }

        public void OnGameEnd()
        {
            _lastGame = Hearthstone_Deck_Tracker.API.Core.Game.CurrentGameStats.CloneWithNewId();
            if (_lastGame.Result == GameResult.Win)
                _winStreak++;
            else
                _winStreak = 0;
        }
    }
}

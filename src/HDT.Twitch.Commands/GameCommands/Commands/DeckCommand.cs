using HDT.Twitch.Core.Commands;
using HDT.Twitch.Core.Extensions;
using Hearthstone_Deck_Tracker;
using System;
using System.Threading.Tasks;
using Deck = Hearthstone_Deck_Tracker.Hearthstone.Deck;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    public class DeckCommand : ChannelCommand
    {
        private const string CurrentlySupportedDecktypes = "[currently only supported for constructed decks]";

        public DeckCommand(ChannelModule module) : base(module)
        {
        }

        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "deck")
                .Description("Get The Current Selected Deck")
                .Do(x => DoGetDeck());
        }

        private Task DoGetDeck()
        {
            try
            {
                Deck deck = DeckList.Instance.ActiveDeckVersion;
                if (deck == null)
                {
                    Client.SendMessage(Module.Client.Channel,
                        $"{Module.Client.Channel} doesn't have a Deck selected in HDT!");
                }

                Client.SendMessage(Module.Client.Channel,
                    deck.IsArenaDeck
                        ? $"Current arena run ({deck.Class}): {deck.WinLossString}, Deck Import Code: {CurrentlySupportedDecktypes}"
                        : $"Currently using \"{deck.Name}\", Class: {deck.Class}, Win-Rate: {deck.WinPercentString} ({deck.WinLossString}), Deck Import Code: {deck.GetHearthstoneDeckCode()}");
            }
            catch (Exception e)
            {
                Module.Logger.Error("Error in {command}: '{error}'", nameof(DeckCommand), e.Message);
            }

            return Task.CompletedTask;
        }
    }
}
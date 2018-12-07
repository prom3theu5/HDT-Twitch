using HearthDb.Deckstrings;
using Hearthstone_Deck_Tracker.Hearthstone;
using Deck = Hearthstone_Deck_Tracker.Hearthstone.Deck;

namespace HDT.Twitch.Core.Extensions
{
    public static class HearthstoneDeck
    {
        public static string GetHearthstoneDeckCode(this Deck deck)
        {
            return deck is null
                ? "Deck code could not be generated as no deck was selected / found"
                : DeckSerializer.Serialize(HearthDbConverter.ToHearthDbDeck(deck), false);
        }
    }
}

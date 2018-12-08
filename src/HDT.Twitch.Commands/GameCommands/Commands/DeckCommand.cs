using HDT.Twitch.Core.Commands;
using HDT.Twitch.Core.Extensions;
using Hearthstone_Deck_Tracker;
using System;
using System.Threading.Tasks;
using Config = HDT.Twitch.Core.Config;
using Deck = Hearthstone_Deck_Tracker.Hearthstone.Deck;

namespace HDT.Twitch.Commands.GameCommands.Commands
{
    /// <summary>
    /// Class DeckCommand.
    /// Implements the <see cref="HDT.Twitch.Commands.ChannelCommand" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Commands.ChannelCommand" />
    public class DeckCommand : ChannelCommand
    {
        /// <summary>
        /// The currently supported decktypes
        /// </summary>
        private const string CurrentlySupportedDecktypes = "[currently only supported for constructed decks]";

        /// <summary>
        /// Creates a new instance of twitch command,
        /// use ": base(module)" in the derived class'
        /// constructor to make sure module is assigned
        /// </summary>
        /// <param name="module">Module this command resides in</param>
        public DeckCommand(ChannelModule module) : base(module)
        {
        }

        /// <summary>
        /// Initializes the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        internal override void Init(CommandGroupBuilder builder)
        {
            builder.CreateCommand(Module.Prefix + "deck")
                .Description("Get The Current Selected Deck")
                .Do(DoGetDeck);
        }

        /// <summary>
        /// Does the get deck.
        /// </summary>
        /// <param name="parameters">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
        /// <returns>Task.</returns>
        private Task DoGetDeck(CommandEventArgs parameters)
        {
            if (!Config.Instance.ChatCommandDeck) return Task.CompletedTask;
            if (Config.Instance.ModeratorOnly)
            {
                if (!parameters.IsAdmin) return Task.CompletedTask;
            }

            try
            {
                Deck deck = DeckList.Instance.ActiveDeckVersion;
                if (deck == null)
                {
                    Module.Client.SendMessage($"{Module.Client.Channel} doesn't have a Deck selected in HDT!");
                }

                Module.Client.SendMessage(
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
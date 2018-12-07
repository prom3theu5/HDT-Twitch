using System;
using TwitchLib.Client.Models;

namespace HDT.Twitch.Core.Commands
{
    /// <summary>
    /// Class CommandServiceConfigBuilder.
    /// </summary>
    public class CommandServiceConfigBuilder
    {
        /// <summary>
        /// Gets or sets the prefix character used to trigger commands, if ActivationMode has the Char flag set.
        /// </summary>
        /// <value>The prefix character.</value>
		public char? PrefixChar { get; set; } = null;
        /// <summary>
        /// Gets or sets a custom function used to detect messages that should be treated as commands.
        /// This function should a positive one indicating the index of where the in the message's RawText the command begins,
        /// and a negative value if the message should be ignored.
        /// </summary>
        /// <value>The custom prefix handler.</value>
        public Func<ChatMessage, int> CustomPrefixHandler { get; set; } = null;
        /// <summary>
        /// Changing this to true makes the bot ignore all messages, except when the messages are from its own account.
        /// </summary>
        /// <value>The execute handler.</value>
        public EventHandler<CommandEventArgs> ExecuteHandler { get; set; }
        /// <summary>
        /// Gets or sets a handler that is called on any error during command parsing or execution.
        /// </summary>
        /// <value>The error handler.</value>
        public EventHandler<CommandErrorEventArgs> ErrorHandler { get; set; }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>CommandServiceConfig.</returns>
        public CommandServiceConfig Build() => new CommandServiceConfig(this);
    }
    /// <summary>
    /// Class CommandServiceConfig.
    /// </summary>
    public class CommandServiceConfig
    {
        /// <summary>
        /// Gets the prefix character.
        /// </summary>
        /// <value>The prefix character.</value>
        public char? PrefixChar { get; }
        /// <summary>
        /// Gets the custom prefix handler.
        /// </summary>
        /// <value>The custom prefix handler.</value>
        public Func<ChatMessage, int> CustomPrefixHandler { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServiceConfig"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        internal CommandServiceConfig(CommandServiceConfigBuilder builder)
        {
            PrefixChar = builder.PrefixChar;
            CustomPrefixHandler = builder.CustomPrefixHandler;
        }
    }
}

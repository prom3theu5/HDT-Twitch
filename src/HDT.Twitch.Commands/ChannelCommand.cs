using HDT.Twitch.Core.Commands;
using TwitchLib.Client.Interfaces;

namespace HDT.Twitch.Commands
{
    /// <summary>
    /// Class ChannelCommand.
    /// </summary>
    public abstract class ChannelCommand
    {
        /// <summary>
        /// Parent module
        /// </summary>
        /// <value>The module.</value>
        protected ChannelModule Module { get; }
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        protected ITwitchClient Client { get; }

        /// <summary>
        /// Creates a new instance of twitch command,
        /// use ": base(module)" in the derived class'
        /// constructor to make sure module is assigned
        /// </summary>
        /// <param name="module">Module this command resides in</param>
        protected ChannelCommand(ChannelModule module)
        {
            Module = module;
            Client = module.Client.TwitchClient;
        }

        /// <summary>
        /// Initializes the specified CGB.
        /// </summary>
        /// <param name="cgb">The CGB.</param>
        internal virtual void Init(CommandGroupBuilder cgb)
        { }
    }
}

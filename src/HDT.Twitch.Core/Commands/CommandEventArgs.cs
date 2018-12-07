using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace HDT.Twitch.Core.Commands
{
    /// <summary>
    /// Class CommandEventArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// The arguments
        /// </summary>
        private readonly string[] _args;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public ChatMessage Message { get; }
        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public Command Command { get; }
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        public Client Client { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="command">The command.</param>
        /// <param name="client">The client.</param>
        /// <param name="args">The arguments.</param>
        public CommandEventArgs(ChatMessage message, Command command, Client client, string[] args)
        {
            Message = message;
            Command = command;
            Client = client;
            _args = args;
            if (command == null) return;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is admin.
        /// </summary>
        /// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
        public bool IsAdmin
        {
            get
            {
                if (Message == null) return false;
                else
                    return Message.Badges.Any(b => b.Key.Equals("broadcaster", StringComparison.InvariantCultureIgnoreCase))
                        || Message.IsModerator;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is super admin.
        /// </summary>
        /// <value><c>true</c> if this instance is super admin; otherwise, <c>false</c>.</value>
        public bool IsSuperAdmin
        {
            get
            {
                if (Message == null) return false;
                else
                    return Message.Badges.Any(b =>
                        b.Key.Equals("broadcaster", StringComparison.InvariantCultureIgnoreCase));
            }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public string[] Args => _args;
        /// <summary>
        /// Gets the argument.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public string GetArg(int index) => _args[index];
        /// <summary>
        /// Gets the argument.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string GetArg(string name) => _args[Command[name].Id];
    }
}

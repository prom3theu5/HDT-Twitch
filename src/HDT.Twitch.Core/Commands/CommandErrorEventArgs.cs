using System;

namespace HDT.Twitch.Core.Commands
{
    /// <summary>
    /// Enum CommandErrorType
    /// </summary>
    public enum CommandErrorType { Exception, UnknownCommand, BadArgCount, InvalidInput }
    /// <summary>
    /// Class CommandErrorEventArgs.
    /// Implements the <see cref="HDT.Twitch.Core.Commands.CommandEventArgs" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Core.Commands.CommandEventArgs" />
    public class CommandErrorEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Gets the type of the error.
        /// </summary>
        /// <value>The type of the error.</value>
        public CommandErrorType ErrorType { get; }
        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandErrorEventArgs"/> class.
        /// </summary>
        /// <param name="errorType">Type of the error.</param>
        /// <param name="baseArgs">The <see cref="CommandEventArgs"/> instance containing the event data.</param>
        /// <param name="client">The client.</param>
        /// <param name="ex">The ex.</param>
        public CommandErrorEventArgs(CommandErrorType errorType, CommandEventArgs baseArgs, Client client, Exception ex)
            : base(baseArgs.Message, baseArgs.Command, client, baseArgs.Args)
        {
            Exception = ex;
            ErrorType = errorType;
        }
    }
}

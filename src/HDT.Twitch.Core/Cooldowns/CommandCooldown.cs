using System;

namespace HDT.Twitch.Core.Cooldowns
{
    /// <summary>
    /// Class CommandCooldown.
    /// </summary>
    public class CommandCooldown
    {
        /// <summary>
        /// The seconds
        /// </summary>
        private readonly int _seconds;
        /// <summary>
        /// The start time
        /// </summary>
        private double _startTime;

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public string Command { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCooldown"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="seconds">The seconds.</param>
        public CommandCooldown(string command, int seconds)
        {
            Command = command;
            _seconds = seconds;
            _startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Activate()
        {
            if (!((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds - _startTime >
                  _seconds)) return false;

            _startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            return true;
        }
    }
}

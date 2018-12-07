using System;

namespace HDT.Twitch.Core.Cooldowns
{
    /// <summary>
    /// Class UserCommandCooldowns.
    /// </summary>
    public class UserCommandCooldowns
    {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <value>The user.</value>
        public string User { get; private set; }
        /// <summary>
        /// The user cd
        /// </summary>
        private readonly int _userCd;
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
        /// Initializes a new instance of the <see cref="UserCommandCooldowns"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userCd">The user cd.</param>
        public UserCommandCooldowns(string user, int userCd)
        {
            User = user;
            _userCd = userCd;
            _startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Activates the user cooldown.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ActivateUserCooldown()
        {
            if ((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds - _startTime > _userCd)
            {
                _startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                return true;
            }
            return false;
        }
    }
}

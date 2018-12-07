using System;

namespace HDT.Twitch.Core.Cooldowns
{
    /// <summary>
    /// Class ActivationCooldown.
    /// </summary>
    public class ActivationCooldown
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
        /// Initializes a new instance of the <see cref="ActivationCooldown"/> class.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        public ActivationCooldown(int seconds)
        {
            _seconds = seconds;
            _startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Activates the cooldown.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ActivateCooldown()
        {
            if ((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds - _startTime > _seconds)
            {
                _startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                return true;
            }
            return false;
        }
    }
}

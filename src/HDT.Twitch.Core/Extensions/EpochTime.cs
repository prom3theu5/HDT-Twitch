using System;

namespace HDT.Twitch.Core.Extensions
{
    /// <summary>
    /// Class EpochTime.
    /// </summary>
    public static class EpochTime
    {
        /// <summary>
        /// The epoch
        /// </summary>
        private const long epoch = 621355968000000000L; //1/1/1970 in Ticks

        /// <summary>
        /// Gets the milliseconds.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public static long GetMilliseconds() => (DateTime.UtcNow.Ticks - epoch) / TimeSpan.TicksPerMillisecond;
    }
}
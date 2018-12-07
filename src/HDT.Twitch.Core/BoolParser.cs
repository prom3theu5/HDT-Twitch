using System;
using System.Collections.Generic;
using System.Linq;

namespace HDT.Twitch.Core
{
    /// <summary>
    /// Class BoolParser.
    /// </summary>
    public static class BoolParser
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetValue(string value)
        {
            return IsTrue(value);
        }

        /// <summary>
        /// Determines whether the specified value is false.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is false; otherwise, <c>false</c>.</returns>
        public static bool IsFalse(string value)
        {
            return !IsTrue(value);
        }

        /// <summary>
        /// Checks the bool valid.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckBoolValid(string word)
        {
            return ValidWords().Any(c => c.Equals(word, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Valids the words.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> ValidWords()
        {
            return new List<string>() { "win", "true", "yes", "false", "lose", "no" };
        }

        /// <summary>
        /// Determines whether the specified value is true.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is true; otherwise, <c>false</c>.</returns>
        public static bool IsTrue(string value)
        {
            try
            {
                if (value == null)
                {
                    return false;
                }

                value = value.Trim();
                value = value.ToLower();

                if (value == "true")
                {
                    return true;
                }
                if (value == "yes")
                {
                    return true;
                }
                if (value == "win")
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

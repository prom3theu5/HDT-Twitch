using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace HDT.Twitch.Core.Extensions
{
    /// <summary>
    /// Class GeneralExtensions.
    /// </summary>
    public static class GeneralExtensions
    {
        /// <summary>
        /// The RNG
        /// </summary>
        private static Random rng = new Random();

        /// <summary>
        /// Scrambles the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>System.String.</returns>
        public static string Scramble(this string word)
        {

            char[] letters = word.ToArray();
            int count = 0;
            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == ' ')
                    continue;

                count++;
                if (count <= letters.Length / 5)
                    continue;

                if (count % 3 == 0)
                    continue;

                if (letters[i] != ' ')
                    letters[i] = '_';
            }
            return "`" + string.Join(" ", letters) + "`";
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }

        /// <summary>
        /// Levenshteins the distance.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="t">The t.</param>
        /// <returns>System.Int32.</returns>
        public static int LevenshteinDistance(this string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        /// <summary>
        /// Kis the b.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public static int KiB(this int value) => value * 1024;
        /// <summary>
        /// Kbs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public static int KB(this int value) => value * 1000;

        /// <summary>
        /// Mis the b.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public static int MiB(this int value) => value.KiB() * 1024;
        /// <summary>
        /// Mbs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public static int MB(this int value) => value.KB() * 1000;

        /// <summary>
        /// Gis the b.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public static int GiB(this int value) => value.MiB() * 1024;
        /// <summary>
        /// Gbs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public static int GB(this int value) => value.MB() * 1000;

        /// <summary>
        /// Matrixes the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.String.</returns>
        public static string Matrix(this string s) => string.Join("", s.Select(c => c.ToString() + " ̵̢̬̜͉̞̭̖̰͋̉̎ͬ̔̇̌̀".TrimTo(rng.Next(0, 12), true)));

        /// <summary>
        /// Shuffles the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[(n / byte.MaxValue) + 1];
                int boxSum;
                do
                {
                    provider.GetBytes(box);
                    boxSum = box.Sum(b => b);
                }
                while (!(boxSum < n * ((byte.MaxValue * box.Length) / n)));
                int k = (boxSum % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Trims to.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="num">The number.</param>
        /// <param name="hideDots">if set to <c>true</c> [hide dots].</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentOutOfRangeException">num - TrimTo argument cannot be less than 0</exception>
        public static string TrimTo(this string str, int num, bool hideDots = false)
        {
            if (num < 0)
                throw new ArgumentOutOfRangeException(nameof(num), "TrimTo argument cannot be less than 0");
            if (num == 0)
                return string.Empty;
            if (num <= 3)
                return string.Join("", str.Select(c => '.'));
            if (str.Length < num)
                return str;
            return string.Join("", str.Take(num - 3)) + (hideDots ? "" : "...");
        }


        /// <summary>
        /// Gets the killing spree.
        /// </summary>
        /// <param name="wins">The wins.</param>
        /// <returns>System.String.</returns>
        public static string GetKillingSpree(this int wins)
        {
            int index = wins / 3 - 1;
            if (index < 0)
                return "";
            if (index > 5)
                index = 5;
            return KillingSprees[index];
        }

        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>System.String.</returns>
        public static string GetOrdinal(this int number)
        {
            if (number < 0)
                return number.ToString();
            int rem = number % 100;
            if (rem >= 11 && rem <= 13)
                return number + "th";
            switch (number % 10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }

        /// <summary>
        /// The killing sprees
        /// </summary>
        private static readonly string[] KillingSprees = { "Killing Spree", "Rampage", "Dominating", "Unstoppable", "GODLIKE", "WICKED SICK" };
    }
}

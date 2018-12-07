using System;
using System.Threading.Tasks;

namespace HDT.Twitch.Core.Extensions
{
    /// <summary>
    /// Class TaskHelper.
    /// </summary>
    internal static class TaskHelper
    {
        /// <summary>
        /// Gets the completed task.
        /// </summary>
        /// <value>The completed task.</value>
        public static Task CompletedTask => Task.Delay(0);

        /// <summary>
        /// Converts to async.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Func&lt;Task&gt;.</returns>
        public static Func<Task> ToAsync(Action action)
        {
            return () =>
            {
                action(); return CompletedTask;
            };
        }
        /// <summary>
        /// Converts to async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns>Func&lt;T, Task&gt;.</returns>
        public static Func<T, Task> ToAsync<T>(Action<T> action)
        {
            return x =>
            {
                action(x); return CompletedTask;
            };
        }
    }
}
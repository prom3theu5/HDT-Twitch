using System;
using System.Threading;
using System.Threading.Tasks;

namespace HDT.Twitch.Core.Extensions
{
    /// <summary>
    /// Class TaskExtensions.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Timeouts the specified milliseconds.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns>Task.</returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task Timeout(this Task task, int milliseconds)
        {
            Task timeoutTask = Task.Delay(milliseconds);
            Task finishedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
            if (finishedTask == timeoutTask)
                throw new TimeoutException();
            else
                await task.ConfigureAwait(false);
        }
        /// <summary>
        /// Timeouts the specified milliseconds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task<T> Timeout<T>(this Task<T> task, int milliseconds)
        {
            Task timeoutTask = Task.Delay(milliseconds);
            Task finishedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
            if (finishedTask == timeoutTask)
                throw new TimeoutException();
            else
                return await task.ConfigureAwait(false);
        }
        /// <summary>
        /// Timeouts the specified milliseconds.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="timeoutToken">The timeout token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task Timeout(this Task task, int milliseconds, CancellationTokenSource timeoutToken)
        {
            try
            {
                timeoutToken.CancelAfter(milliseconds);
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (timeoutToken.IsCancellationRequested)
                    throw new TimeoutException();
                throw;
            }
        }
        /// <summary>
        /// Timeouts the specified milliseconds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="timeoutToken">The timeout token.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task<T> Timeout<T>(this Task<T> task, int milliseconds, CancellationTokenSource timeoutToken)
        {
            try
            {
                timeoutToken.CancelAfter(milliseconds);
                return await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (timeoutToken.IsCancellationRequested)
                    throw new TimeoutException();
                throw;
            }
        }

        /// <summary>
        /// Waits the specified token source.
        /// </summary>
        /// <param name="tokenSource">The token source.</param>
        /// <returns>Task.</returns>
        public static async Task Wait(this CancellationTokenSource tokenSource)
        {
            CancellationToken token = tokenSource.Token;
            try { await Task.Delay(-1, token).ConfigureAwait(false); }
            catch (OperationCanceledException) { } //Expected
        }
        /// <summary>
        /// Waits the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public static async Task Wait(this CancellationToken token)
        {
            try { await Task.Delay(-1, token).ConfigureAwait(false); }
            catch (OperationCanceledException) { } //Expected
        }
    }
}
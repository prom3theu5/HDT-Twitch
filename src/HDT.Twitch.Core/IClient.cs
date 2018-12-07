using System;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;

namespace HDT.Twitch.Core
{
    /// <summary>
    /// Interface IClient
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        string Channel { get; }
        /// <summary>
        /// Gets or sets the last message received.
        /// </summary>
        /// <value>The last message received.</value>
        DateTime LastMessageReceived { get; set; }
        /// <summary>
        /// Gets the nickname.
        /// </summary>
        /// <value>The nickname.</value>
        string Nickname { get; }
        /// <summary>
        /// Gets the twitch client.
        /// </summary>
        /// <value>The twitch client.</value>
        ITwitchClient TwitchClient { get; }
        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        T AddService<T>() where T : class, IService, new();
        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>T.</returns>
        T AddService<T>(T instance) where T : class, IService;
        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>Task.</returns>
        Task Connect();
        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns>Task.</returns>
        Task Disconnect();
        /// <summary>
        /// Executes the and wait.
        /// </summary>
        /// <param name="asyncAction">The asynchronous action.</param>
        void ExecuteAndWait(Func<Task> asyncAction);
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>T.</returns>
        T GetService<T>(bool isRequired = true) where T : class, IService;

        void SendMessage(string message);
    }
}
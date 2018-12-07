using Nito.AsyncEx;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace HDT.Twitch.Core
{
    /// <summary>
    /// Class Client.
    /// Implements the <see cref="HDT.Twitch.Core.IClient" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Core.IClient" />
    public class Client : IClient
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// The nickname
        /// </summary>
        private readonly string _nickname;
        /// <summary>
        /// The password
        /// </summary>
        private readonly string _password;
        /// <summary>
        /// The channel
        /// </summary>
        private readonly string _channel;
        /// <summary>
        /// The connection lock
        /// </summary>
        private readonly AsyncLock _connectionLock;
        /// <summary>
        /// The disconnected event
        /// </summary>
        private readonly ManualResetEvent _disconnectedEvent;
        /// <summary>
        /// The connected event
        /// </summary>
        private readonly ManualResetEventSlim _connectedEvent;
        /// <summary>
        /// The services
        /// </summary>
        private readonly ServiceCollection _services;
        /// <summary>
        /// The connection stopwatch
        /// </summary>
        private readonly Stopwatch _connectionStopwatch;
        /// <summary>
        /// Gets the twitch client.
        /// </summary>
        /// <value>The twitch client.</value>
        public ITwitchClient TwitchClient { get; private set; }
        /// <summary>
        /// Gets or sets the last message received.
        /// </summary>
        /// <value>The last message received.</value>
        public DateTime LastMessageReceived { get; set; }
        /// <summary>
        /// Gets the nickname.
        /// </summary>
        /// <value>The nickname.</value>
        public string Nickname => _nickname;
        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public string Channel => _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public Client(ILogger logger)
        {
            _logger = logger;
            _connectionStopwatch = new Stopwatch();

            //Async
            _connectionLock = new AsyncLock();
            _disconnectedEvent = new ManualResetEvent(true);
            _connectedEvent = new ManualResetEventSlim(false);

            _nickname = Config.Instance.User;
            _password = Config.Instance.OAuth;
            _channel = Config.Instance.Channel;

            //Client
            ConnectionCredentials credentials = new ConnectionCredentials(_nickname, _password);
            TwitchClient = new TwitchClient(protocol: ClientProtocol.TCP);
            TwitchClient.Initialize(credentials, _channel);
            TwitchClient.OnConnected += TwitchClient_OnConnected;
            TwitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
            TwitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;

            //Extensibility
            _services = new ServiceCollection(this);
        }

        /// <summary>
        /// Twitches the client on joined channel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TwitchClient_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
        {
            _logger.Information("Joined Channel > {channel} as {user}", e.Channel, e.BotUsername);
        }

        /// <summary>
        /// Twitches the client on message received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TwitchClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            LastMessageReceived = DateTime.Now;
        }

        /// <summary>
        /// Twitches the client on connected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TwitchClient_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            _logger.Information("Connected to Twitch.");
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task Connect()
        {
            using (await _connectionLock.LockAsync().ConfigureAwait(false))
            {
                await Disconnect().ConfigureAwait(false);
                _disconnectedEvent.Reset();
                TwitchClient.Connect();
            }
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns>Task.</returns>
        public Task Disconnect()
        {
            return Task.Run(() =>
            {
                if (TwitchClient.IsConnected)
                {
                    TwitchClient.Disconnect();
                }

                _connectedEvent.Reset();
                _disconnectedEvent.Set();
            });
        }

        public void SendMessage(string message)
        {
            TwitchClient.SendMessage(Channel, message);
        }

        #region Services
        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>T.</returns>
        public T AddService<T>(T instance)
            where T : class, IService
            => _services.Add(instance);
        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T AddService<T>()
            where T : class, IService, new()
            => _services.Add(new T());
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>T.</returns>
        public T GetService<T>(bool isRequired = true)
            where T : class, IService
            => _services.Get<T>(isRequired);
        #endregion

        #region Async Wrapper
        /// <summary>
        /// Executes the and wait.
        /// </summary>
        /// <param name="asyncAction">The asynchronous action.</param>
        public void ExecuteAndWait(Func<Task> asyncAction)
        {
            asyncAction().GetAwaiter().GetResult();
            _disconnectedEvent.WaitOne();
        }

        #endregion
    }
}
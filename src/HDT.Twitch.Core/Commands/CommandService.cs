using System;
using System.Collections.Generic;

namespace HDT.Twitch.Core.Commands
{
    /// <summary>
    /// Class CommandService.
    /// Implements the <see cref="HDT.Twitch.Core.IService" />
    /// </summary>
    /// <seealso cref="HDT.Twitch.Core.IService" />
    public partial class CommandService : IService
    {
        /// <summary>
        /// All commands
        /// </summary>
        private readonly List<Command> _allCommands;
        /// <summary>
        /// The categories
        /// </summary>
        private readonly Dictionary<string, CommandMap> _categories;
        /// <summary>
        /// The map
        /// </summary>
        private readonly CommandMap _map; //Command map stores all commands by their input text, used for fast resolving and parsing

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public CommandServiceConfig Config { get; }
        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The root.</value>
        public CommandGroupBuilder Root { get; }
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        public Client Client { get; private set; }

        //AllCommands store a flattened collection of all commands
        /// <summary>
        /// Gets all commands.
        /// </summary>
        /// <value>All commands.</value>
        public IEnumerable<Command> AllCommands => _allCommands;
        //Groups store all commands by their module, used for more informative help
        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        internal IEnumerable<CommandMap> Categories => _categories.Values;

        /// <summary>
        /// Occurs when [command executed].
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandExecuted = delegate { };
        /// <summary>
        /// Occurs when [command errored].
        /// </summary>
        public event EventHandler<CommandErrorEventArgs> CommandErrored = delegate { };

        /// <summary>
        /// Handles the <see cref="E:Command" /> event.
        /// </summary>
        /// <param name="args">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
        private void OnCommand(CommandEventArgs args)
            => CommandExecuted(this, args);
        /// <summary>
        /// Called when [command error].
        /// </summary>
        /// <param name="errorType">Type of the error.</param>
        /// <param name="args">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
        /// <param name="ex">The ex.</param>
        private void OnCommandError(CommandErrorType errorType, CommandEventArgs args, Exception ex = null)
            => CommandErrored(this, new CommandErrorEventArgs(errorType, args, Client, ex));

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandService" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public CommandService(CommandServiceConfigBuilder builder)
            : this(builder.Build())
        {
            if (builder.ExecuteHandler != null)
            {
                CommandExecuted += builder.ExecuteHandler;
            }
            if (builder.ErrorHandler != null)
                CommandErrored += builder.ErrorHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandService" /> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public CommandService(CommandServiceConfig config)
        {
            Config = config;

            _allCommands = new List<Command>();
            _map = new CommandMap();
            _categories = new Dictionary<string, CommandMap>();
            Root = new CommandGroupBuilder(this);
        }

        /// <summary>
        /// Installs the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        void IService.Install(Client client)
        {
            Client = client;

            client.TwitchClient.OnMessageReceived += async (s, e) =>
            {
                if (_allCommands.Count == 0) return;
                if (string.IsNullOrWhiteSpace(e.ChatMessage.Message)) return;

                string cmdMsg = null;

                //Check for command char
                if (Config.PrefixChar.HasValue)
                {
                    if (e.ChatMessage.Message[0] == Config.PrefixChar.Value)
                        cmdMsg = e.ChatMessage.Message.Substring(1);
                }

                //Check using custom activator
                if (cmdMsg == null && Config.CustomPrefixHandler != null)
                {
                    int index = Config.CustomPrefixHandler(e.ChatMessage);
                    if (index >= 0)
                        cmdMsg = e.ChatMessage.Message.Substring(index);
                }

                if (cmdMsg == null) return;

                //Parse command
                CommandParser.ParseCommand(cmdMsg, _map, out IEnumerable<Command> commands, out int argPos);
                if (commands == null)
                {
                    CommandEventArgs errorArgs = new CommandEventArgs(e.ChatMessage, null, Client, null);
                    OnCommandError(CommandErrorType.UnknownCommand, errorArgs);
                    return;
                }
                else
                {
                    foreach (Command command in commands)
                    {
                        //Parse arguments
                        CommandErrorType? error = CommandParser.ParseArgs(cmdMsg, argPos, command, out string[] args);
                        if (error != null)
                        {
                            if (error == CommandErrorType.BadArgCount)
                                continue;
                            else
                            {
                                CommandEventArgs errorArgs = new CommandEventArgs(e.ChatMessage, command, Client, null);
                                OnCommandError(error.Value, errorArgs);
                                return;
                            }
                        }

                        CommandEventArgs eventArgs = new CommandEventArgs(e.ChatMessage, command, Client, args);

                        // Run the command
                        try
                        {
                            OnCommand(eventArgs);
                            await command.Run(eventArgs).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            OnCommandError(CommandErrorType.Exception, eventArgs, ex);
                        }
                        return;
                    }
                    CommandEventArgs errorArgs2 = new CommandEventArgs(e.ChatMessage, null, Client, null);
                    OnCommandError(CommandErrorType.BadArgCount, errorArgs2);
                }
            };
        }

        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="config">The configuration.</param>
        public void CreateGroup(string cmd, Action<CommandGroupBuilder> config = null) => Root.CreateGroup(cmd, config);
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>CommandBuilder.</returns>
        public CommandBuilder CreateCommand(string cmd) => Root.CreateCommand(cmd);

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="command">The command.</param>
        internal void AddCommand(Command command)
        {
            _allCommands.Add(command);

            //Get category
            string categoryName = command.Category ?? "";
            if (!_categories.TryGetValue(categoryName, out CommandMap category))
            {
                category = new CommandMap();
                _categories.Add(categoryName, category);
            }

            //Add main command
            category.AddCommand(command.Text, command, false);
            _map.AddCommand(command.Text, command, false);

            //Add aliases
            foreach (string alias in command.Aliases)
            {
                category.AddCommand(alias, command, true);
                _map.AddCommand(alias, command, true);
            }
        }

        /// <summary>
        /// Removes the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void RemoveCommand(Command command)
        {
            try
            {
                _allCommands.Remove(command);

                CommandMap cat = _categories[command.Category];
                cat.Items.Remove(command.Text.ToLower());

                foreach (string item in command.Aliases)
                {
                    cat.Items.Remove(item.ToLower());
                    _map.Items.Remove(item.ToLower());
                }

                _map.Items.Remove(command.Text.ToLower());
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.Message);
            }
        }

    }
}

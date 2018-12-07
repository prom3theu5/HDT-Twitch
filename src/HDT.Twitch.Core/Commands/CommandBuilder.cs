using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDT.Twitch.Core.Commands
{
    //TODO: Make this more friendly and expose it to be extendable
    /// <summary>
    /// Class CommandBuilder. This class cannot be inherited.
    /// </summary>
    public sealed class CommandBuilder
    {
        /// <summary>
        /// The service
        /// </summary>
        private readonly CommandService _service;

        public void Do(Task task)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The command
        /// </summary>
        private readonly Command _command;
        /// <summary>
        /// The parameters
        /// </summary>
        private readonly List<CommandParameter> _params;
        /// <summary>
        /// The aliases
        /// </summary>
        private readonly List<string> _aliases;
        /// <summary>
        /// The prefix
        /// </summary>
        private readonly string _prefix;
        /// <summary>
        /// The allow required parameters
        /// </summary>
        private bool _allowRequiredParams, _areParamsClosed;

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>The service.</value>
        public CommandService Service => _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBuilder"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="text">The text.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="category">The category.</param>
        internal CommandBuilder(CommandService service, string text, string prefix = "", string category = "")
        {
            _service = service;
            _prefix = prefix;

            _command = new Command(AppendPrefix(prefix, text));
            _command.Category = category;

            _params = new List<CommandParameter>();
            _aliases = new List<string>();

            _allowRequiredParams = true;
            _areParamsClosed = false;
        }

        /// <summary>
        /// Aliases the specified aliases.
        /// </summary>
        /// <param name="aliases">The aliases.</param>
        /// <returns>CommandBuilder.</returns>
        public CommandBuilder Alias(params string[] aliases)
        {
            _aliases.AddRange(aliases);
            return this;
        }
        /// <summary>
        /// Descriptions the specified description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>CommandBuilder.</returns>
        public CommandBuilder Description(string description)
        {
            _command.Description = description;
            return this;
        }
        /// <summary>
        /// Parameters the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns>CommandBuilder.</returns>
        /// <exception cref="System.Exception">
        /// No parameters may be added after a {nameof(ParameterType.Multiple)} or {nameof(ParameterType.Unparsed)}
        /// or
        /// </exception>
        public CommandBuilder Parameter(string name, ParameterType type = ParameterType.Required)
        {
            if (_areParamsClosed)
                throw new Exception($"No parameters may be added after a {nameof(ParameterType.Multiple)} or {nameof(ParameterType.Unparsed)} parameter.");
            if (!_allowRequiredParams && type == ParameterType.Required)
                throw new Exception($"{nameof(ParameterType.Required)} parameters may not be added after an optional one");

            _params.Add(new CommandParameter(name, type));

            if (type == ParameterType.Optional)
                _allowRequiredParams = false;
            if (type == ParameterType.Multiple || type == ParameterType.Unparsed)
                _areParamsClosed = true;
            return this;
        }
        /// <summary>
        /// Hides this instance.
        /// </summary>
        /// <returns>CommandBuilder.</returns>
        public CommandBuilder Hide()
        {
            _command.IsHidden = true;
            return this;
        }
        /// <summary>
        /// Does the specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        public void Do(Func<CommandEventArgs, Task> func)
        {
            _command.SetRunFunc(func);
            Build();
        }
        /// <summary>
        /// Does the specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        public void Do(Action<CommandEventArgs> func)
        {
            _command.SetRunFunc(func);
            Build();
        }
        /// <summary>
        /// Builds this instance.
        /// </summary>
        private void Build()
        {
            _command.SetParameters(_params.ToArray());
            _command.SetAliases(_aliases.Select(x => AppendPrefix(_prefix, x)).ToArray());
            _service.AddCommand(_command);
        }

        /// <summary>
        /// Appends the prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="cmd">The command.</param>
        /// <returns>System.String.</returns>
        internal static string AppendPrefix(string prefix, string cmd)
        {
            if (cmd != "")
            {
                if (prefix != "")
                    return prefix + ' ' + cmd;
                else
                    return cmd;
            }
            else
                return prefix;
        }
    }
    /// <summary>
    /// Class CommandGroupBuilder.
    /// </summary>
    public class CommandGroupBuilder
    {
        /// <summary>
        /// The service
        /// </summary>
        private readonly CommandService _service;
        /// <summary>
        /// The prefix
        /// </summary>
        private readonly string _prefix;
        /// <summary>
        /// The category
        /// </summary>
        private string _category;

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>The service.</value>
        public CommandService Service => _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandGroupBuilder"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="category">The category.</param>
        internal CommandGroupBuilder(CommandService service, string prefix = "", string category = null)
        {
            _service = service;
            _prefix = prefix;
            _category = category;
        }

        /// <summary>
        /// Categories the specified category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>CommandGroupBuilder.</returns>
        public CommandGroupBuilder Category(string category)
        {
            _category = category;
            return this;
        }
        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>CommandGroupBuilder.</returns>
        public CommandGroupBuilder CreateGroup(string cmd, Action<CommandGroupBuilder> config)
        {
            config(new CommandGroupBuilder(_service, CommandBuilder.AppendPrefix(_prefix, cmd), _category));
            return this;
        }
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <returns>CommandBuilder.</returns>
        public CommandBuilder CreateCommand()
            => CreateCommand("");
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>CommandBuilder.</returns>
        public CommandBuilder CreateCommand(string cmd)
            => new CommandBuilder(_service, cmd, _prefix, _category);
    }
}

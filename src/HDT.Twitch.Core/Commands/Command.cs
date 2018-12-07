using HDT.Twitch.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HDT.Twitch.Core.Commands
{
    /// <summary>
    /// Class Command.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// The aliases
        /// </summary>
        private string[] _aliases;
        /// <summary>
        /// The parameters
        /// </summary>
        internal CommandParameter[] _parameters;
        /// <summary>
        /// The run function
        /// </summary>
        private Func<CommandEventArgs, Task> _runFunc;
        /// <summary>
        /// The parameters by name
        /// </summary>
        internal readonly Dictionary<string, CommandParameter> _parametersByName;

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; }
        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        public string Category { get; internal set; }
        /// <summary>
        /// Gets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; internal set; }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; internal set; }

        /// <summary>
        /// Gets the aliases.
        /// </summary>
        /// <value>The aliases.</value>
        public IEnumerable<string> Aliases => _aliases;
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IEnumerable<CommandParameter> Parameters => _parameters;
        /// <summary>
        /// Gets the <see cref="CommandParameter"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>CommandParameter.</returns>
        public CommandParameter this[string name] => _parametersByName[name];

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        internal Command(string text)
        {
            Text = text;
            IsHidden = false;
            _aliases = new string[0];
            _parameters = new CommandParameter[0];
            _parametersByName = new Dictionary<string, CommandParameter>();
        }


        /// <summary>
        /// Sets the aliases.
        /// </summary>
        /// <param name="aliases">The aliases.</param>
        internal void SetAliases(string[] aliases)
        {
            _aliases = aliases;
        }
        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        internal void SetParameters(CommandParameter[] parameters)
        {
            _parametersByName.Clear();
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].Id = i;
                _parametersByName[parameters[i].Name] = parameters[i];
            }
            _parameters = parameters;
        }

        /// <summary>
        /// Sets the run function.
        /// </summary>
        /// <param name="func">The function.</param>
        internal void SetRunFunc(Func<CommandEventArgs, Task> func)
        {
            _runFunc = func;
        }

        /// <summary>
        /// Sets the run function.
        /// </summary>
        /// <param name="func">The function.</param>
        internal void SetRunFunc(Action<CommandEventArgs> func)
        {
            _runFunc = TaskHelper.ToAsync(func);
        }

        /// <summary>
        /// Runs the specified arguments.
        /// </summary>
        /// <param name="args">The <see cref="CommandEventArgs"/> instance containing the event data.</param>
        /// <returns>Task.</returns>
        internal Task Run(CommandEventArgs args)
        {
            Task task = _runFunc(args);
            if (task != null)
                return task;
            else
                return TaskHelper.CompletedTask;
        }
    }
}

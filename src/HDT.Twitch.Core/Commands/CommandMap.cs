﻿using System.Collections.Generic;

namespace HDT.Twitch.Core.Commands
{
    internal class CommandMap
    {
        private readonly CommandMap _parent;
        private readonly string _name, _fullName;

        private readonly List<Command> _commands;
        private readonly Dictionary<string, CommandMap> _items;
        private bool _isVisible, _hasNonAliases, _hasSubGroups;

        public string Name => _name;
        public string FullName => _fullName;
        public bool IsVisible => _isVisible;
        public bool HasNonAliases => _hasNonAliases;
        public bool HasSubGroups => _hasSubGroups;
        public IEnumerable<Command> Commands => _commands;
        public IEnumerable<CommandMap> SubGroups => _items.Values;
        public Dictionary<string, CommandMap> Items => _items;

        public CommandMap()
        {
            _items = new Dictionary<string, CommandMap>();
            _commands = new List<Command>();
            _isVisible = false;
            _hasNonAliases = false;
            _hasSubGroups = false;
        }
        public CommandMap(CommandMap parent, string name, string fullName)
            : this()
        {
            _parent = parent;
            _name = name;
            _fullName = fullName;
        }

        public CommandMap GetItem(string text)
        {
            return GetItem(0, text.Split(' '));
        }
        public CommandMap GetItem(int index, string[] parts)
        {
            if (index != parts.Length)
            {
                string nextPart = parts[index];
                if (_items.TryGetValue(nextPart.ToLowerInvariant(), out CommandMap nextGroup))
                    return nextGroup.GetItem(index + 1, parts);
                else
                    return null;
            }
            return this;
        }

        public IEnumerable<Command> GetCommands()
        {
            if (_commands.Count > 0)
                return _commands;
            else if (_parent != null)
                return _parent.GetCommands();
            else
                return null;
        }
        public IEnumerable<Command> GetCommands(string text)
        {
            return GetCommands(0, text.Split(' '));
        }
        public IEnumerable<Command> GetCommands(int index, string[] parts)
        {
            if (index != parts.Length)
            {
                string nextPart = parts[index];
                if (_items.TryGetValue(nextPart.ToLowerInvariant(), out CommandMap nextGroup))
                {
                    IEnumerable<Command> cmd = nextGroup.GetCommands(index + 1, parts);
                    if (cmd != null)
                        return cmd;
                }
            }

            if (_commands != null)
                return _commands;
            return null;
        }

        public void AddCommand(string text, Command command, bool isAlias)
        {
            AddCommand(0, text.Split(' '), command, isAlias);
        }
        private void AddCommand(int index, string[] parts, Command command, bool isAlias)
        {
            if (!command.IsHidden)
                _isVisible = true;

            if (index != parts.Length)
            {
                string name = parts[index].ToLowerInvariant();
                string fullName = string.Join(" ", parts, 0, index + 1);
                if (!_items.TryGetValue(name, out CommandMap nextGroup))
                {
                    nextGroup = new CommandMap(this, name, fullName);
                    _items.Add(name, nextGroup);
                    _hasSubGroups = true;
                }
                nextGroup.AddCommand(index + 1, parts, command, isAlias);
            }
            else
            {
                _commands.Add(command);
                if (!isAlias)
                    _hasNonAliases = true;
            }
        }

        public void RemoveCommand(Command command)
        {
            _commands.Remove(command);
        }
    }
}

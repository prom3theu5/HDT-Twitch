using System.Collections.Generic;

namespace HDT.Twitch.Core.Commands
{
    /// <summary>
    /// Class CommandParser.
    /// </summary>
    internal static class CommandParser
    {
        /// <summary>
        /// Enum ParserPart
        /// </summary>
        private enum ParserPart
        {
            /// <summary>
            /// The none
            /// </summary>
            None,
            /// <summary>
            /// The parameter
            /// </summary>
            Parameter,
            /// <summary>
            /// The quoted parameter
            /// </summary>
            QuotedParameter,
            /// <summary>
            /// The double quoted parameter
            /// </summary>
            DoubleQuotedParameter
        }

        /// <summary>
        /// Parses the command.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="map">The map.</param>
        /// <param name="commands">The commands.</param>
        /// <param name="endPos">The end position.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ParseCommand(string input, CommandMap map, out IEnumerable<Command> commands, out int endPos)
        {
            int startPosition = 0;
            int endPosition = 0;
            int inputLength = input.Length;
            bool isEscaped = false;
            commands = null;
            endPos = 0;

            if (input == "")
                return false;

            while (endPosition < inputLength)
            {
                char currentChar = input[endPosition++];
                if (isEscaped)
                    isEscaped = false;
                else if (currentChar == '\\')
                    isEscaped = true;

                bool isWhitespace = IsWhiteSpace(currentChar);
                if ((!isEscaped && isWhitespace) || endPosition >= inputLength)
                {
                    int length = (isWhitespace ? endPosition - 1 : endPosition) - startPosition;
                    string temp = input.Substring(startPosition, length);
                    if (temp == "")
                        startPosition = endPosition;
                    else
                    {
                        CommandMap newMap = map.GetItem(temp);
                        if (newMap != null)
                        {
                            map = newMap;
                            endPos = endPosition;
                        }
                        else
                            break;
                        startPosition = endPosition;
                    }
                }
            }
            commands = map.GetCommands(); //Work our way backwards to find a command that matches our input
            return commands != null;
        }
        /// <summary>
        /// Determines whether [is white space] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is white space] [the specified c]; otherwise, <c>false</c>.</returns>
        private static bool IsWhiteSpace(char c) => c == ' ' || c == '\n' || c == '\r' || c == '\t';

        //TODO: Check support for escaping
        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="startPos">The start position.</param>
        /// <param name="command">The command.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Nullable&lt;CommandErrorType&gt;.</returns>
        public static CommandErrorType? ParseArgs(string input, int startPos, Command command, out string[] args)
        {
            ParserPart currentPart = ParserPart.None;
            int startPosition = startPos;
            int endPosition = startPos;
            int inputLength = input.Length;
            bool isEscaped = false;

            CommandParameter[] expectedArgs = command._parameters;
            List<string> argList = new List<string>();
            CommandParameter parameter = null;

            args = null;

            if (input == "")
                return CommandErrorType.InvalidInput;

            while (endPosition < inputLength)
            {
                if (startPosition == endPosition && (parameter == null || parameter.Type != ParameterType.Multiple)) //Is first char of a new arg
                {
                    if (argList.Count >= expectedArgs.Length)
                        return CommandErrorType.BadArgCount; //Too many args
                    parameter = expectedArgs[argList.Count];
                    if (parameter.Type == ParameterType.Unparsed)
                    {
                        argList.Add(input.Substring(startPosition));
                        break;
                    }
                }

                char currentChar = input[endPosition++];
                if (isEscaped)
                    isEscaped = false;
                else if (currentChar == '\\')
                    isEscaped = true;

                bool isWhitespace = IsWhiteSpace(currentChar);
                if (endPosition == startPosition + 1 && isWhitespace) //Has no text yet, and is another whitespace
                {
                    startPosition = endPosition;
                    continue;
                }

                switch (currentPart)
                {
                    case ParserPart.None:
                        if ((!isEscaped && currentChar == '\"'))
                        {
                            currentPart = ParserPart.DoubleQuotedParameter;
                            startPosition = endPosition;
                        }
                        else if ((!isEscaped && currentChar == '\''))
                        {
                            currentPart = ParserPart.QuotedParameter;
                            startPosition = endPosition;
                        }
                        else if ((!isEscaped && isWhitespace) || endPosition >= inputLength)
                        {
                            int length = (isWhitespace ? endPosition - 1 : endPosition) - startPosition;
                            if (length == 0)
                                startPosition = endPosition;
                            else
                            {
                                string temp = input.Substring(startPosition, length);
                                argList.Add(temp);
                                currentPart = ParserPart.None;
                                startPosition = endPosition;
                            }
                        }
                        break;
                    case ParserPart.QuotedParameter:
                        if ((!isEscaped && currentChar == '\''))
                        {
                            string temp = input.Substring(startPosition, endPosition - startPosition - 1);
                            argList.Add(temp);
                            currentPart = ParserPart.None;
                            startPosition = endPosition;
                        }
                        else if (endPosition >= inputLength)
                            return CommandErrorType.InvalidInput;
                        break;
                    case ParserPart.DoubleQuotedParameter:
                        if ((!isEscaped && currentChar == '\"'))
                        {
                            string temp = input.Substring(startPosition, endPosition - startPosition - 1);
                            argList.Add(temp);
                            currentPart = ParserPart.None;
                            startPosition = endPosition;
                        }
                        else if (endPosition >= inputLength)
                            return CommandErrorType.InvalidInput;
                        break;
                }
            }

            //Unclosed quotes
            if (currentPart == ParserPart.QuotedParameter ||
                currentPart == ParserPart.DoubleQuotedParameter)
                return CommandErrorType.InvalidInput;

            //Too few args
            for (int i = argList.Count; i < expectedArgs.Length; i++)
            {
                CommandParameter param = expectedArgs[i];
                switch (param.Type)
                {
                    case ParameterType.Required:
                        return CommandErrorType.BadArgCount;
                    case ParameterType.Optional:
                    case ParameterType.Unparsed:
                        argList.Add("");
                        break;
                }
            }

            /*if (argList.Count > expectedArgs.Length)
			{
				if (expectedArgs.Length == 0 || expectedArgs[expectedArgs.Length - 1].Type != ParameterType.Multiple)
					return CommandErrorType.BadArgCount;
            }*/

            args = argList.ToArray();
            return null;
        }
    }
}

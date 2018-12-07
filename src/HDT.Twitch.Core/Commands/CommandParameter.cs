namespace HDT.Twitch.Core.Commands
{
    /// <summary>
    /// Enum ParameterType
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// Catches a single required parameter.
        /// </summary>
        Required,
        /// <summary>
        /// Catches a single optional parameter.
        /// </summary>
        Optional,
        /// <summary>
        /// Catches a zero or more optional parameters.
        /// </summary>
        Multiple,
        /// <summary>
        /// Catches all remaining text as a single optional parameter.
        /// </summary>
        Unparsed
    }
    /// <summary>
    /// Class CommandParameter.
    /// </summary>
    public class CommandParameter
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; internal set; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public ParameterType Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        internal CommandParameter(string name, ParameterType type)
        {
            Name = name;
            Type = type;
        }
    }
}

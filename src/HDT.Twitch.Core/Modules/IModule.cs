namespace HDT.Twitch.Core.Modules
{
    /// <summary>
    /// Interface IModule
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Installs the specified manager.
        /// </summary>
        /// <param name="manager">The manager.</param>
        void Install(ModuleManager manager);
    }
}

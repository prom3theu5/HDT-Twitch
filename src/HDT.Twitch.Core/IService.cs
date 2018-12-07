namespace HDT.Twitch.Core
{
    /// <summary>
    /// Interface IService
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Installs the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        void Install(Client client);
    }
}
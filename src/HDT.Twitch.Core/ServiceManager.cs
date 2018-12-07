using System;
using System.Collections;
using System.Collections.Generic;

namespace HDT.Twitch.Core
{
    /// <summary>
    /// Class ServiceCollection.
    /// Implements the <see cref="System.Collections.Generic.IEnumerable{HDT.Twitch.Core.IService}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{HDT.Twitch.Core.IService}" />
    internal class ServiceCollection : IEnumerable<IService>
    {
        /// <summary>
        /// The services
        /// </summary>
        private readonly Dictionary<Type, IService> _services;

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        internal Client Client { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCollection"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        internal ServiceCollection(Client client)
        {
            Client = client;
            _services = new Dictionary<Type, IService>();
        }

        /// <summary>
        /// Adds the specified service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns>T.</returns>
        public T Add<T>(T service)
            where T : class, IService
        {
            _services.Add(typeof(T), service);
            service.Install(Client);
            return service;
        }

        /// <summary>
        /// Gets the specified is required.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>T.</returns>
        /// <exception cref="InvalidOperationException">This operation requires {typeof(T).Name} to be added to {nameof(Client)}</exception>
        public T Get<T>(bool isRequired = true)
            where T : class, IService
        {
            T singletonT = null;

            if (_services.TryGetValue(typeof(T), out IService service))
                singletonT = service as T;

            if (singletonT == null && isRequired)
                throw new InvalidOperationException($"This operation requires {typeof(T).Name} to be added to {nameof(Client)}.");
            return singletonT;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IService> GetEnumerator() => _services.Values.GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _services.Values.GetEnumerator();
    }
}
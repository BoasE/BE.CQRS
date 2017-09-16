using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using BE.CQRS.Data.GetEventStore;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using RepositorySamples.EventStore.Domain;

namespace RepositorySamples.EventStore
{
    internal static class Bootstrap
        
    {
        public const string Prefix= "Sample1";

        internal static EventStoreRepository CreateDomainObjectRepository()
        {

          // Prefix is used to seperate different tennants or applications withing one geteventstore

            IEventStoreConnection connection = ConnectToGetEventStoreDatabase(); // Get a Connection to the GetEventStore Database

            var activator = new ActivatorDomainObjectActivator(); // Responsible for creating Domain objects, see BE.CQRS.Di.* Packages for more possibilities or implement your own one.
            

            // Context holds all dependencies that are used by the DomainObjectRepository
            var context = EventStoreContext.CreateDefault(Prefix, connection, activator); 

            var repo = new EventStoreRepository(context);

            return repo;
        }


        internal static IEventStoreConnection ConnectToGetEventStoreDatabase()
        {
            var creds = new UserCredentials("admin","changeit");
            
            var settings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(creds)
                .SetHeartbeatInterval(TimeSpan.FromSeconds(10))
                .Build();

            IEventStoreConnection connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113));
            connection.ConnectAsync().Wait();    

            return connection;
        }
    }
}

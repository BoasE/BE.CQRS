using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore.IntegrationTests
{
    public static class LocalConfig
    {
        public static string EventStoreIp { get; set; } = "127.0.0.1";

        public static int EventStorePort { get; set; } = 1113;

        public static async Task<IEventStoreConnection> GetConnection()
        {
            ConnectionSettings settings = ConnectionSettings.Create().KeepReconnecting().KeepRetrying().Build();
            IEventStoreConnection result = EventStoreConnection.Create(settings,
                new IPEndPoint(IPAddress.Parse(EventStoreIp), EventStorePort));
            await result.ConnectAsync();
            return result;
        }
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AspNetCoreSample
{
    public static class MongoDbBooter
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
        {
            var client = new MongoClient(config["CustomerDatabase:MongoDb:Host"]);
            IMongoDatabase db = client.GetDatabase(config["CustomerDatabase:MongoDb:Name"]);

            services
                .AddSingleton(db)
                .AddSingleton<IMongoClient>(client);
            return services;
        }
    }
}
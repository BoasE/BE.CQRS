# CQRS

## Common
This project represents a interpretation of the CQRS pattern, setting the focus on domainobjects, states and repositories.
Although this project is already used in some real world projects like [groupl](https://www.groupl.de) I can't provide you any guarantues on functionality or support.

Currently a process of migration, documentation and samples creation is in progress. If you have any questions or wished please drop an issue here.

This project doesn't claim to be a prefect cqrs implementation. As most patterns CQRS has also many different real world interpretations.

## Persistance
Variant persistance implementations can be achieved by subclassing the domain object repository base. 
Currently two databases are implemented:
- [EventStore](https://geteventstore.com/)
- [Mongodb](https://www.mongodb.com/)

The EventStore implementation is already used in production scenarios , the mongodb is currentlyin a exeperimentell state.

## Samples
To get started have a look at the sample directory.

### Adding the write part

#### MongoDb as EventStore, and asp.core serviceprovider for di
```csharp
public static void AddWrite(this IServiceCollection collection, IConfiguration config)
{
    var mongoDb = GetEsMongoDbFromConfig(config);

    collection.AddEventSource(
        new EventSourceConfiguration()
            .SetDomainObjectAssemblies(typeof(ChildDomainObject).Assembly)
            .SetServiceProviderActivator()
            .SetMongoDomainObjectRepository(mongoDb)
            .SetInMemoryCommandBus());
}

public static void UseWrite(this IApplicationBuilder app)
{
    app.UseServiceProviderActivator();
}
```


### Adding the denormalizers

```csharp
public static void AddCqrsDenormalizer(this IServiceCollection collection, IConfiguration config)
{
    var eventDb = GetEsMongoDbFromConfig(config);
    var streamPosDb = GetStreamPositionMongoDbFromConfig(config);

    collection.AddDenormalizers(
        new DenormalizerConfiguration()
            .SetDenormalizerAssemblies(typeof(ChildDenormalizer).Assembly)
            .SetMongoEventPositionGateway(streamPosDb)
            .SetMongoDbEventSubscriber(eventDb)
            .SetServiceProviderDenormalizerActivator()
            ); 
}

public static async Task<IApplicationBuilder> UseCqrsDenormalizerAsync(this IApplicationBuilder app, IServiceProvider provider)
{
    EventDenormalizer denormalizer = app.UseConvetionBasedDenormalizer();
    await denormalizer.StartAsync(TimeSpan.FromMilliseconds(250));

    return app;
}
     
```
## Ressources
To get started I strongly recommend to have a look at the awesome CQRS Webcasts by GregYoung.


## Key-Concepts
### DomainObject
The DomainObject is the center of each application logic. Each

### DomainObjectRepository
The DomainObjectRepository is responsible for persisting and reading the eventstreams of the domainobject.

### States
States are visitors that are iterating over stream of events to determine a desired state.

### Policies
Policies are specialized states which result in a boolean value. For example "IsCustomerActiveState"

### CommandBus
CommandBus is used so send commands and in order to find their related domainobjects and processes that can handle the given command

### EventSubscribers
EventSubscriber connect to eventstreams and provide notifications on new events

### Denormalizer
Denormalizers are working with eventsubscriber and are publishing new events to they registered eventhandlers e.g. for projecting informations in databases.


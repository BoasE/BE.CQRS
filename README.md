# CQRS

## Common
This project represents a interpretation of the CQRS pattern, setting the focus on domainobjects, states and repositories.
Although this project is already used in some real world projects like [groupl](https://www.groupl.de) I can't provide you any guarantues on functionality or support.

Currently a process of migration, documentation and samples creation is in progress. If you have any questions or wished please drop an issue here.

This project doesn't claim to be a prefect cqrs implementation. As most patterns CQRS has also many different real world interpretations.


## Usage-Example

This is how a very basic domainobject looks like

```csharp
public sealed class SessionDomainObject : DomainObjectBase
    {
        private readonly ILessonFactory lessonFactory;
        
        public SessionDomainObject(string id,ILessonFactory lessonFactory) : base(id,null)
        {
            Console.WriteLine("Creating object");

            this.lessonFactory = lessonFactory;
        }

        [Create]
        public void CreateSession(StartSessionForUserCommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).IsValidModel();
            
            ILesson lesson = lessonFactory.FromShortKey(cmd.LessonKey);

            IWorksheet worksheet = lesson.CreateWorksheet(new WorksheetArgument());
            
            
            RaiseEvent<SessionCreatedEvent>(@event =>
            {
                @event.Started = DateTimeOffset.Now;
                @event.UserId = cmd.UserId;
                @event.LessonKey = cmd.LessonKey;
                @event.WorksheetItems = worksheet.Items.ToEvent()
            });
        }
    }
```

Following an example for an api controler which triggers the creation process. The bus sends the command to a handler which creates the domain object, processes the events and persists the result


```csharp
[HttpPost]
public async Task<IActionResult> Post([FromBody][Required] StartSessionModel model)
{
    Precondition.For(model, nameof(model)).NotNull().IsValidModel();

   await bus.EnqueueAsync(new StartSessionForUserCommand(Guid.NewGuid().ToString(), model.LessonKey, userId));

    return Accepted();
}
```` 


## Persistance
Variant persistance implementations can be achieved by subclassing the domain object repository base. 
Currently two databases are implemented:
- [EventStore](https://geteventstore.com/)
- [Mongodb](https://www.mongodb.com/)

The EventStore implementation is already used in production scenarios , the mongodb is currentlyin a exeperimentell state.

## Configuration-Examples
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
The DomainObject is the center of each application logic. A very basic example is the [Customer](Samples/1_NET_Core/NetCoreConsoleSample.Domain/Customer.cs) Domain Object in our first sample. 

### DomainObjectRepository
The DomainObjectRepository is responsible for persisting and reading the eventstreams of the domainobject. A very basic example is the usage of the `MongoDomainObjectRepository` which is registered first in the [CQRSBooter](Samples/1_NET_Core/NetCoreConsoleSample/CQRSBooter.cs#L16) in our first sample. Its usage is shown in the [Program.cs](Samples/1_NET_Core/NetCoreConsoleSample/Program.cs#L29) of the same sample.

### States
States are visitors that are iterating over stream of events to determine a desired state.
The [NameState](Samples/1_NET_Core/NetCoreConsoleSample.Domain/States/NameState.cs) for the `Customer` Domain Object determines the `Name` State for a customer. Accessing the `NameState` is also shown in the first sample's [Program.cs](Samples/1_NET_Core/NetCoreConsoleSample/Program.cs#L31).

### Policies
Policies are specialized states which result in a boolean value. For example "IsCustomerActiveState"

### CommandBus
CommandBus is used to send commands and in order to find their related Domain Objects and Processes that can handle the given Command. The CommandBus is registered in the [CQRSBooter.cs](Samples/2_ASPNET_Core/AspNetCoreSample/CQRSBooter.cs#L28) of our second sample. After registration, it can be accessed in the [CustomersController](Samples/2_ASPNET_Core/AspNetCoreSample/Controllers/CustomersController.cs#L31) and enqueue Commmands.

### EventSubscribers
EventSubscriber connect to eventstreams and provide notifications on new events. The [MongoDbEventSubscriber] is registered in the [CQRSBooter.cs](Samples/3_ASPNET_Core_ReadModels/AspNetCoreSample/CQRSBooter.cs#L54) of our third sample.

### Denormalizer
Denormalizers are working with eventsubscriber and are publishing new events to they registered eventhandlers e.g. for projecting informations in databases. By registering the [CustomerDenormalizer](Samples/3_ASPNET_Core_ReadModels/AspNetCoreSample.Denormalizer/CustomerDenormalizer.cs) in the [CQRSBooter](Samples/3_ASPNET_Core_ReadModels/AspNetCoreSample/CQRSBooter.cs#L74) it can subscribe to the [Notifications](Samples/3_ASPNET_Core_ReadModels/AspNetCoreSample.Denormalizer/CustomerDenormalizer.cs#L18) of the `MongoDbEventSubscriber` and create a [CustomerReadModel](Samples/3_ASPNET_Core_ReadModels/AspNetCoreSample.Denormalizer/CustomerDenormalizer.cs#L22).


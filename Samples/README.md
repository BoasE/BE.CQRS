# Samples
Here you will find samples showing how to setup and use the cqrs framework. 

The samples are seperated in MongoDb and GetEventstore. Both do the same and they only differ in the kind of persistance, which is determined by using either the MongoDbDomainObjectRepository or the geteventstore repository

## GetEventstore
For Mongodb you need a running mongodb database from https://www.mongodb.com
When running examples  alsoneed to have a projection in the geteventstore matching the prefix of the domain object repository you set in the code. The code will to this for you but be aware that this is required when you setup a project on your own.
The easiest way to get eventstore started is to use its dockerimage -> https://hub.docker.com/r/eventstore/eventstore/

```docker run --name eventstore-node -it -p 2113:2113 -p 1113:1113 eventstore/eventstore```


## MongoDb
For GetEventstore repository you need a running server which you can get from https://geteventstore.com/

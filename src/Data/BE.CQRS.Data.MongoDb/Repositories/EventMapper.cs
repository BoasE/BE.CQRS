using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BE.CQRS.Data.MongoDb.Commits;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;

namespace BE.CQRS.Data.MongoDb.Repositories
{
    public sealed class EventMapper
    {
        private readonly IEventSerializer serializer;
        private readonly IEventHash eventHash;

        public EventMapper(IEventSerializer serializer, IEventHash hash)
        {
            Precondition.For(hash, nameof(hash)).NotNull();
            Precondition.For(serializer, nameof(serializer)).NotNull();
            this.serializer = serializer;
            this.eventHash = hash;
        }

        public EventCommit ToCommit(EventCommit target,string domainobjectId, Type domainObjectType, long originVersion,
            long commitVersion,
            IList<IEvent> events)
        {
            Dictionary<string, EventDto> items = MapEvents(domainobjectId, events);

            target.AggregateId = domainobjectId;
            target.AggregateType = domainObjectType.FullName;
            target.AggregateTypeShort = domainObjectType.Name;
            target.AggregatePackage = domainObjectType.GetTypeInfo().Assembly.GetName().Name;
            target.Ordinal = Timestamp.FromNow();
            target.Timestamp = DateTime.UtcNow;
            target.VersionEvents = originVersion + events.Count;
            target.VersionCommit = commitVersion;
            target.Events = items;


            PreAggregateEventTypes(events, target);

            return target;
        }

        private static void PreAggregateEventTypes(IEnumerable<IEvent> events, EventCommit commit)
        {
            commit.AllEventTypes = events
                .Select(x => x.Headers.GetString(EventHeaderKeys.AssemblyEventType))
                .Distinct()
                .ToList();
        }

        public IEnumerable<IEvent> ExtractEvents(EventCommit commit)
        {
            foreach (KeyValuePair<string, EventDto> @event in commit.Events)
            {
                @event.Value.Headers.Add(EventHeaderKeys.AggregateId, commit.AggregateId);
                @event.Value.Headers.Add(EventHeaderKeys.AggregateType, commit.AggregateType);
                @event.Value.Headers.Add(EventHeaderKeys.CommitId, commit.Ordinal.ToString());
                yield return serializer.DeserializeEvent(@event.Value.Headers, @event.Value.Body);
            }
        }

        private Dictionary<string, EventDto> MapEvents(string domainObjectId, IList<IEvent> events)
        {
            var items = new Dictionary<string, EventDto>(events.Count);

            for (var i = 0; i < events.Count; i++)
            {
                IEvent @event = events[i];
                if (!domainObjectId.Equals(@event.Headers.AggregateId))
                    throw new InvalidOperationException("Domainobject id did not match!");

                string content = serializer.SerializeEvent(@event);

                var dto = new EventDto
                {
                    Body = content,
                    Headers = @event.Headers.ToDictionary(),
                    Id = @event.Headers.GetString(EventHeaderKeys.EventId)
                };

                var bodyHash = eventHash.HashString(dto.Body);

                string header = string.Join('-', dto.Headers.Values);
                var headerHash = eventHash.HashString(header);

                dto.Headers.Add(EventHeaderKeys.BodyHash, bodyHash);
                dto.Headers.Add(EventHeaderKeys.HeaderHash, headerHash);

                items.Add(i.ToString(), dto);
            }

            return items;
        }
    }
}
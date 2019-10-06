using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Policies;
using BE.CQRS.Domain.States;

namespace BE.CQRS.Domain.DomainObjects
{
    public interface IDomainObject
    {
        string Id { get; }

        bool CheckVersionOnSave { get; }

        bool HasUncommittedEvents { get; }

        string Namespace { get; }

        long Version { get; }

        long CommitVersion { get; }

        long OriginVersion { get; }

        IReadOnlyCollection<IEvent> GetUncommittedEvents();

        IReadOnlyCollection<IEvent> GetCommittedEvents();

        void CommitChanges(long commitVersion);

        void RevertChanges();

        Task ApplyEvents(IAsyncEnumerable<IEvent> eventsToCommit, ISet<Type> allowedEventTypes = null);

        void ApplyConfig(EventSourceConfiguration configuration);

        bool Policy<T>() where T : PolicyBase, new();

        bool Policy<T>(ICommand command) where T : PolicyBase;

        bool Policy(Type policy, ICommand command);

        T State<T>() where T : StateBase, new();

        T State<T>(bool includeUnComitted) where T : StateBase, new();
    }
}
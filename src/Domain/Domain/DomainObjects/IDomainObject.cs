using System;
using System.Collections.Generic;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Policies;

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

        void CommitChanges(long commitVersion);

        void RevertChanges();

        void ApplyEvents(ICollection<IEvent> eventsToCommit);

        bool Policy<T>() where T : PolicyBase, new();

        bool Policy<T>(ICommand command) where T : PolicyBase;

        bool Policy(Type policy, ICommand command);
    }
}
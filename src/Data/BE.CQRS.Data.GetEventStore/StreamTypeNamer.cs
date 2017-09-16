using System;
using System.Text;
using BE.CQRS.Domain.DomainObjects;
using BE.FluentGuard;

namespace BE.CQRS.Data.GetEventStore
{
    public sealed class StreamTypeNamer : IStreamNamer
    {
        private readonly string prefix;
        private readonly string seperator = "-";

        public StreamTypeNamer(string prefix)
        {
            Precondition.For(prefix, nameof(prefix)).NotNullOrWhiteSpace();

            this.prefix = prefix;
        }

        public string Resolve(Type domainObject, string id, string @namespace = null)
        {
            Precondition.For(id, nameof(id)).NotNullOrWhiteSpace();

            string result = BuildKey(domainObject, id, @namespace);

            return result;
        }

        public string Resolve<T>(T domainObject) where T : class, IDomainObject
        {
            Precondition.For(domainObject, nameof(domainObject)).NotNull();
            Precondition.For(domainObject.Id, nameof(domainObject.Id)).NotNullOrWhiteSpace();

            string result = BuildKey(domainObject.GetType(), domainObject.Id, domainObject.Namespace);

            return result;
        }

        private string BuildKey(Type domainObject, string id, string @namespace = null)
        {
            var builder = new StringBuilder(prefix);

            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                builder.Append(seperator);
                builder.Append(@namespace);
            }

            builder.Append(seperator).Append(domainObject.Name).Append(seperator).Append(id);

            return builder.ToString();
        }
    }
}
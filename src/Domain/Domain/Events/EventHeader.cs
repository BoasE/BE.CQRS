using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace BE.CQRS.Domain.Events
{
    public sealed class EventHeader
    {
        private readonly ConcurrentDictionary<string, string> values = new ConcurrentDictionary<string, string>();

        public string AssemblyEventType => GetString(EventHeaderKeys.AssemblyEventType);

        public Guid EventId => GetGuid(EventHeaderKeys.EventId);

        public string AggregateId => GetString(EventHeaderKeys.AggregateId);

        public DateTimeOffset Created => GetDateTimeOffset(EventHeaderKeys.Created);

        public EventHeader()
        {
        }

        public EventHeader(IDictionary<string, string> dictionary)
        {
            values = new ConcurrentDictionary<string, string>(dictionary);
        }

        public void ApplyEventHeader(EventHeader other)
        {
            foreach (KeyValuePair<string, string> entry in other.values)
            {
                values.AddOrUpdate(entry.Key, entry.Value, (a, b) => entry.Value);
            }
        }

        public Type ResolveEventType()
        {
            string name = GetString(EventHeaderKeys.EventType);

            return Type.GetType(name);
        }

        public void Set(string key, string value)
        {
            SetValue(key, value, x => x);
        }

        public string GetString(string key)
        {
            return ReadString(key);
        }

        public void SetDetaults(string id)
        {
            Set(EventHeaderKeys.AggregateId, id);
            Set(EventHeaderKeys.EventId, Guid.NewGuid());
            Set(EventHeaderKeys.Timestamp, DateTimeOffset.UtcNow); //Change to instant ! And support instant!
        }

        public void Set(string key, int value)
        {
            SetValue(key, value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        public int GetInteger(string key)
        {
            return ParseValue(key, v => int.Parse(v, CultureInfo.InvariantCulture));
        }

        public long GetLong(string key)
        {
            return ParseValue(key, v => long.Parse(v, CultureInfo.InvariantCulture));
        }

        public void Set(string key, float value)
        {
            SetValue(key, value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        public float GetReal(string key)
        {
            return ParseValue(key, v => float.Parse(v, CultureInfo.InvariantCulture));
        }

        public void Set(string key, Guid value)
        {
            SetValue(key, value, x => x.ToString());
        }

        public Guid GetGuid(string key)
        {
            return ParseValue(key, Guid.Parse);
        }

        public void Set(string key, TimeSpan value)
        {
            SetValue(key, value, x => x.ToString());
        }

        public TimeSpan GetTimeSpan(string key)
        {
            return ParseValue(key, v => TimeSpan.Parse(v, CultureInfo.InvariantCulture));
        }

        public void Set(string key, DateTime value)
        {
            SetValue(key, value, x => x.ToString("yyyy - MM - ddTHH:mm: ss.fffffffzzz", CultureInfo.InvariantCulture));
        }

        public void Set(string key, object value)
        {
            SetValue(key, value, x => string.Format(CultureInfo.InvariantCulture, "{0}", x));
        }

        public DateTime GetDateTime(string key)
        {
            return ParseValue(key,
                v => DateTime.Parse(v, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
        }

        public void Set(string key, DateTimeOffset value)
        {
            SetValue(key, value, x => x.ToString("yyyy - MM - ddTHH:mm: ss.fffffffzzz"));
        }

        public object Get(string key, Type type)
        {
            string value = ReadString(key);

            if (type == typeof(string))
            {
                return value;
            }
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (type == typeof(Guid))
                {
                    return Guid.Parse(value);
                }
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            return Activator.CreateInstance(type);
        }

        public DateTimeOffset GetDateTimeOffset(string key)
        {
            return ParseValue(key, v => DateTimeOffset.Parse(v, CultureInfo.InvariantCulture));
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>(values);
        }

        private void SetValue<T>(string key, T value, Func<T, string> write)
        {
            values[key] = write(value);
        }

        private string ReadString(string key)
        {
            return values[key];
        }

        private T ParseValue<T>(string key, Func<string, T> parse) where T : struct
        {
            if (!values.TryGetValue(key, out string value))
                throw new KeyNotFoundException($"{key} was not found");

            T result;

            try
            {
                result = parse(value);
            }
            catch
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture,
                    "The value in the header is not a valid {0}", typeof(T)));
            }

            return result;
        }

        public bool HasKey(string key)
        {
            return values.ContainsKey(key);
        }
    }
}
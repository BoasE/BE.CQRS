namespace BE.CQRS.Domain.Events
{
    public sealed class SubstitutionEvent : EventBase
    {
        public string StreamName { get; }

        public long StreamPos { get; }

        public byte[] RawData { get; }

        public byte[] RawHeader { get; }

        public SubstitutionEvent(string streamName, long pos, byte[] rawHeader, byte[] rawData)
        {
            StreamName = streamName;
            StreamPos = pos;
            RawData = rawData;
            RawHeader = rawHeader;
        }
    }
}
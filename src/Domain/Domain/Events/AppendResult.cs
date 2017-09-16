namespace BE.CQRS.Domain.Events
{
    public struct AppendResult
    {
        public long CurrentVersion { get; }

        public bool HadWrongVersion { get; }

        public AppendResult(bool wrongVersion, long currentVersion)
        {
            CurrentVersion = currentVersion;
            HadWrongVersion = wrongVersion;
        }

        public static AppendResult NoUpdate => new AppendResult(false, 0);

        public static AppendResult WrongVersion(long version)
        {
            return new AppendResult(true, 0);
        }
    }
}
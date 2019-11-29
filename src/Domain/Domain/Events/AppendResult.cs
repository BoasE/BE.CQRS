namespace BE.CQRS.Domain.Events
{
    public struct AppendResult
    {
        public string CommitId { get; }

        public long CurrentVersion { get; }

        public bool HadWrongVersion { get; }

        public AppendResult(string commitId, bool wrongVersion, long currentVersion)
        {
            CommitId = commitId;
            CurrentVersion = currentVersion;
            HadWrongVersion = wrongVersion;
        }

        public static AppendResult NoUpdate => new AppendResult("",false, 0);

        public static AppendResult WrongVersion(long version)
        {
            return new AppendResult("",true, 0);
        }
    }
}
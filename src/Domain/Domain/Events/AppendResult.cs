namespace BE.CQRS.Domain.Events
{
    public struct AppendResult
    {
        public string CommitId { get; }

        public long CurrentVersion { get; }

        public bool HadWrongVersion { get; }

        public string Message { get; }

        public AppendResult(string commitId, bool wrongVersion, long currentVersion,string message)
        {
            CommitId = commitId;
            CurrentVersion = currentVersion;
            HadWrongVersion = wrongVersion;
            Message = message;
        }

        public static AppendResult NoUpdate => new AppendResult("",false, 0,"NoUpdate");

        public static AppendResult WrongVersion(long version)
        {
            return new AppendResult("",true, 0,"Wrong Version");
        }
    }
}
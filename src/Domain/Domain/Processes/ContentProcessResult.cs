namespace BE.CQRS.Domain.Processes
{
    public class ContentProcessResult<TContent> : ProcessResult
    {
        public TContent Content { get; }

        private ContentProcessResult(bool success, bool notAllowed, bool notfound, TContent content) : base(success,
            notAllowed, notfound)
        {
            Content = content;
        }

        public static ContentProcessResult<TContent> ByContent(TContent content)
        {
            return new ContentProcessResult<TContent>(true, false, false, content);
        }
    }
}
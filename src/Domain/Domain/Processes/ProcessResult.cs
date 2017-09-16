namespace BE.CQRS.Domain.Processes
{
    public class ProcessResult
    {
        public bool WasSuccessful { get; }

        public bool WasForbidden { get; }

        public bool WasNotFound { get; }

        protected ProcessResult(bool success, bool notAllowed, bool notFound)
        {
            WasSuccessful = success;
            WasForbidden = notAllowed;
            WasNotFound = notFound;
        }

        public static ProcessResult Forbidden()
        {
            return new ProcessResult(false, true, false);
        }

        public static ProcessResult Success()
        {
            return new ProcessResult(true, false, false);
        }

        public static ProcessResult NotFound()
        {
            return new ProcessResult(false, false, true);
        }
    }
}
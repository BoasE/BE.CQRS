using System;
using System.Net;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Processes;

namespace AspCore
{
    public static class StatusCodeConverter
    {
        public static HttpStatusCode From(AppendResult result)
        {
            return result.HadWrongVersion ? HttpStatusCode.Conflict : HttpStatusCode.Accepted;
        }

        public static HttpStatusCode From(ProcessResult result)
        {
            if (result.WasSuccessful)
            {
                return HttpStatusCode.Accepted;
            }
            else if (result.WasNotFound)
            {
                return HttpStatusCode.NotFound;
            }

            else if (result.WasForbidden)
            {
                return HttpStatusCode.Forbidden;
            }

            throw new InvalidOperationException();
        }
    }
}
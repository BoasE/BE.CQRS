using System.Net;
using BE.CQRS.Domain.Events;
using Xunit;

namespace AspCore.Tests
{
    public class GivenStatusCodeConverter
    {
        [Fact]
        public void AcceptedWhenNoError()
        {
            var result = new AppendResult("1", false, 14,"SUCCESS");

            HttpStatusCode code = StatusCodeConverter.From(result);

            Assert.Equal(HttpStatusCode.Accepted, code);
        }

        [Fact]
        public void ConflictWhenVersionError()
        {
            var result = new AppendResult("", true, 14,"WRONGVERSION");

            HttpStatusCode code = StatusCodeConverter.From(result);

            Assert.Equal(HttpStatusCode.Conflict, code);
        }
    }
}
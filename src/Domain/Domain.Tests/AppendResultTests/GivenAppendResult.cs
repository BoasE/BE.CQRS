using BE.CQRS.Domain.Events;
using Xunit;

namespace BE.CQRS.Domain.Tests.AppendResultTests
{
    public class GivenAppendResult
    {
        public AppendResult GetSut(int version)
        {
            return new AppendResult("", false, version,"");
        }

        [Fact]
        public void ItAppliesTheVersion()
        {
            AppendResult result = GetSut(15);
            Assert.Equal(15, result.CurrentVersion);
        }
    }
}
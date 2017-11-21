using System;
using Xunit;

namespace BE.CQRS.Domain.Tests.StateTests
{
    public class WhenVisiting : GivenState
    {
        [Fact]
        public void ItThrowsWhenNull()
        {
            SampleState sut = GetSut();

            Assert.Throws<ArgumentNullException>(() => sut.Execute(null));
        }

    }
}
using System;
using EventStore.ClientAPI;
using FakeItEasy;
using Xunit;

namespace BE.CQRS.Data.GetEventStore.Tests.RepositoryTests.SaveTests
{
    public class WhenCreating : GivenEventStore
    {
        private const string Prefix = "GivenEventStore.WhenCreating.";

        [Fact(DisplayName = Prefix + nameof(ItThrowsWhenPrefixIsEmpty))]
        public void ItThrowsWhenPrefixIsEmpty()
        {
            var con = A.Fake<IEventStoreConnection>();
            Assert.Throws<ArgumentException>(() => GetSut("", con));
        }

        [Fact(DisplayName = Prefix + nameof(ItThrowsWhenPrefixIsNull))]
        public void ItThrowsWhenPrefixIsNull()
        {
            var con = A.Fake<IEventStoreConnection>();
            Assert.Throws<ArgumentNullException>(() => GetSut(null, con));
        }

        [Fact(DisplayName = Prefix + nameof(ItThrowsWhenConnectionIsNull))]
        public void ItThrowsWhenConnectionIsNull()
        {
            var con = A.Fake<IEventStoreConnection>();
            Assert.Throws<ArgumentNullException>(() => GetSut("foo", null));
        }
    }
}
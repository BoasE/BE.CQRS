using System;
using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Tests.EventBaseTests;
using Xunit;

namespace BE.CQRS.Domain.Tests.EventHeaderTests
{
    public class GivenEventHeader
    {
        private readonly string expectedId = "1233sf33";

        protected EventHeader GetSut()
        {
            var sut = new EventHeader();
            sut.Set(EventHeaderKeys.EventType, typeof(SampleEvent).AssemblyQualifiedName);
            sut.Set(EventHeaderKeys.AggregateId, expectedId);
            return sut;
        }

        [Fact]
        public void ItResolvesAggregateId()
        {
            EventHeader sut = GetSut();
            string id = sut.AggregateId;

            Assert.Equal(id, expectedId);
        }

        [Fact]
        public void GetEventType()
        {
            EventHeader sut = GetSut();
            Type type = sut.ResolveEventType();

            Assert.Equal(type, typeof(SampleEvent));
        }

        [Fact]
        public void TheDicationaryHasValues()
        {
            EventHeader sut = GetSut();

            var expected = "sdmnfö2342ütäerfd";
            sut.Set("foo", expected);

            Dictionary<string, string> values = sut.ToDictionary();

            Assert.Equal(3, values.Count);
        }

        [Fact]
        public void GettingStringAsDatetimeThrows()
        {
            EventHeader sut = GetSut();

            var expected = "sdmnfö2342ütäerfd";
            sut.Set("foo", expected);

            Assert.Throws<InvalidCastException>(() => sut.GetDateTime("foo"));
        }

        [Fact]
        public void HasKeyNotfound()
        {
            EventHeader sut = GetSut();

            Assert.False(sut.HasKey("abb"));
        }

        [Fact]
        public void HasKeyfound()
        {
            EventHeader sut = GetSut();
            sut.Set("foo", "dfsdf");
            Assert.True(sut.HasKey("foo"));
        }

        [Fact]
        public void ItCanSetAndGetString()
        {
            EventHeader sut = GetSut();

            var expected = "sdmnfö2342ütäerfd";
            sut.Set("foo", expected);

            string result = sut.GetString("foo");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ItCanSetAndGetInt()
        {
            EventHeader sut = GetSut();

            var expected = 123;
            sut.Set("foo", expected);

            int result = sut.GetInteger("foo");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ItCanSetAndGetFloat()
        {
            EventHeader sut = GetSut();

            var expected = 123f;
            sut.Set("foo", expected);

            float result = sut.GetReal("foo");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ItCanSetAndGetDateTime()
        {
            DateTime expected = DateTime.Now;

            EventHeader sut = GetSut();
            sut.Set("foo", expected);

            DateTime result = sut.GetDateTime("foo");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ItCanSetAndGetTimespan()
        {
            TimeSpan expected = TimeSpan.FromHours(2);

            EventHeader sut = GetSut();
            sut.Set("foo", expected);

            TimeSpan result = sut.GetTimeSpan("foo");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ItCanSetAndGetlongs()
        {
            long expected = 123;
            EventHeader sut = GetSut();
            sut.Set("foo", expected);

            long result = sut.GetLong("foo");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ItCanSetAndGetDateTimeOffset()
        {
            DateTimeOffset expected = DateTimeOffset.Now;

            EventHeader sut = GetSut();
            sut.Set("foo", expected);

            DateTimeOffset result = sut.GetDateTimeOffset("foo");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetByGenericType()
        {
            long expected = 213;

            EventHeader sut = GetSut();
            sut.Set("foo", expected);

            var result = (long)sut.Get("foo", expected.GetType());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetStringByType()
        {
            var expected = "213";

            EventHeader sut = GetSut();
            sut.Set("foo", expected);

            var result = (string)sut.Get("foo", expected.GetType());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetGuidByType()
        {
            Guid expected = Guid.NewGuid();

            EventHeader sut = GetSut();
            sut.Set("foo", expected);

            var result = (Guid)sut.Get("foo", expected.GetType());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetNullByType()
        {
            string expected = null;

            EventHeader sut = GetSut();
            sut.Set("foo", expected);

            var result = sut.Get("foo", typeof(string)) as string;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryGettingNonExistingValueThrows()
        {
            EventHeader sut = GetSut();
            Assert.Throws<KeyNotFoundException>(() => sut.GetLong("foo"));
        }
    }
}
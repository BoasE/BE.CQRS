using System;
using BE.CQRS.Domain.Serialization;
using Xunit;

namespace BE.CQRS.Domain.Tests.EventSerializerTests
{
    public class GivenEventSerializationException
    {
        [Fact]
        public void ItCanBeCreatedWithMessageOnly()
        {
            var expectedMessage = "Serialization failed";
            
            var sut = new EventSerializationException(expectedMessage);

            Assert.Equal(expectedMessage, sut.Message);
            Assert.Null(sut.EventData);
            Assert.Null(sut.EventType);
            Assert.Null(sut.AggregateId);
        }

        [Fact]
        public void ItCanBeCreatedWithMessageAndInnerException()
        {
            var expectedMessage = "Serialization failed";
            var innerException = new InvalidOperationException("Inner error");
            
            var sut = new EventSerializationException(expectedMessage, innerException);

            Assert.Equal(expectedMessage, sut.Message);
            Assert.Same(innerException, sut.InnerException);
            Assert.Null(sut.EventData);
            Assert.Null(sut.EventType);
            Assert.Null(sut.AggregateId);
        }

        [Fact]
        public void ItCanBeCreatedWithEventTypeAndAggregateId()
        {
            var expectedMessage = "Serialization failed";
            var expectedEventType = "MyNamespace.MyEvent";
            var expectedAggregateId = "aggregate-123";
            var innerException = new InvalidOperationException("Inner error");
            
            var sut = new EventSerializationException(
                expectedMessage, 
                expectedEventType, 
                expectedAggregateId, 
                innerException);

            Assert.Equal(expectedMessage, sut.Message);
            Assert.Equal(expectedEventType, sut.EventType);
            Assert.Equal(expectedAggregateId, sut.AggregateId);
            Assert.Same(innerException, sut.InnerException);
            Assert.Null(sut.EventData);
        }

        [Fact]
        public void ItCanBeCreatedWithAllProperties()
        {
            var expectedMessage = "Serialization failed";
            var expectedEventType = "MyNamespace.MyEvent";
            var expectedAggregateId = "aggregate-123";
            var expectedEventData = "{\"id\":\"123\",\"value\":\"test\"}";
            var innerException = new InvalidOperationException("Inner error");
            
            var sut = new EventSerializationException(
                expectedMessage, 
                expectedEventType, 
                expectedAggregateId, 
                expectedEventData,
                innerException);

            Assert.Equal(expectedMessage, sut.Message);
            Assert.Equal(expectedEventType, sut.EventType);
            Assert.Equal(expectedAggregateId, sut.AggregateId);
            Assert.Equal(expectedEventData, sut.EventData);
            Assert.Same(innerException, sut.InnerException);
        }

        [Fact]
        public void ItInheritsFromException()
        {
            var sut = new EventSerializationException("Test");

            Assert.IsAssignableFrom<Exception>(sut);
        }

        [Fact]
        public void ItCanBeCaught()
        {
            var caught = false;

            try
            {
                throw new EventSerializationException(
                    "Test error", 
                    "TestEvent", 
                    "agg-1", 
                    "{}", 
                    new Exception("Inner"));
            }
            catch (EventSerializationException ex)
            {
                caught = true;
                Assert.NotNull(ex);
            }

            Assert.True(caught);
        }
    }
}


using System.Reactive.Subjects;
using BE.CQRS.Domain.Commands;
using Xunit;

namespace BE.CQRS.Domain.Tests
{
    public abstract class CommandBaseTest
    {
        protected  ICommand Sut { get; }
        
        public CommandBaseTest()
        {
            Sut = GetSut();
        }
        protected abstract ICommand GetSut();

        [Fact]
        public void HasId()
        {
            Assert.NotNull(Sut.DomainObjectId);
        }
    }
}
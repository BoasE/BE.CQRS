using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.Tests
{
    public class MyCommandTest : CommandBaseTest
    {
        protected override ICommand GetSut()
        {
            return new MyCommand("123");
        }
    }
}
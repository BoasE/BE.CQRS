using BE.CQRS.Domain.Denormalization;

namespace BE.CQRS.Domain.Tests.DenormalizerLocatorTests
{
    public class GivenDenormalizerLocator
    {
        public DenormalizerLocator GetSut()
        {
            return new DenormalizerLocator();
        }
    }
}
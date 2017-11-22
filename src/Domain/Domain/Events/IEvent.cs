namespace BE.CQRS.Domain.Events
{
    public interface IEvent
    {
        EventHeader Headers { get; }

        bool Validate();

        void AssertValidation();
    }
}
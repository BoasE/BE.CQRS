namespace BE.CQRS.Domain.EventDescriptions
{
    public interface IDescribeableEvent
    {
        string BuildTitle();
        string BuildDescription();
    }
}
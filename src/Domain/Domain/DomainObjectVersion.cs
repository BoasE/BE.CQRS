namespace BE.CQRS.Domain
{
    public static class DomainObjectVersion
    {
        public const int Any = -2;
        public const int Missing = -1;
        public const int Empty = -1;
        public const int Exists = -4;
    }
}